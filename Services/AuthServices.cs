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
        public class AuthResult
        {
            public int UserId { get; set; }
            public string Role { get; set; } = null!;
        }
        public AuthResult? AuthenticateUser(UserData data)
        {
            // 1. الأدمن
            var admin = _adminRepository.GetAdminByName(data.Name);
            if (admin != null && admin.Password == data.Password)
            {
                return new AuthResult { UserId = admin.Id, Role = "Admin" };
            }

            // 2. الـ Driver (Recycler)
            var recycler = _recyclerRepository.GetRecyclerByName(data.Name);
            if (recycler != null && BCrypt.Net.BCrypt.Verify(data.Password, recycler.PasswordHash))
            {
                return new AuthResult { UserId = recycler.RecyclerId, Role = "Driver" };
            }

            // 3. الـ HubStaff
            var hubStaff = _hubStaffRepository.GetHubStaffByName(data.Name);
            if (hubStaff != null && BCrypt.Net.BCrypt.Verify(data.Password, hubStaff.PasswordHash))
            {
                return new AuthResult { UserId = hubStaff.StaffId, Role = "HubStaff" };
            }

            // 4. الـ User العادي
            var user = _userRepository.GetUserByName(data.Name);
            if (user != null && BCrypt.Net.BCrypt.Verify(data.Password, user.PasswordHash))
            {
                return new AuthResult { UserId = user.UserId, Role = "User" };
            }

            return null;
        }
    }
}
