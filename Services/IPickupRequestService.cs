using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IPickupRequestService
    {
        public void AddPickupRequest(PickupRequest pickupRequest);
        public PickupRequest GetPickupRequestById(int id);
        public void UpdatePickupRequest(PickupRequest pickupRequest);
        public void DeletePickupRequest(int id);
        public IEnumerable<PickupRequest> GetAllPickupRequests();
        public List<PickupRequest> GetAllPickupRequestsWithRecyclersAndHubStaff();
        public int GetTotalPickupRequests();
        public decimal? TotalEaring();
        public void SaveChanges();
    }
}
