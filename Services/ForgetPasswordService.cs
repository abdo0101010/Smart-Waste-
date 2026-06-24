using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class ForgetPasswordService: IForgetPasswordService
    {
        IForgetPasswordRepository _forgetPasswordRepository;
        public ForgetPasswordService(IForgetPasswordRepository forgetPasswordRepository)
        {
            _forgetPasswordRepository = forgetPasswordRepository;
        }
        public async Task SaveOtpCodeAsync(string email, string role, string code)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(code))
            {
                await _forgetPasswordRepository.SaveOtpCodeAsync(email, role, code);
            }
        }
        public async Task VerifyOTPAndResetPasswordAsync    (string email, string code, string newPassword, string confirmPassword, string role)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(role))
            {
               await _forgetPasswordRepository.VerifyOTPAndResetPasswordAsync(email, code, newPassword, confirmPassword, role);
            }
        }

    }
}
