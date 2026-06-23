using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        public smartwasteContext _context;

        public EcoSnapService(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment, smartwasteContext context)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
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

            // رجع الـ RequestId عشان الموبايل يحتفظ بيه كـ transaction_id
            return newRequest.RequestId;
        }

        // ====== [ الخطوة الثانية: الهاب ستاف يفحص ويضرب الـ AI ] ======
        public async Task<int> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter)
        {
            // 1. هنجيب الطلب بالـ ID ونلقط صورة اليوزر المحفوظة
            var pickupRequest = await _context.PickupRequests.FirstOrDefaultAsync(p => p.RequestId == transactionId);
            if (pickupRequest == null) throw new KeyNotFoundException("رقم العملية (Transaction ID) هذا غير مسجل بالداتابيز.");

            string userImagePath = pickupRequest.RequestImageUrl;
            if (string.IsNullOrEmpty(userImagePath)) throw new Exception("هذا الطلب لا يحتوي على صورة مرفوعة من المستخدم.");

            // 2. تحويل مسار السيرفر لـ Stream حقيقي
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullUserImagePath = Path.Combine(rootPath, userImagePath.TrimStart('/'));
            if (!File.Exists(fullUserImagePath)) throw new FileNotFoundException("صورة المستخدم الأصلية لم تعد موجودة على السيرفر.");

            // 3. حفظ صورة الهاب ستاف الجديدة برضه للتوثيق
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

            // ضخ الـ file_before (صورة اليوزر من السيرفر) بدون using فرعي
            var fileStreamBefore = new FileStream(fullUserImagePath, FileMode.Open, FileAccess.Read);
            var contentBefore = new StreamContent(fileStreamBefore);
            content.Add(contentBefore, "file_before", Path.GetFileName(fullUserImagePath));

            // ضخ الـ file_after (صورة الهاب ستاف الحالية)
            var streamAfter = fileAfter.OpenReadStream();
            var contentAfter = new StreamContent(streamAfter);
            content.Add(contentAfter, "file_after", fileAfter.FileName);

            // ضخ الـ transaction_id
            content.Add(new StringContent(transactionId.ToString()), "transaction_id");

            // 🚀 إرسال الطلب لايف للـ FastAPI
            var response = await httpClient.PostAsync(aiApiUrl, content);

            // قفل الـ Stream اليدوي فوراً بعد الإرسال
            fileStreamBefore.Close();

            if (!response.IsSuccessStatusCode)
            {
                var errorDetail = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Server rejected the payload. Status: {response.StatusCode}. Reason: {errorDetail}");
            }

            // 5. قراءة الفحص وحساب النقاط
            var jsonResult = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (jsonResult.TryGetProperty("count_after", out var countElement))
            {
                int countAfter = countElement.GetInt32();
                decimal pointsEarned = countAfter * 5;

                // تحديث الداتابيز بالبيانات النهائية
                pickupRequest.Status = "Verified";
                pickupRequest.FinalBottlesCount = countAfter;
                pickupRequest.FinalPoints = pointsEarned;
                pickupRequest.VerificationDate = DateTime.UtcNow;

                await _userRepository.UpdateUserBottlesAndPointsAsync(pickupRequest.UserId, countAfter, pointsEarned);
                await _context.SaveChangesAsync();

                return countAfter;
            }

            return 0;
        }
    }
}
