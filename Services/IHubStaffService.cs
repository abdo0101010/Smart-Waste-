using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface IHubStaffService: IHubStaffRepository
    {
        public  Task<bool> VerifyShipmentWithAIAsync(IFormFile fileBefore, IFormFile fileAfter, int transactionId);


    }
}
