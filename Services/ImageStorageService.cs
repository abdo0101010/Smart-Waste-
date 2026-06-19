using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace SmartWaste.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 ميجابايت

        public ImageStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile image, string folder)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("الصورة فاضية أو مش موجودة");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("نوع الملف غير مسموح به. مسموح فقط: jpg, jpeg, png, webp");

            if (image.Length > MaxFileSizeBytes)
                throw new ArgumentException("حجم الصورة كبير جداً، الحد الأقصى 5 ميجابايت");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        public void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}