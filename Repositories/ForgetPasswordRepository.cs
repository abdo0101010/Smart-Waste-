namespace SmartWaste.Repositories
{
    public class ForgetPasswordRepository: IForgetPasswordRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecyclerRepository _recyclerRepository;
       
        public ForgetPasswordRepository(IUserRepository userRepository, IRecyclerRepository recyclerRepository)
        {
            _userRepository = userRepository;
            _recyclerRepository = recyclerRepository;
        }
        public void SaveOtpCode(string email, string role, string code)
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(5);

            if (role.Equals("Recycler", StringComparison.OrdinalIgnoreCase))
            {
                var recycler = _recyclerRepository.GetRecyclerByEmail(email);
                if (recycler == null) throw new KeyNotFoundException("هذا البريد الإلكتروني غير مسجل للـ Recycler");

                recycler.VerificationCode = code;
                recycler.VerificationCodeExpiration = expirationTime;
                _recyclerRepository.UpdateRecycler(recycler);
                _recyclerRepository.SaveChanges();
            }
            else
            {
                var user = _userRepository.GetUserByEmail(email);
                if (user == null) throw new KeyNotFoundException("هذا البريد الإلكتروني غير مسجل للـ User");

                user.VerificationCode = code;
                user.VerificationCodeExpiration = expirationTime;
                _userRepository.UpdateUser(user);
          _userRepository.SaveChanges();
            }
        }

        public void VerifyOTPAndResetPassword(string email, string code, string newPassword, string confirmPassword, string role)
        {
            if (newPassword != confirmPassword)
                throw new InvalidOperationException("كلمتا المرور غير متطابقتين.");

            string hashedPass = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (role.Equals("Recycler", StringComparison.OrdinalIgnoreCase))
            {
                var recycler = _recyclerRepository.GetRecyclerByEmail(email);
                if (recycler == null) throw new KeyNotFoundException("الـ Recycler غير موجود.");

                if (recycler.VerificationCode != code || recycler.VerificationCodeExpiration < DateTime.UtcNow)
                    throw new InvalidOperationException("كود التحقق غير صحيح أو انتهت صلاحيته.");

                recycler.PasswordHash = hashedPass;
                recycler.VerificationCode = null;
                recycler.VerificationCodeExpiration = null;
                _recyclerRepository.UpdateRecycler(recycler);
                _recyclerRepository.SaveChanges();
            }
            else
            {
                var user = _userRepository.GetUserByEmail(email);
                if (user == null) throw new KeyNotFoundException("الـ User غير موجود.");

                if (user.VerificationCode != code || user.VerificationCodeExpiration < DateTime.UtcNow)
                    throw new InvalidOperationException("كود التحقق غير صحيح أو انتهت صلاحيته.");

                user.PasswordHash = hashedPass;
                user.VerificationCode = null;
                user.VerificationCodeExpiration = null;
                _userRepository.UpdateUser(user);
                _userRepository.SaveChanges();
            }
        }
    }
}
