namespace SmartWaste.Services
{
    using SmartWaste.Repositories;
    using System.Net.Http.Json;

    public class EcoSnapService : IEcoSnapService
    {
        private readonly IUserRepository _userRepository;

        // بنحسـن الـ Repository هنا جوه الـ Service
        public EcoSnapService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> ProcessImageWithAIAsync(int userId, IFormFile file)
        {
            using var httpClient = new HttpClient();
            string aiApiUrl = "https://badass-ecosystem-hazy.ngrok-free.dev/verify-shipment/"; // URL صاحبك

            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "image", file.FileName);

            var response = await httpClient.PostAsync(aiApiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to connect to the AI Model of EcoSnap.");
            }

            var jsonResult = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            if (jsonResult != null && jsonResult.TryGetValue("bottle_count", out int count))
            {
                // حساب النقاط: مثلاً 5 نقاط لكل زجاجة
                decimal pointsEarned = count * 5;

                // نكلم الـ Repository عشان يحفظ في الداتابيز عل طول
                await _userRepository.UpdateUserBottlesAndPointsAsync(userId, count, pointsEarned);

                return count;
            }

            return 0;
        }
    }
}
