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

        public PickupRequest GetPickupRequestById(int id)
        {
            if(id > 0)
            {
                return _pickupRequestRepository.GetPickupRequestById(id);
            }
            return null;
        }

        public void SaveChanges()
        {
            _pickupRequestRepository.SaveChanges();
        }

        public void UpdatePickupRequest(PickupRequest pickupRequest)
        {
            if(pickupRequest != null)
            {
                _pickupRequestRepository.UpdatePickupRequest(pickupRequest);
            }
        }
        public int GetTotalPickupRequests()
        {
            return _pickupRequestRepository.GetTotalPickupRequests();
        }
        public decimal? TotalEaring()
        {
            return _pickupRequestRepository.TotalEaring();



        }
    }
}
