using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IHubStaffRepository
    {
        public void AddHubStaff(HubStaff hubStaff);
        public HubStaff GetHubStaffById(int id);
        public void UpdateHubStaff(HubStaff hubStaff);
        public void DeleteHubStaff(int id);
        public IEnumerable<HubStaff> GetAllHubStaff();
        public List<HubStaff> GetAllHubStaffWithPickupRequests();
        public HubStaff GetHubStaffByName(string Name);
        public void SaveChanges();
    }
}
