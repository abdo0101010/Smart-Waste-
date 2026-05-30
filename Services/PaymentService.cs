using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartWaste.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartWaste.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly smartwasteContext _context;

        public PaymentService(IConfiguration configuration, smartwasteContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// أولاً: معالجة عملية الدفع (من المستخدم للسيستم) لطلب تجميع مخلفات معين
        /// </summary>
        public async Task<(Payment Payment, string RedirectUrl)> ProcessPaymentAsync(int requestId, int userId, string paymentMethod, decimal amount)
        {
            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
            if (request == null)
                throw new KeyNotFoundException($"طلب التجميع رقم {requestId} غير موجود.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                throw new KeyNotFoundException($"المستخدم رقم {userId} غير موجود.");

            using var httpClient = new HttpClient();

            string secretKey = _configuration["Paymob:SecretKey"] ?? throw new ArgumentException("Paymob Secret Key is missing in appsettings.");
            string publicKey = _configuration["Paymob:PublicKey"] ?? throw new ArgumentException("Paymob Public Key is missing in appsettings.");

            int specialReference = RandomNumberGenerator.GetInt32(1000000, 9999999) + requestId;
            var amountCents = (int)(amount * 100); // تحويل المبلغ لقروش

            var billingData = new
            {
                apartment = "N/A",
                first_name = user.FullName ?? "SmartWaste Customer",
                last_name = "User",
                street = user.Address ?? "N/A",
                building = "N/A",
                phone_number = "01000000000",
                country = "Egypt",
                email = user.Email,
                floor = "N/A",
                state = "N/A",
                city = "N/A"
            };

            int integrationId = int.Parse(DetermineIntegrationId(paymentMethod));

            var payload = new
            {
                amount = amountCents,
                currency = "EGP",
                payment_methods = new[] { integrationId },
                billing_data = billingData,
                items = new[]
                {
                    new
                    {
                        name = $"SmartWaste Order #{specialReference}",
                        amount = amountCents,
                        description = $"Payment for Pickup Request #{requestId}",
                        quantity = 1
                    }
                },
                customer = new
                {
                    first_name = billingData.first_name,
                    last_name = billingData.last_name,
                    email = billingData.email,
                    extras = new { requestId = requestId }
                },
                extras = new
                {
                    requestId = requestId,
                    userId = user.UserId
                },
                special_reference = specialReference,
                expiration = 3600,
                merchant_order_id = specialReference.ToString()
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", secretKey);
            requestMessage.Content = JsonContent.Create(payload);

            var response = await httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Paymob Intention API failed: {responseContent}");

            var resultJson = JsonDocument.Parse(responseContent);
            var clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString();

            // تسجيل العملية في جدول الـ Payments المصلح عندك
            var payment = new Payment
            {
                RequestID = requestId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = "Pending",
                TransactionId = specialReference.ToString(),
                PaymentDate = DateTime.Now
            };

            _context.Payment.Add(payment);
            request.Status = "Pending Payment";

            await _context.SaveChangesAsync();

            string redirectUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            return (payment, redirectUrl);
        }

        /// <summary>
        /// ثانياً: تحويل نقاط المستخدم إلى فلوس وإرسالها فوراً إلى رقم محفظته (Disbursement)
        /// </summary>
        public async Task<bool> TransferPointsToWalletAsync(int userId, string walletNumber, decimal pointsToRedeem)
        {
            // 1. التأكد من وجود المستخدم ورصيد نقاطه الحالي
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) throw new KeyNotFoundException("المستخدم غير موجود.");

            if ((user.WalletPoints ?? 0) < pointsToRedeem)
                throw new InvalidOperationException("رصيد نقاطك الحالي لا يكفي لإجراء هذه العملية.");

            // 2. الحسبة المالية (كل 10 نقط = 1 جنيه)
            decimal conversionRate = 0.1m;
            decimal amountEgp = pointsToRedeem * conversionRate;

            if (amountEgp < 5)
                throw new InvalidOperationException("الحد الأدنى للسحب النقدي الفوري هو 5 جنيهات.");

            // 3. تسجيل العملية مؤقتاً بوضع Pending وخصم النقط من اليوزر
            var redemption = new WalletRedemption
            {
                UserId = userId,
                WalletNumber = walletNumber,
                PointsRedeemed = pointsToRedeem,
                AmountEgp = amountEgp,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            user.WalletPoints -= pointsToRedeem;
            _context.WalletRedemptions.Add(redemption);
            await _context.SaveChangesAsync();

            // 4. استدعاء الـ API الفوري الخاص بـ Paymob Disbursement للتحويل على الرقم
            using var httpClient = new HttpClient();
            string disbursementToken = _configuration["Paymob:DisbursementToken"] ?? throw new ArgumentException("Disbursement Token is missing in appsettings.");

            var payload = new
            {
                amount = (int)(amountEgp * 100), // المبلغ بالقروش
                currency = "EGP",
                wallet_number = walletNumber,
                merchant_command_id = redemption.RedemptionId.ToString(), // ربط العملية بالـ Primary Key بتاعنا
                issuer = "VODAFONE"
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/disbursement/disburse/");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", disbursementToken);
            requestMessage.Content = JsonContent.Create(payload);

            var response = await httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var resultJson = JsonDocument.Parse(responseContent);
                if (resultJson.RootElement.TryGetProperty("transaction_id", out var txId))
                {
                    redemption.TransactionId = txId.ToString();
                }

                redemption.Status = "Success"; // الفلوس وصلت للعميل بنجاح والمحفظة استلمت
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                // إذا فشل تحويل البوابة لأي سبب (مثلاً رقم غلط)، يتم رد النقط فوراً للعميل
                user.WalletPoints += pointsToRedeem;
                redemption.Status = "Failed";
                await _context.SaveChangesAsync();

                throw new Exception($"فشل سيرفر الدفع في إتمام عملية الصرف النقدي: {responseContent}");
            }
        }

        public async Task<Payment> UpdateOrderSuccess(string specialReference)
        {
            var payment = await _context.Payment.FirstOrDefaultAsync(p => p.TransactionId == specialReference);
            if (payment == null) throw new KeyNotFoundException("Payment record not found.");

            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == payment.RequestID);

            payment.Status = "Success";
            if (request != null) request.Status = "Paid";

            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateOrderFailed(string specialReference)
        {
            var payment = await _context.Payment.FirstOrDefaultAsync(p => p.TransactionId == specialReference);
            if (payment == null) throw new KeyNotFoundException("Payment record not found.");

            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == payment.RequestID);

            payment.Status = "Failed";
            if (request != null) request.Status = "Payment Failed";

            await _context.SaveChangesAsync();
            return payment;
        }

        public string ComputeHmacSHA512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private string DetermineIntegrationId(string paymentMethod)
        {
            return paymentMethod?.ToLower() switch
            {
                "card" => _configuration["Paymob:CardIntegrationId"] ?? throw new ArgumentException("Card integration ID missing"),
                "wallet" => _configuration["Paymob:WalletIntegrationId"] ?? throw new ArgumentException("Wallet integration ID missing"),
                _ => throw new ArgumentException($"Invalid payment method: {paymentMethod}")
            };
        }
    }
}