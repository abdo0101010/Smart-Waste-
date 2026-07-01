using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.HubStaffDTOS;
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
        public void AddHubStaff(HubstaffCreationsDto hubStaff)
        {
                HubStaff newHubStaff = new HubStaff
                {
                    FullName = hubStaff.FullName,
                    PasswordHash = hubStaff.PasswordHash

                };
            _context.HubStaffs.Add(newHubStaff);
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
        public IEnumerable<ListHubStaffDTO> GetAllHubStaff()
        {
                var hubStaffs = _context.HubStaffs.Select(h => new ListHubStaffDTO
                {
                    HubStaffId = h.StaffId,
                    Name = h.FullName
                }).ToList();

            return hubStaffs;
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
        public void CreateHubStaff(HubstaffCreationsDto hubStaff)
        {
            HubStaff newHubStaff = new HubStaff
            {
                FullName = hubStaff.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(hubStaff.PasswordHash)    
                
            };
            _context.HubStaffs.Add(newHubStaff);
            SaveChanges();
        }
        public List<PickupRequest> GetHistoryofHubstaff(int id)
        {
            // هنا بنجيب كل الطلبات اللي ارتبطت بموظف الفرز ده وتأكدت حالتها بنجاح
            return _context.PickupRequests
                .Where(p => p.HubStaffId == id && p.Status == "Verified")
                .OrderByDescending(p => p.VerificationDate) // ترتيب من الأحدث للأقدم
                .ToList();
        }





    }
}
