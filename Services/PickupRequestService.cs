using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class PickupRequestService: IPickupRequestService
    {
        IPickupRequestRepository _pickupRequestRepository;
        public PickupRequestService(IPickupRequestRepository pickupRequestRepository)
        {
            _pickupRequestRepository = pickupRequestRepository;
        }

        public void AddPickupRequest(PickupRequest pickupRequest)
        {
            if(pickupRequest != null)
            {
                _pickupRequestRepository.AddPickupRequest(pickupRequest);
            }
        }
        public PickupRequest GetPickupRequestById(int id)
        {
            if(id > 0)
            {
                return _pickupRequestRepository.GetPickupRequestById(id);
            }
            return null;
        }
        public void UpdatePickupRequest(PickupRequest pickupRequest)
        {
            if(pickupRequest != null)
            {
                _pickupRequestRepository.UpdatePickupRequest(pickupRequest);
            }
        }
        public void DeletePickupRequest(int id)
        {
            if(id > 0)
            {
                _pickupRequestRepository.DeletePickupRequest(id);
            }
        }

        public IEnumerable<PickupRequest> GetAllPickupRequests()
        {
            return _pickupRequestRepository.GetAllPickupRequests();
        }

        public List<PickupRequest> GetAllPickupRequestsWithRecyclersAndHubStaff()
        {
            return _pickupRequestRepository.GetAllPickupRequestsWithRecyclersAndHubStaff();
        }

         public int GetTotalPickupRequests()
        {
            return _pickupRequestRepository.GetTotalPickupRequests();
        }
        public decimal? TotalEaring()
        {
            return _pickupRequestRepository.TotalEaring();
        }
        public PickupInfoDTOS GetTodayPickupSummary()
        {
            return _pickupRequestRepository.GetTodayPickupSummary();
        }
        public List<PickupRequestViewModelDTO> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority, string? zone, string? material)
        {
            // 1. استدعاء الـ Repo لجلب البيانات الخام
            var requests = _pickupRequestRepository.GetRecyclerRequestsWithFilters(search, status, priority, zone, material);

            // 2. حل إيرورز الـ Mapping: القراءة من الـ Navigation Properties الصحيحة للـ Entity
            var dtoList = requests.Select(p => new PickupRequestViewModelDTO
            {
                RequestId = p.RequestId,
                Status = p.Status ?? "Pending",
                Priority = p.Priority ?? "Normal",

                // حل إيرور CitizenName: ندخل لجدول الـ User ومنه الـ FullName
                CitizenName = p.User?.FullName ?? "N/A",

                // حل إيرور Zone: ندخل لجدول الـ User ومنه الـ Address
                Zone = p.User?.Address ?? "N/A",

                // حل إيرور CategoryName: ندخل جوه لستة الـ RequestItems ونأخذ أول فئة للمخلفات
                CategoryName = p.RequestItems.FirstOrDefault()?.Category?.CategoryName ?? "N/A"

            }).ToList();

            return dtoList;
        }

      public async Task<bool> AcceptBulkPickupRequestsAsync(List<int> requestIds, int recyclerId)
        {
            return await _pickupRequestRepository.AcceptBulkPickupRequestsAsync(requestIds, recyclerId);
        }
        public async Task<IEnumerable<PickupRequestViewModelDTO>> GetUserHistoryAsync(int userId)
        {
            var requests = await _pickupRequestRepository.GetRequestsByUserIdAsync(userId);

            return requests.Select(r => new PickupRequestViewModelDTO
            {
                RequestId = r.RequestId,
                CitizenName = r.User?.FullName ?? "N/A",
                Status = r.Status ?? "Pending",
                Priority = r.Priority ?? "Normal",
                Zone = r.User?.Address ?? "N/A",
                // حل مشكلة الـ decimal? لـ int صريح
                PointsEarned = r.FinalPoints.HasValue ? Convert.ToInt32(r.FinalPoints.Value) : 0,
                RequestImageUrl = r.RequestImageUrl ?? string.Empty,
                // حل مشكلة الـ DateTime? لـ DateTime صريح
                CreatedAt = r.RequestDate.GetValueOrDefault(DateTime.Now)
            }).ToList();
        }
        public async Task<IEnumerable<PendingRequestFormDTO>> GetPendingHubRequestsAsync()
        {
            return await _pickupRequestRepository.GetPendingHubRequestsAsync();
        }

        //public async Task<IEnumerable<PickupRequestViewModelDTO>> GetRecyclerHistoryAsync(int recyclerId)
        //{
        //    var requests = await _pickupRequestRepository.GetRequestsByRecyclerIdAsync(recyclerId);
        //    return requests.Select(r => new PickupRequestViewModelDTO
        //    {
        //        RequestId = r.RequestId,
        //        CitizenName = r.User?.FullName ?? "N/A",
        //        Status = r.Status ?? "Pending",
        //        Priority = r.Priority ?? "Normal",
        //        Zone = r.User?.Address ?? "N/A",
        //        PointsEarned = r.FinalPoints.HasValue ? Convert.ToInt32(r.FinalPoints.Value) : 0,
        //        RequestImageUrl = r.RequestImageUrl ?? string.Empty,
        //        CreatedAt = r.RequestDate.GetValueOrDefault(DateTime.Now)
        //    }).ToList();
        //}
        public async Task<IEnumerable<PickupRequestViewModelDTO>> GetRecyclerHistoryAsync(int recyclerId)
        {
            return await _pickupRequestRepository.GetRecyclerHistoryAsync(recyclerId);
        }
        public async Task<IEnumerable<PickupRequest>> GetRequestsByRecyclerIdAsync(int recyclerId)
        {
            return await _pickupRequestRepository.GetRequestsByRecyclerIdAsync(recyclerId);
        }

        public void SaveChanges()
        {
            _pickupRequestRepository.SaveChanges();
        }       
      
    }
}
