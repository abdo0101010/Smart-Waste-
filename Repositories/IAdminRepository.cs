using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IAdminRepository
    {
        
        public Admin GetAdminByName(string Name);
    }
}
