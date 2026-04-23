using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;
namespace SmartWaste.Repositories
{
    public class HubStaffRepository: IHubStaffRepository
    {
        smartwasteContext _context;
        public HubStaffRepository(smartwasteContext context)
        {
            _context = context;
        }
        public void AddHubStaff(HubStaff hubStaff)
        {
            _context.HubStaffs.Add(hubStaff);
            SaveChanges();
        }
        public HubStaff GetHubStaffById(int id)
        {
            return _context.HubStaffs.Find(id);
        }
        public void UpdateHubStaff(HubStaff hubStaff)
        {
            _context.HubStaffs.Update(hubStaff);
            SaveChanges();
        }
        public void DeleteHubStaff(int id)
        {
            var hubStaff = _context.HubStaffs.Find(id);
            if (hubStaff != null)
            {
                _context.HubStaffs.Remove(hubStaff);
                SaveChanges();
            }
        }
        public IEnumerable<HubStaff> GetAllHubStaff()
        {
            return _context.HubStaffs.ToList();
        }
        public List<HubStaff> GetAllHubStaffWithPickupRequests()
        {
            return _context.HubStaffs.Include(h => h.PickupRequests).ToList();
        }
        public HubStaff GetHubStaffByName(string Name)
        {
            return _context.HubStaffs.FirstOrDefault(h => h.FullName.ToLower() == Name.ToLower());
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}
