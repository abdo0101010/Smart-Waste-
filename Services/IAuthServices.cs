using SmartWaste.DTO.AccountDTOS;

namespace SmartWaste.Services
{
    public interface IAuthServices
    {
        public string? AuthenticateUser(UserData data);
    }

}
