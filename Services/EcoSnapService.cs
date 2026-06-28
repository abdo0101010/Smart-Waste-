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

            // 🎯 خطوة 1: جلب اسم المواطن عشان نمنع الـ NULL في جدول الـ Notifications
            var citizen = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
            string citizenName = citizen != null ? citizen.FullName : "مستخدم جديد";

            try
            {
                // 🎯 خطوة 2: زرع إشعار الـ Request الجديد في الـ History للـ Hub Staff والأدمن
                var hubNotification = new Notification
                {
                    Title = "New Request Alert! 📦",
                    Message = $"Citizen ({citizenName}) uploaded a new request (ORD-{newRequest.RequestId}) waiting for routing.",
                    Type = "HubAlert",
                    CreatedAt = DateTime.UtcNow,
                    UserName = citizenName // 👈 قفلنا الـ NULL constraint هنا
                };
                _context.Notifications.Add(hubNotification);
                await _context.SaveChangesAsync();
            }
            catch (Exception) { /* لضمان عدم كراش العملية الأساسية */ }

            // ضخها لايف عبر الـ SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "New pickup request", $"{citizenName} scheduled a new plastic recycling pickup.", "Pickup");

            return newRequest.RequestId;
        }

        public async Task<int> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter)
        {
            // 1. جلب الطلب بالـ ID وعمل Include للـ User والـ Recycler عشان نقرأ الأسماء صح
            var pickupRequest = await _context.PickupRequests
                .Include(p => p.User)
                .Include(p => p.Recycler)
                .FirstOrDefaultAsync(p => p.RequestId == transactionId);

            if (pickupRequest == null)
            {
                throw new KeyNotFoundException("رقم العملية (Transaction ID) هذا غير مسجل بالداتابيز.");
            }

            string citizenName = pickupRequest.User != null ? pickupRequest.User.FullName : "مواطن EcoSnap";
            string driverName = pickupRequest.Recycler != null ? pickupRequest.Recycler.FullName : "سائق EcoSnap";

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

            // 4. تجهيز الـ HttpClient وضخ الـ Payload الثلاثي للـ FastAPI
            using var httpClient = new HttpClient();
            string aiApiUrl = "https://badass-ecosystem-hazy.ngrok-free.dev/verify-shipment/";

            using var content = new MultipartFormDataContent();

            byte[] fileBeforeBytes = await File.ReadAllBytesAsync(fullUserImagePath);
            var contentBefore = new ByteArrayContent(fileBeforeBytes);
            contentBefore.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(contentBefore, "file_before", "user_image.jpg");

            using var streamAfter = fileAfter.OpenReadStream();
            using var memoryStreamAfter = new MemoryStream();
            await streamAfter.CopyToAsync(memoryStreamAfter);
            byte[] fileAfterBytes = memoryStreamAfter.ToArray();

            var contentAfter = new ByteArrayContent(fileAfterBytes);
            contentAfter.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(contentAfter, "file_after", "hub_image.jpg");

            content.Add(new StringContent(transactionId.ToString()), "transaction_id");

            var response = await httpClient.PostAsync(aiApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Server HTTP Error. Status: {response.StatusCode}. Reason: {errorDetail}");
            }

            // 5. قراءة الـ JSON وفحص الـ Logic الداخلي للموديل
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

            // 🚨 الحالة الأولى: لو الـ AI رجع FAILED
            if (jsonResult.TryGetProperty("status", out var statusElement) && statusElement.GetString() == "FAILED")
            {
                string aiMessage = jsonResult.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "فشلت عملية المطابقة وعدم تطابق الكمية.";

                try
                {
                    // 🎯 إرسال نوتفكيشن الفشل للمواطن مع حقن الـ UserName لمنع الـ NULL error
                    var failNotification = new Notification
                    {
                        Title = "Verification Failed ❌",
                        Message = $"Your request (ORD-{transactionId}) was rejected. Reason: Quantity Mismatch!",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId,
                        UserName = citizenName // ✅ حل أزمة الـ DB Constraint
                    };
                    _context.Notifications.Add(failNotification);
                    await _context.SaveChangesAsync();

                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", failNotification.Title, failNotification.Message, failNotification.Type);
                }
                catch (Exception) { }

                throw new Exception(aiMessage);
            }

            // ✅ الحالة الثانية: لو الـ عملية نجحت ومطابقة للـ Before
            if (jsonResult.TryGetProperty("count_after", out var countElement))
            {
                int countAfter = countElement.GetInt32();
                decimal pointsEarned = countAfter * 5;

                pickupRequest.Status = "Verified";
                pickupRequest.FinalBottlesCount = countAfter;
                pickupRequest.FinalPoints = pointsEarned;
                pickupRequest.VerificationDate = DateTime.UtcNow;

                await _userRepository.UpdateUserBottlesAndPointsAsync(pickupRequest.UserId, countAfter, pointsEarned);

                try
                {
                    // 🎯 1. إرسال نوتفكيشن النجاح وإضافة النقاط للمواطن
                    var successNotification = new Notification
                    {
                        Title = "Shipment Verified! 🎉",
                        Message = $"Congratulations! Your request (ORD-{transactionId}) verified successfully. +{pointsEarned} points added to your wallet.",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId,
                        UserName = citizenName // ✅ قفلنا الـ NULL هنا برضه
                    };
                    _context.Notifications.Add(successNotification);

                    // 🎯 2. إرسال نوتفكيشن شكر وتقفيل للسواق اللي وصل الشحنة
                    if (pickupRequest.RecyclerId.HasValue)
                    {
                        var driverNotification = new Notification
                        {
                            Title = "Trip Closed 🏁",
                            Message = $"Great job {driverName}! Request ORD-{transactionId} has been fully processed and points delivered to {citizenName}.",
                            Type = "Logistics",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            RecyclerId = pickupRequest.RecyclerId.Value,
                            UserName = citizenName // أو "System"
                        };
                        _context.Notifications.Add(driverNotification);
                    }
                }
                catch (Exception) { }

                await _context.SaveChangesAsync();

                try
                {
                    // 🚀 ضخ إشعار النجاح لايف فوراً للمواطن والسائق بالـ SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Shipment Verified! 🎉", $"Order (ORD-{transactionId}) has been fully verified and closed.", "Pickup");
                }
                catch (Exception) { }

                return countAfter;
            }

            return 0;
        }
    }
}