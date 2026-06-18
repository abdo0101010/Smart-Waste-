using SmartWaste.DTO.AccountDTOS;
using static SmartWaste.Services.AuthServices;

namespace SmartWaste.Services
{
    public interface IAuthServices
    {
        public AuthResult? AuthenticateUser(UserData data);
    }

}
