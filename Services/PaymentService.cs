using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartWaste.Hubs;
using SmartWaste.Models;
using SmartWaste.Repositories;
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
        // 🚀 حقن الـ SignalR HubContext عشان الإشعارات اللحظية
        private readonly IHubContext<NotificationHub> _hubContext;

        public PaymentService(IConfiguration configuration, smartwasteContext context, IHubContext<NotificationHub> hubContext)
        {
            _configuration = configuration;
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// أولاً: معالجة عملية الدفع (Bypass ذكي ومستقر لغرض العرض والمناقشة)
        /// </summary>
        public async Task<(Payment Payment, string RedirectUrl)> ProcessPaymentAsync(int requestId, int userId, string paymentMethod, decimal amount)
        {
            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
            if (request == null)
                throw new KeyNotFoundException($"طلب التجميع رقم {requestId} غير موجود.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); // تعديل الـ ID حسب الـ Identity الموحد
            if (user == null)
                throw new KeyNotFoundException($"المستخدم رقم {userId} غير موجود.");

            string citizenName = user.FullName ?? "مواطن EcoSnap";
            int specialReference = RandomNumberGenerator.GetInt32(1000000, 9999999) + requestId;

            var payment = new Payment
            {
                RequestID = requestId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Status = "Success",
                TransactionId = specialReference.ToString(),
                PaymentDate = DateTime.Now
            };

            _context.Payment.Add(payment);
            request.Status = "Paid";

            try
            {
                // 🎯 1. زرع إشعار نجاح الدفع للمواطن
                var payNotification = new Notification
                {
                    Title = "Payment Successful! 💳",
                    Message = $"Your payment of {amount} EGP for request ORD-{requestId} was processed successfully.",
                    Type = "Payment",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    UserId = userId,
                    UserName = citizenName // حماية ضد الـ NULL
                };
                _context.Notifications.Add(payNotification);
            }
            catch (Exception) { }

            await _context.SaveChangesAsync();

            try
            {
                // 🚀 2. ضخ الإشعار لايف عبر الـ SignalR للموبايل
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Payment Successful! 💳", $"Order (ORD-{requestId}) has been paid.", "Payment");
            }
            catch (Exception) { }

            string staticSuccessUrl = "https://cdn.pixabay.com/photo/2017/01/13/01/22/ok-1976099_1280.png";

            return (payment, staticSuccessUrl);
        }

        /// <summary>
        /// ثانياً: تحويل نقاط المستخدم إلى فلوس وخصمها فوراً من الـ DB 
        /// </summary>
        public async Task<bool> TransferPointsToWalletAsync(int userId, string walletNumber, decimal pointsToRedeem)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new KeyNotFoundException("المستخدم غير موجود.");

            if ((user.WalletPoints ?? 0) < pointsToRedeem)
                throw new InvalidOperationException("رصيد نقاطك الحالي لا يكفي لإجراء هذه العملية.");

            decimal conversionRate = 0.1m;
            decimal amountEgp = pointsToRedeem * conversionRate;

            if (amountEgp < 5)
                throw new InvalidOperationException("الحد الأدنى للسحب النقدي الفوري هو 5 جنيهات.");

            var secretKey = _configuration["Paymob:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("خطأ في الإعدادات: الـ SecretKey الخاص بـ Paymob غير موجود.");
            }

            int mockTxId = RandomNumberGenerator.GetInt32(100000, 999999);
            string citizenName = user.FullName ?? "مواطن EcoSnap";

            var redemption = new WalletRedemption
            {
                UserId = userId,
                WalletNumber = walletNumber,
                PointsRedeemed = pointsToRedeem,
                AmountEgp = amountEgp,
                Status = "Success",
                TransactionId = mockTxId.ToString(),
                CreatedAt = DateTime.Now
            };

            user.WalletPoints -= pointsToRedeem;
            _context.WalletRedemptions.Add(redemption);

            try
            {
                // 🎯 3. زرع إشعار تحويل النقاط لكاش في محفظة فودافون كاش أو غيرها
                var cashNotification = new Notification
                {
                    Title = "Points Redeemed to Cash! 💰",
                    Message = $"Successfully transferred {pointsToRedeem} points to wallet {walletNumber}. +{amountEgp} EGP credited.",
                    Type = "Wallet",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    UserId = userId,
                    UserName = citizenName
                };
                _context.Notifications.Add(cashNotification);
            }
            catch (Exception) { }

            await _context.SaveChangesAsync();

            try
            {
                // 🚀 4. ضخ إشعار الكاش لايف بالـ SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Cash Out Successful! 💰", $"{pointsToRedeem} points converted to cash.", "Wallet");
            }
            catch (Exception) { }

            return true;
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
}s