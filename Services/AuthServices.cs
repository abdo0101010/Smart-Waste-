using SmartWaste.DTO.AccountDTOS;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class AuthServices: IAuthServices
    {
            private readonly IAdminRepository _adminRepository;
            private readonly IUserRepository _userRepository;
        private readonly IRecyclerRepository _recyclerRepository;
        private readonly IHubStaffRepository _hubStaffRepository;

        public AuthServices(IAdminRepository adminRepository, IUserRepository userRepository , IRecyclerRepository recyclerRepository, IHubStaffRepository hubStaffRepository)
            {
                _adminRepository = adminRepository;
            _userRepository = userRepository;
            _recyclerRepository = recyclerRepository;
            _hubStaffRepository = hubStaffRepository;
        }
        public string? AuthenticateUser(UserData data)
        {
          
            var admin = _adminRepository.GetAdminByName(data.Name);
            if (admin != null && admin.Password == data.Password)
            {
                return "Admin"; 
            }

          
            var recycler = _recyclerRepository.GetRecyclerByName(data.Name);
            if (recycler != null && recycler.PasswordHash == data.Password)
            {
                return "Driver";
            }
            var hubStaff = _hubStaffRepository.GetHubStaffByName(data.Name);
            if (hubStaff != null && hubStaff.PasswordHash == data.Password)
            {
                return "HubStaff";
            }
             var user = _userRepository.GetUserByName(data.Name);
            if (user != null && user.PasswordHash == data.Password)
            {
                return "User";
            }

            return null; 
        }
       
    }
}
