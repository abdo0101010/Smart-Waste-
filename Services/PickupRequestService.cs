using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;
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
         public async Task<IEnumerable<UserRedemptionDTO>> GetAllRedeemUserAsync(int userId)
        {
            return await _pickupRequestRepository.GetAllRedeemUserAsync(userId);
        }

        public async Task<IEnumerable<PickupRequestViewModelDTO>> GetUserHistoryAsync(int userId)
        {
            // 1. سحب الطلبات بكامل بياناتها والعلاقات من الـ Repository أولاً لضمان وجود الداتا في الـ Memory
            var requests = await _pickupRequestRepository.GetRequestsByUserIdAsync(userId);

            // تأمين الكوليكشن في الـ Memory لتجنب مشاكل ترجمة الـ LINQ لـ SQL
            var requestsList = requests.ToList();

            // 2. عمل الـ Mapping في الـ Memory (LINQ to Objects) حيث كل الدوال مدعومة 🎯
            return requestsList.Select(r => {
                // فحص مسبق للـ Feedbacks والتيكيتس لتسهيل القراءة والأداء
                var hasFeedback = r.Feedbacks != null && r.Feedbacks.Any();
                var matchingTicket = r.User?.SupportTickets?.FirstOrDefault(t =>
                    t.DriverID == r.RecyclerId &&
                    t.CreatedAt.Date == r.RequestDate.GetValueOrDefault(DateTime.Now).Date);

                return new PickupRequestViewModelDTO
                {
                    RequestId = r.RequestId, // تم التأكيد على الحرف الصغير d حسب الـ Entity
                    CitizenName = r.User?.FullName ?? "N/A",
                    Status = r.Status ?? "Pending",
                    Priority = r.Priority ?? "Normal",
                    Zone = r.User?.Address ?? "N/A",

                    // حل مشكلة تحويل الـ decimal? لـ int صريح
                    PointsEarned = r.FinalPoints.HasValue ? Convert.ToInt32(r.FinalPoints.Value) : 0,
                    RequestImageUrl = r.RequestImageUrl ?? string.Empty,
                    ArrivalImageUrl = r.VerificationImageUrl,
                    BottlesCount = r.FinalBottlesCount,
                    CreatedAt = r.RequestDate.GetValueOrDefault(DateTime.Now),
                    RequestDate = r.RequestDate,
                    PickupDate = r.PickupDate,
                    Address = r.User?.Address,
                    DriverName = r.Recycler?.FullName,
                    HubStaffName = r.HubStaff?.FullName,

                    // 1. التقييم (Feedback)
                    HasFeedback = hasFeedback,
                    DriverRating = hasFeedback ? r.Feedbacks.FirstOrDefault().Rating : null,

                    // 2. تيكت الدعم (Support Ticket) - تعمل بكفاءة الآن في الـ Memory 🚀
                    HasTicket = matchingTicket != null,
                    TicketStatus = matchingTicket != null ? matchingTicket.Status.ToString() : "No Ticket"
                };
            }).ToList();
        }
        
        public async Task<IEnumerable<PendingRequestFormDTO>> GetInProgressHubRequestsAsync()
        {
            return await _pickupRequestRepository.GetInProgressHubRequestsAsync();
        }
        public async Task<IEnumerable<PendingRequestFormDTO>> GetPendingRequestFormsAsync()
        {
            return await _pickupRequestRepository.GetPendingRequestFormsAsync();
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
