using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.AccountDTOS; // تأكد من مطابقة مسميات الـ DTOs حسب مشروعك
using SmartWaste.DTO.PickupRequestDTOS;
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
        private readonly IHubContext<NotificationHub> _hubContext; // تأكد من اسم الـ Hub عندك

        public EcoSnapService(
            IUserRepository userRepository,
            IWebHostEnvironment webHostEnvironment,
            smartwasteContext context,
            IHubContext<NotificationHub> hubContext)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// 1️⃣ خطوة: رفع طلب تجميع جديد بواسطة المواطن (Waiting Room Pattern)
        /// </summary>
        public async Task<int> ProcessUserUploadAsync(int userId, IFormFile file)
        {
            // حفظ ملف اليوزر في wwwroot/uploads
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string relativeImagePath = "/uploads/" + uniqueFileName;

            // كارييت طلب جديد في الداتابيز وحفظ المسار
            var newRequest = new PickupRequest
            {
                UserId = userId,
                RequestImageUrl = relativeImagePath,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.PickupRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            // ضخ إشعار فوري لحظي عبر الـ SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "New pickup request", "Citizen scheduled a glass pickup...", "Pickup");

            // رجع الـ RequestId عشان الموبايل يحتفظ بيه كـ transaction_id
            return newRequest.RequestId;
        }

        /// <summary>
        /// 2️⃣ خطوة: فحص ومطابقة الشحنة بالـ AI واعتماد النقاط (موظف الهاب ستاف)
        /// </summary>
        public async Task<pickupverifyDto> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter)
        {
            // جلب الطلب بالـ ID وعمل Include للـ User والـ Recycler عشان نقرأ الأسماء صح
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
                return new pickupverifyDto
                {
                    Status = "FAILED",
                    Message = "هذا الطلب لا يحتوي على صورة مرفوعة من المستخدم."
                };
            }

            // تحويل مسار السيرفر لـ مسار فيزيائي حقيقي للوصول للملف
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullUserImagePath = Path.Combine(rootPath, userImagePath.TrimStart('/'));

            if (!File.Exists(fullUserImagePath))
            {
                return new pickupverifyDto
                {
                    Status = "FAILED",
                    Message = "صورة المستخدم الأصلية لم تعد موجودة على السيرفر."
                };
            }

            // حفظ صورة الهاب ستاف (After) الجديدة في الـ uploads للتوثيق الجنائي والمالي
            string uniqueFileNameAfter = Guid.NewGuid().ToString() + "_" + fileAfter.FileName;
            string filePathAfter = Path.Combine(rootPath, "uploads", uniqueFileNameAfter);

            using (var fileStream = new FileStream(filePathAfter, FileMode.Create))
            {
                await fileAfter.CopyToAsync(fileStream);
            }
            pickupRequest.VerificationImageUrl = "/uploads/" + uniqueFileNameAfter;

            // تجهيز الـ HttpClient وضخ الـ Payload للـ FastAPI
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

            // قراءة الـ JSON وفحص الـ Logic الداخلي للموديل
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();

            // 🚨 الحالة الأولى: لو الـ AI رجع FAILED بشكل صريح (عدم تطابق الكمية)
            if (jsonResult.TryGetProperty("status", out var statusElement) && statusElement.GetString() == "FAILED")
            {
                string aiMessage = jsonResult.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Quantity Mismatch Error!";
                int countBefore = jsonResult.TryGetProperty("count_before", out var cb) ? cb.GetInt32() : 0;
                int countAfter = jsonResult.TryGetProperty("count_after", out var ca) ? ca.GetInt32() : 0;
                double score = jsonResult.TryGetProperty("similarity_score", out var ss) ? ss.GetDouble() : 0.0;

                // تحديث حالة الطلب في الداتابيز وإسناد الموظف المسؤول فوراً لمنع كراش الـ Tracking
                pickupRequest.Status = "Failed";
                pickupRequest.HubStaffId = userId; // 👈 حفظ معرف موظف الفرز الحالي

                try
                {
                    // إرسال نوتفكيشن الفشل للمواطن
                    var failNotification = new Notification
                    {
                        Title = "Verification Failed ❌",
                        Message = $"Your request (ORD-{transactionId}) was rejected. Reason: {aiMessage}",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId,
                        UserName = citizenName
                    };
                    _context.Notifications.Add(failNotification);
                }
                catch (Exception) { }

                // الحفظ الآمن في الداتابيز
                await _context.SaveChangesAsync();

                try
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Verification Failed ❌", $"Request (ORD-{transactionId}) was rejected.", "Pickup");
                }
                catch (Exception) { }

                return new pickupverifyDto
                {
                    Status = "FAILED",
                    FinalBottlesCount = 0,
                    FinalPoints = 0,
                    Message = aiMessage,
                    CountBefore = countBefore,
                    CountAfter = countAfter,
                    SimilarityScore = score
                };
            }

            // ✅ الحالة الثانية: لو الـ عملية نجحت ومطابقة للـ Before
            if (jsonResult.TryGetProperty("count_after", out var countElement))
            {
                int countAfter = countElement.GetInt32();
                int countBefore = jsonResult.TryGetProperty("count_before", out var cb) ? cb.GetInt32() : countAfter;
                double score = jsonResult.TryGetProperty("similarity_score", out var ss) ? ss.GetDouble() : 1.0;

                decimal pointsEarned = countAfter * 5;

                // 1️⃣ تحديث بيانات العملية وحفظ الـ ID لموظف الفرز الحالي فوراً
                pickupRequest.Status = "Verified";
                pickupRequest.HubStaffId = userId; // 👈 تم التثبيت هنا قبل أي SaveChanges خارجية
                pickupRequest.FinalBottlesCount = countAfter;
                pickupRequest.FinalPoints = pointsEarned;
                pickupRequest.VerificationDate = DateTime.UtcNow;

                var pick = new pickupverifyDto
                {
                    Status = "Verified",
                    FinalBottlesCount = countAfter,
                    FinalPoints = (int)pointsEarned,
                    CountBefore = countBefore,
                    CountAfter = countAfter,
                    SimilarityScore = score,
                    Message = "Shipment verified successfully!"
                };

                // 2️⃣ إضافة إشعارات النجاح والشكر للـ Context
                try
                {
                    // 🎯 إرسال نوتفكيشن النجاح وإضافة النقاط للمواطن
                    var successNotification = new Notification
                    {
                        Title = "Shipment Verified! 🎉",
                        Message = $"Congratulations! Your request (ORD-{transactionId}) verified successfully. +{pointsEarned} points added to your wallet.",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        UserId = pickupRequest.UserId,
                        UserName = citizenName
                    };
                    _context.Notifications.Add(successNotification);

                    // 🎯 إرسال نوتفكيشن شكر للسواق
                    if (pickupRequest.RecyclerId.HasValue)
                    {
                        var driverNotification = new Notification
                        {
                            Title = "Trip Closed 🏁",
                            Message = $"Great job {driverName}! Request ORD-{transactionId} has been fully processed.",
                            Type = "Logistics",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            RecyclerId = pickupRequest.RecyclerId.Value,
                            UserName = citizenName
                        };
                        _context.Notifications.Add(driverNotification);
                    }
                }
                catch (Exception) { }

                // 3️⃣ الحفظ الرسمي والنهائي للطلب والإشعارات معاً داخل الـ SQL Server 
                await _context.SaveChangesAsync();

                // 4️⃣ الـ Repo الخارجي يشتغل لتحديث المحفظة بعد الاطمئنان على حفظ الـ HubStaffId
                try
                {
                    await _userRepository.UpdateUserBottlesAndPointsAsync(pickupRequest.UserId, countAfter, pointsEarned);
                }
                catch (Exception) { }

                // 5️⃣ بث الـ SignalR لايف
                try
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Shipment Verified! 🎉", $"Order (ORD-{transactionId}) has been fully verified.", "Pickup");
                }
                catch (Exception) { }

                return pick;
            }

            return null;
        }
    }
}