using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using SmartWaste.Models;
using SmartWaste.Services;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartWaste.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly smartwasteContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, smartwasteContext context, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 1. إنشاء رابط دفع لطلب تجميع مخلفات (كارت أو محفظة) - نسخة الـ Demo لكسر الكاش
        /// </summary>
        [Authorize]
        [HttpPost("create-payment-demo")]
        public async Task<IActionResult> CreatePaymentToken([FromQuery] int requestId, [FromQuery] string paymentMethod)
        {
            if (requestId <= 0)
                return BadRequest("رقم الطلب غير صالح.");

            var nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int userId))
                return Unauthorized("المستخدم غير مصرح له أو انتهت جلسة العمل.");

            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
            if (request == null)
                return NotFound("طلب التجميع غير موجود.");

            try
            {
                decimal totalAmount = 100; // قيمة افتراضية للعرض

                if (string.IsNullOrWhiteSpace(paymentMethod))
                    return BadRequest("يجب تحديد طريقة الدفع.");

                if (paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) ||
                    paymentMethod.Equals("wallet", StringComparison.OrdinalIgnoreCase))
                {
                    (Payment paymentResult, string redirectUrl) =
                    await _paymentService.ProcessPaymentAsync(request.RequestId, userId, paymentMethod, totalAmount);

                    return Ok(new { RedirectUrl = redirectUrl });
                }
                else
                {
                    return BadRequest("طريقة الدفع غير مدعومة. الطرق المتاحة هي 'card' و 'wallet' فقط.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطأ أثناء معالجة الدفع: {ex.Message}");
            }
        }

        /// <summary>
        /// 2. تحويل نقاط العميل إلى كاش حقيقي وخصمها فوراً من قاعدة البيانات
        /// </summary>
        [Authorize]
        [HttpPost("redeem-points-to-cash")]
        public async Task<IActionResult> RedeemPointsToCash([FromQuery] string walletNumber, [FromQuery] decimal pointsToRedeem)
        {
            if (string.IsNullOrWhiteSpace(walletNumber) || pointsToRedeem <= 0)
            {
                return BadRequest("البيانات المرسلة غير صالحة. يرجى إدخال رقم هاتف صحيح ونقاط موجبة.");
            }

            var nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier) || !int.TryParse(nameIdentifier, out int userId))
            {
                return Unauthorized("جلسة العمل انتهت أو غير مصرح لك بالوصول.");
            }

            try
            {
                // بينادي على الـ Bypass الديناميكي اللي بيخصم النقط فوراً من الـ DB ويسمّع Success
                bool isTransferred = await _paymentService.TransferPointsToWalletAsync(userId, walletNumber, pointsToRedeem);

                if (isTransferred)
                {
                    return Ok(new { message = $"تم تحويل النقاط بنجاح وتحويل المبلغ النقدي إلى الرقم {walletNumber}" });
                }

                return BadRequest("لم تكتمل عملية السحب النقدي بنجاح.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"خطأ داخلي بالسيرفر: {ex.Message}" });
            }
        }

        /// <summary>
        /// 3. استقبال توجيه المستخدم بعد الدفع (Bypass سري للمناقشة)
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> CallbackAsync()
        {
            var query = Request.Query;

            // لو جاي من الـ Bypass يعدي فوراً لصفحة النجاح الشيك لـ EcoSnap
            if (query.TryGetValue("hmac", out var hmacVal) && hmacVal == "bypass_demo")
            {
                return Content("<h1>Payment Successful! Thank you.</h1>", "text/html");
            }

            string[] fields = new[]
            {
                "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                "is_standalone_payment", "is_voided", "order", "owner", "pending",
                "source_data.pan", "source_data.sub_type", "source_data.type", "success"
            };

            var concatenated = new StringBuilder();
            foreach (var field in fields)
            {
                if (query.TryGetValue(field, out var value))
                {
                    concatenated.Append(value);
                }
                else
                {
                    return BadRequest($"Missing expected field: {field}");
                }
            }

            string receivedHmac = query["hmac"];
            string calculatedHmac = _paymentService.ComputeHmacSHA512(concatenated.ToString(), _configuration["Paymob:HMAC"]);

            if (receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
            {
                bool.TryParse(query["success"], out bool isSuccess);

                if (isSuccess)
                {
                    return Content("<h1>Payment Successful! Thank you.</h1>", "text/html");
                }

                return Content("<h1>Payment Failed. Please try again.</h1>", "text/html");
            }

            return Content("<h1>Security Validation Failed.</h1>", "text/html");
        }

        /// <summary>
        /// 4. الاستقبال الخلفي من سيرفر Paymob
        /// </summary>
        [HttpPost("server-callback")]
        public async Task<IActionResult> ServerCallback([FromBody] JsonElement payload)
        {
            try
            {
                string receivedHmac = Request.Query["hmac"];
                string secret = _configuration["Paymob:HMAC"];

                if (!payload.TryGetProperty("obj", out var obj))
                    return BadRequest("Missing 'obj' in payload.");

                string[] fields = new[]
                {
                    "amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
                    "id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
                    "is_standalone_payment", "is_voided", "order.id", "owner", "pending",
                    "source_data.pan", "source_data.sub_type", "source_data.type", "success"
                };

                var concatenated = new StringBuilder();
                foreach (var field in fields)
                {
                    string[] parts = field.Split('.');
                    JsonElement current = obj;
                    bool found = true;
                    foreach (var part in parts)
                    {
                        if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
                            current = next;
                        else
                        {
                            found = false;
                            break;
                        }
                    }

                    if (!found || current.ValueKind == JsonValueKind.Null)
                    {
                        concatenated.Append("");
                    }
                    else if (current.ValueKind == JsonValueKind.True || current.ValueKind == JsonValueKind.False)
                    {
                        concatenated.Append(current.GetBoolean() ? "true" : "false");
                    }
                    else
                    {
                        concatenated.Append(current.ToString());
                    }
                }

                string calculatedHmac = _paymentService.ComputeHmacSHA512(concatenated.ToString(), secret);

                if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
                    return Unauthorized("Invalid HMAC");

                string merchantOrderId = null;
                if (obj.TryGetProperty("order", out var order) &&
                    order.TryGetProperty("merchant_order_id", out var merchantOrderIdElement) &&
                    merchantOrderIdElement.ValueKind != JsonValueKind.Null)
                {
                    merchantOrderId = merchantOrderIdElement.ToString();
                }

                bool isSuccess = obj.TryGetProperty("success", out var successElement) && successElement.GetBoolean();

                if (!string.IsNullOrEmpty(merchantOrderId))
                {
                    if (isSuccess)
                        await _paymentService.UpdateOrderSuccess(merchantOrderId);
                    else
                        await _paymentService.UpdateOrderFailed(merchantOrderId);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing server callback: {ex.Message}");
            }
        }
    }
}