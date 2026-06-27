using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IPickupRequestRepository
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
        List<PickupRequest> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority, string? zone, string? material);
        public void SaveChanges();
        Task<IEnumerable<PickupRequest>> GetRequestsByUserIdAsync(int userId);
        public Task<bool> AcceptBulkPickupRequestsAsync(List<int> requestIds, int recyclerId);
        public Task<IEnumerable<PickupRequest>> GetRequestsByRecyclerIdAsync(int recyclerId);
        public Task<IEnumerable<PickupRequestViewModelDTO>> GetRecyclerHistoryAsync(int recyclerId);



        public Task<IEnumerable<PendingRequestFormDTO>> GetPendingHubRequestsAsync();


    }
}
