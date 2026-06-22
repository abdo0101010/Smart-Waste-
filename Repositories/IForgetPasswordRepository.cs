namespace SmartWaste.Repositories
{
    public interface IForgetPasswordRepository
    {
        public void SaveOtpCode(string email, string role, string code);
        public void VerifyOTPAndResetPassword(string email, string code, string newPassword, string confirmPassword, string role);


    }
}
