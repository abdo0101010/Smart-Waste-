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
        public void SaveOtpCode(string email, string role, string code)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(code))
            {
                _forgetPasswordRepository.SaveOtpCode(email, role, code);
            }
        }
        public void VerifyOTPAndResetPassword(string email, string code, string newPassword, string confirmPassword, string role)
        {
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(role))
            {
                _forgetPasswordRepository.VerifyOTPAndResetPassword(email, code, newPassword, confirmPassword, role);
            }
        }

    }
}
