using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartWaste.Repositories
{
    public class ForgetPasswordRepository : IForgetPasswordRepository
    {
        private readonly smartwasteContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRecyclerRepository _recyclerRepository;

        public ForgetPasswordRepository(smartwasteContext context, IUserRepository userRepository, IRecyclerRepository recyclerRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _recyclerRepository = recyclerRepository;
        }

        // 1. ميثود حفظ كود الـ OTP بشكل Async
        public async Task SaveOtpCodeAsync(string email, string role, string code)
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(5);

            if (role.Equals("Recycler", StringComparison.OrdinalIgnoreCase))
            {
                var recycler = await _context.Recyclers.FirstOrDefaultAsync(r => r.Email == email);
                if (recycler == null) throw new KeyNotFoundException("هذا البريد الإلكتروني غير مسجل للـ Recycler");

                recycler.VerificationCode = code;
                recycler.VerificationCodeExpiration = expirationTime;

                _context.Recyclers.Update(recycler); // إجبار السيرفر اللايف على التحديث
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) throw new KeyNotFoundException("هذا البريد الإلكتروني غير مسجل للـ User");

                user.VerificationCode = code;
                user.VerificationCodeExpiration = expirationTime;

                _context.Users.Update(user); // إجبار السيرفر اللايف على التحديث
            }

            await _context.SaveChangesAsync();
        }

        // 2. ميثود التحقق وتغيير الباسورد بالـ BCrypt الحاسم 🎯
        public async Task VerifyOTPAndResetPasswordAsync(string email, string code, string newPassword, string confirmPassword, string role)
        {
            if (newPassword != confirmPassword)
                throw new InvalidOperationException("كلمتا المرور غير متطابقتين.");

            string hashedPass = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // 🚀 فتح Transaction صريحة لضمان الحفظ النهائي في الـ SQL Server
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (role.Equals("Recycler", StringComparison.OrdinalIgnoreCase))
                {
                    var recycler = await _context.Recyclers.FirstOrDefaultAsync(r => r.Email == email);
                    if (recycler == null) throw new KeyNotFoundException("الـ Recycler غير موجود.");

                    if (recycler.VerificationCode != code)
                        throw new InvalidOperationException("كود التحقق غير صحيح.");

                    recycler.PasswordHash = hashedPass;
                    recycler.VerificationCode = null;
                    recycler.VerificationCodeExpiration = null;

                    _context.Recyclers.Update(recycler);
                }
                else
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null) throw new KeyNotFoundException("الـ User غير موجود.");

                    if (user.VerificationCode != code)
                        throw new InvalidOperationException("كود التحقق غير صحيح.");

                    user.PasswordHash = hashedPass;
                    user.VerificationCode = null;
                    user.VerificationCodeExpiration = null;

                    _context.Users.Update(user);
                }

                // حفظ التغييرات
                await _context.SaveChangesAsync();

                // 🎯 السطر السحري: إجبار الـ SQL Server على كتابة الداتا الجديدة حالا وقفل الـ Transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // لو حصلت أي مشكلة بنلغي كل حاجة
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}