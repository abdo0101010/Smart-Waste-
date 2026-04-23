using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IHubStaffService
    {
        public void AddHubStaff(HubStaff hubStaff);
        public HubStaff GetHubStaffById(int id);
        public void UpdateHubStaff(HubStaff hubStaff);
        public void DeleteHubStaff(int id);
        public IEnumerable<HubStaff> GetAllHubStaff();
        public List<HubStaff> GetAllHubStaffWithPickupRequests();
        public void SaveChanges();
    }
}
