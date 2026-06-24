namespace SmartWaste.Repositories
{
    public interface IForgetPasswordRepository
    {
        Task SaveOtpCodeAsync(string email, string role, string code);
        Task VerifyOTPAndResetPasswordAsync(string email, string code, string newPassword, string confirmPassword, string role);

    }
}
