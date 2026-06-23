using SmartWaste.DTO.PickupRequestDTOS;
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
            var requests = _pickupRequestRepository.GetRecyclerRequestsWithFilters(search, status, priority, zone, material);
            var dtoList = requests.Select(p => new PickupRequestViewModelDTO
            {
                RequestId = p.RequestId,
                CitizenName = p.User?.FullName ?? "N/A", // اسم المواطن
                Status = p.Status ?? "Pending",
                Priority = p.Priority ?? "Normal",
                Zone = p.User?.Address ?? "N/A", // العنوان (المنطقة)
                CategoryName = p.RequestItems.FirstOrDefault()?.Category?.CategoryName ?? "N/A",
                TotalWeight = p.RequestItems.Sum(ri => ri.Quantity)
            }).ToList();
            return dtoList;
        }

        public bool AcceptPickupRequest(int requestId, int recyclerId)
        {
            return _pickupRequestRepository.AcceptPickupRequest(requestId, recyclerId);
        }
        public void SaveChanges()
        {
            _pickupRequestRepository.SaveChanges();
        }       
      
    }
}
