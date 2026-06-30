using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IEcoSnapService
    {
        Task<int> ProcessUserUploadAsync(int userId, IFormFile file);

        // الخطوة الثانية للهاب ستاف
        Task<pickupverifyDto> VerifyHubShipmentAsync(int userId, int transactionId, IFormFile fileAfter);
    }

}

