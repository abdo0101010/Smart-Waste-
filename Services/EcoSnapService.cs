using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartWaste.Hubs;
using SmartWaste.Models;
using SmartWaste.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartWaste.Services
{
    public class EcoSnapService : IEcoSnapService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly smartwasteContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EcoSnapService(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment, smartwasteContext context, IHubContext<NotificationHub> hubContext)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
                        _hubContext = hubContext;
        }
        public async Task<int> ProcessUserUploadAsync(int userId, IFormFile file)
        {
            // 1. حفظ ملف اليوزر في wwwroot/uploads
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(rootPath, "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string relativeImagePath = "/uploads/" + uniqueFileName;

            // 2. كارييت طلب جديد في الداتابيز وحفظ المسار (Waiting Room Pattern)
            var newRequest = new PickupRequest
            {
                UserId = userId,
                RequestImageUrl = relativeImagePath,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };
            _context.PickupRequests.Add(newRequest);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "New pickup request", "Citizen scheduled a glass pickup...", "Pickup");

            // رجع الـ RequestId عشان الموبايل يحتفظ بيه كـ transaction_id
            return newRequest.RequestId;
        }

        public async Task<int> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter)
        {
            // 1. جلب الطلب بالـ ID لنسحب مسار صورة اليوزر (Before)
            var pickupRequest = await _context.PickupRequests.FirstOrDefaultAsync(p => p.RequestId == transactionId);
            if (pickupRequest == null)
            {
                throw new KeyNotFoundException("رقم العملية (Transaction ID) هذا غير مسجل بالداتابيز.");
            }

            string userImagePath = pickupRequest.RequestImageUrl;
            if (string.IsNullOrEmpty(userImagePath))
            {
                throw new Exception("هذا الطلب لا يحتوي على صورة مرفوعة من المستخدم.");
            }

            // 2. تحويل مسار السيرفر لـ مسار فيزيائي حقيقي للوصول للملف
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullUserImagePath = Path.Combine(rootPath, userImagePath.TrimStart('/'));
            if (!File.Exists(fullUserImagePath))
            {
                throw new FileNotFoundException("صورة المستخدم الأصلية لم تعد موجودة على السيرفر.");
            }

            // 3. حفظ صورة الهاب ستاف (After) الجديدة في الـ uploads للتوثيق
            string uniqueFileNameAfter = Guid.NewGuid().ToString() + "_" + fileAfter.FileName;
            string filePathAfter = Path.Combine(rootPath, "uploads", uniqueFileNameAfter);

            using (var fileStream = new FileStream(filePathAfter, FileMode.Create))
            {
                await fileAfter.CopyToAsync(fileStream);
            }
            pickupRequest.VerificationImageUrl = "/uploads/" + uniqueFileNameAfter;

            // 4. تجهيز الـ HttpClient وضخ الـ Payload الثلاثي للـ FastAPI كـ Bytes وبالمسميات الدقيقة
            using var httpClient = new HttpClient();
            string aiApiUrl = "https://badass-ecosystem-hazy.ngrok-free.dev/verify-shipment/";

            using var content = new MultipartFormDataContent();

            // تحويل الـ file_before لـ ByteArrayContent لضمان قراءتها كاملة
            byte[] fileBeforeBytes = await File.ReadAllBytesAsync(fullUserImagePath);
            var contentBefore = new ByteArrayContent(fileBeforeBytes);
            contentBefore.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(contentBefore, "file_before", "user_image.jpg");

            // تحويل الـ file_after لـ ByteArrayContent لمنع مشاكل الـ Stream Position
            using var streamAfter = fileAfter.OpenReadStream();
            using var memoryStreamAfter = new MemoryStream();
            await streamAfter.CopyToAsync(memoryStreamAfter);
            byte[] fileAfterBytes = memoryStreamAfter.ToArray();

            var contentAfter = new ByteArrayContent(fileAfterBytes);
            contentAfter.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(contentAfter, "file_after", "hub_image.jpg");

            // ضخ الـ transaction_id كـ StringContent بنفس المسمى المستهدف في البايثون
            content.Add(new StringContent(transactionId.ToString()), "transaction_id");

            // 🚀 إرسال الطلب لايف وسيرفر الـ FastAPI هيرد بـ HTTP 200 في الحالتين
            var response = await httpClient.PostAsync(aiApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Server HTTP Error. Status: {response.StatusCode}. Reason: {errorDetail}");
            }

            // 5. قراءة الـ JSON وفحص الـ Logic الداخلي للموديل (مربط الفرس 🎯)
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

            // 🚨 الحالة الأولى: التشيك على الـ status الداخلية لو الـ AI رجع FAILED
            if (jsonResult.TryGetProperty("status", out var statusElement) && statusElement.GetString() == "FAILED")
            {
                string aiMessage = jsonResult.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "فشلت عملية المطابقة وعدم تطابق الكمية.";

                try
                {
                    // 🎯 إرسال نوتفكيشن الفشل للمواطن قبل ما نرمي الـ Exception ونقفل
                    var failNotification = new Notification
                    {
                        Title = "Verification Failed ❌",
                        Message = $"Your request (ORD-{transactionId}) was rejected. Reason: Quantity Mismatch!",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId // موجه للمواطن صاحب الطلب
                    };
                    _context.Notifications.Add(failNotification);
                    await _context.SaveChangesAsync();

                    // ضخها لايف بالـ SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", failNotification.Title, failNotification.Message, failNotification.Type);
                }
                catch (Exception) { /* لضمان عدم كراش الميثود أثناء تسجيل الإشعار الفاشل */ }

                // رمي إيرور صريح يوقف الـ Cycle ويمنع الـ النقاط
                throw new Exception(aiMessage);
            }

            // ✅ الحالة الثانية: لو الـ Status مش FAILED (يعني الـ عملية نجحت ومطابقة للـ Before)
            if (jsonResult.TryGetProperty("count_after", out var countElement))
            {
                int countAfter = countElement.GetInt32();
                decimal pointsEarned = countAfter * 5; // حساب النقاط (كل زجاجة بـ 5 نقاط)

                // تحديث بيانات الـ Request في الـ Database بالقيم النهائية
                pickupRequest.Status = "Verified";
                pickupRequest.FinalBottlesCount = countAfter;
                pickupRequest.FinalPoints = pointsEarned;
                pickupRequest.VerificationDate = DateTime.UtcNow;

                // تحديث إجمالي نقاط وبوتلز المستخدم الحقيقي المربوط بالعملية أوتوماتيكياً
                await _userRepository.UpdateUserBottlesAndPointsAsync(pickupRequest.UserId, countAfter, pointsEarned);

                try
                {
                    // 🎯 إرسال نوتفكيشن النجاح وإضافة النقاط للمواطن
                    var successNotification = new Notification
                    {
                        Title = "Shipment Verified! 🎉",
                        Message = $"Congratulations! (ORD-{transactionId}) verified successfully. +{pointsEarned} points added to your wallet.",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId
                    };
                    _context.Notifications.Add(successNotification);
                }
                catch (Exception) { }

                // حفظ كل شيء (تحديث الطلب + إضافة إشعار النجاح)
                await _context.SaveChangesAsync();

                try
                {
                    // 🚀 ضخ إشعار النجاح لايف فوراً للمواطن بالـ SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Shipment Verified! 🎉", $"Order (ORD-{transactionId}) has been fully verified.", "Pickup");
                }
                catch (Exception) { }

                return countAfter;
            }

            return 0;
        }
    }
}
