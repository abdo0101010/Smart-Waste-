namespace SmartWaste.Services
{
    public interface IEcoSnapService
    {
        Task<int> ProcessImageWithAIAsync(int userId, IFormFile file);
    }
}
