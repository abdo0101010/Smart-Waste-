using SmartWaste.DTO.PickupRequestDTOS;
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
        public List<PickupRequest> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority, string? zone, string? material);
        public bool AcceptPickupRequest(int requestId, int recyclerId);
        public void SaveChanges();
    }}
