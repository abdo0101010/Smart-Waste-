namespace SmartWaste.Services
{
    public interface IEcoSnapService
    {
        Task<int> ProcessUserUploadAsync(int userId, IFormFile file);

        // الخطوة الثانية للهاب ستاف
        Task<object> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter);
    }

}

