using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class AdminRepository: IAdminRepository
    {
            smartwasteContext _context;
            public AdminRepository(smartwasteContext context)
            {
                _context = context;
             }
        public Admin GetAdminByName(string Name)
        {
            return _context.Admins.FirstOrDefault(a => a.Username.ToLower() == Name.ToLower());
        }
    }
}
