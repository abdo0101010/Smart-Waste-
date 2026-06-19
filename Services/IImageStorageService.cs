using Microsoft.AspNetCore.Http;

namespace SmartWaste.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile image, string folder);
        void DeleteImage(string? imagePath);
    }
}