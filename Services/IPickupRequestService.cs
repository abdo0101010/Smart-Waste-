using SmartWaste.DTO.PickupRequestDTOS;
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
        public PickupInfoDTOS GetTodayPickupSummary();

        public bool AcceptPickupRequest(int requestId, int recyclerId);
        public List<PickupRequestViewModelDTO> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority, string? zone, string? material);
        public void SaveChanges();
    }
}
