using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class HubStaffService : IHubStaffService
    {
        IHubStaffRepository _hubStaffRepository;
        public HubStaffService(IHubStaffRepository hubStaffRepository)
        {
            _hubStaffRepository = hubStaffRepository;
        }
        public void AddHubStaff(HubStaff hubStaff)
        {
            if (hubStaff != null)
            {
                _hubStaffRepository.AddHubStaff(hubStaff);
               

            }
            
        }

        public void DeleteHubStaff(int id)
        {
            if(id > 0)
            {
                _hubStaffRepository.DeleteHubStaff(id);
            }
        }

        public IEnumerable<HubStaff> GetAllHubStaff()
        {
            return _hubStaffRepository.GetAllHubStaff();
        }

        public List<HubStaff> GetAllHubStaffWithPickupRequests()
        {
            return _hubStaffRepository.GetAllHubStaffWithPickupRequests();
        }

        public HubStaff GetHubStaffById(int id)
        {
            if(id > 0)
            {
                return _hubStaffRepository.GetHubStaffById(id);
            }
            return null;
        }

        public void SaveChanges()
        {
            _hubStaffRepository.SaveChanges();
        }

        public void UpdateHubStaff(HubStaff hubStaff)
        {
            if(hubStaff != null)
            {
                _hubStaffRepository.UpdateHubStaff(hubStaff);
            }
        }
    }
}
