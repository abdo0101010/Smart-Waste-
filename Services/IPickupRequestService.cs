using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface IPickupRequestService
    {
        List<PickupRequestViewModelDTO> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority, string? zone, string? material);

        // حل الإيرور الثالث: إضافة تعريف الـ History جوه انترفيس السيرفيس
        Task<IEnumerable<PickupRequestViewModelDTO>> GetUserHistoryAsync(int userId);

        // حل الإيرور الثاني: إضافة تعريف الـ Summary جوه انترفيس السيرفيس
        PickupInfoDTOS GetTodayPickupSummary();

        // باقي الميثودز الأساسية اللي الـ Controller بيحتاجها من السيرفيس
        void AddPickupRequest(PickupRequest pickupRequest);
        PickupRequest GetPickupRequestById(int id);
        void UpdatePickupRequest(PickupRequest pickupRequest);
        void DeletePickupRequest(int id);
        IEnumerable<PickupRequest> GetAllPickupRequests();
        List<PickupRequest> GetAllPickupRequestsWithRecyclersAndHubStaff();
        int GetTotalPickupRequests();
        decimal? TotalEaring();
        public  Task<bool> AcceptBulkPickupRequestsAsync(List<int> requestIds, int recyclerId);
        public Task<IEnumerable<PendingRequestFormDTO>> GetPendingHubRequestsAsync();
        public Task<IEnumerable<PickupRequest>> GetRequestsByRecyclerIdAsync(int recyclerId);
        public  Task<IEnumerable<PickupRequestViewModelDTO>> GetRecyclerHistoryAsync(int recyclerId);


        void SaveChanges();
    }
}
