using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartWaste.DTO.AccountDTOS;
using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.DTO.Register;
using SmartWaste.Models;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Endpoints for user authentication and account management")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : ControllerBase
    {
        IAuthServices _authServices;
        IUserService _userService;
        IRecyclerService _recyclerService;
        IEmailService _emailService;
        IForgetPasswordService _forgetPasswordService;

        public AccountController(IAuthServices authServices, IUserService userService, IRecyclerService recyclerService, IEmailService emailService, IForgetPasswordService forgetPasswordService)
        {
            _authServices = authServices;
            _userService = userService;
            _recyclerService = recyclerService;
            _emailService = emailService;
            _forgetPasswordService = forgetPasswordService;
        }

        [HttpPost("Login")]
        [SwaggerOperation(
            Summary = "Login endpoint for user authentication",
            Description = "Authenticates a user and returns a JWT token if successful",
            OperationId = "Login",
            Tags = new[] { "Account", "Authentication" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a JWT token and user information upon successful authentication", typeof(object))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returns if the authentication fails due to invalid credentials")]
        public IActionResult Login(UserData data)
        {
            var authResult = _authServices.AuthenticateUser(data);

            if (authResult == null)
            {
                return Unauthorized();
            }

            List<Claim> USerINfo = new List<Claim>();
            string securityKey = "this is my custom Secret key for authentication";
            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));
            var sgnr = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            USerINfo.Add(new Claim(ClaimTypes.NameIdentifier, authResult.UserId.ToString()));
            USerINfo.Add(new Claim(ClaimTypes.Name, data.Name));
            USerINfo.Add(new Claim(ClaimTypes.Role, authResult.Role));

            var jwttoken = new JwtSecurityToken(
                claims: USerINfo,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: sgnr
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwttoken);

            return Ok(new
            {
                Token = token,
                Role = authResult.Role,
                User = data.Name,
                UserId = authResult.UserId
            });
        }

        [HttpPost("Register")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Register endpoint for user and driver registration",
            Description = "Registers a new user or driver based on the provided role",
            OperationId = "Register",
            Tags = new[] { "Account", "Registration" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a success message upon successful registration", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns if the registration fails due to invalid role or data")]
        public async Task<IActionResult> Register([FromForm] dataforregister userCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (userCreationDTO.Role == "User")
            {
                _userService.RegisterUser(userCreationDTO);
                return Ok("User registered successfully");
            }
            else if (userCreationDTO.Role == "Recycler")
            {
                var recyclerDto = new RecyclerCreationDTO
                {
                    FullName = userCreationDTO.FullName,
                    Email = userCreationDTO.Email,
                    Phone = userCreationDTO.Phone,
                    PasswordHash = userCreationDTO.PasswordHash,
                    ProfilePictureUrl = userCreationDTO.ProfilePictureUrl
                };
                await _recyclerService.CreateRecycler(recyclerDto);
                return Ok("Driver registered successfully");
            }
            else
            {
                return BadRequest("Invalid role specified. Please use 'User' or 'Driver'.");
            }
        }

        [HttpPost("SendVerificationCode")]
        [SwaggerOperation(
            Summary = "Send OTP verification code to email",
            Description = "Generates a 6-digit OTP code, saves it to the database, and sends it to the user's or recycler's email.",
            Tags = new[] { "Account", "Password Reset" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Verification code sent successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Email not found for the specified role")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Missing data or exception occurred")]
        public async Task<IActionResult> SendCode(string email, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                return BadRequest(new { message = "البيانات المطلوبة ناقصة." });
            }

            string otpCode = new Random().Next(100000, 999999).ToString();

            try
            {
                // 🚀 الـ await السحرية: السيستم هيقف هنا ويروح يشيك في الداتابيز الأول
                // لو الإيميل غلط أو مش متسجل، الـ Repo هترمي Exception والـ catch هتمسكه حالا!
                await _forgetPasswordService.SaveOtpCodeAsync(email, role, otpCode);

                // كود تصميم الإيميل
                string emailBody = $@"
        <div style='direction: rtl; font-family: sans-serif; text-align: center; border: 1px solid #e0e0e0; padding: 20px; border-radius: 8px;'>
            <h3 style='color: #2e7d32;'>مرحباً بك في Eco Vision</h3>
            <p>طلبك لإعادة تعيين كلمة المرور جاهز. كود التحقق الخاص بك هو:</p>
            <h2 style='color: #2e7d32; letter-spacing: 4px; background: #e8f5e9; padding: 10px; display: inline-block; border-radius: 4px;'><b>{otpCode}</b></h2>
            <p style='color: #757575; font-size: 12px;'>هذا الكود صالح لمدة 5 دقائق فقط.</p>
        </div>";

                // إرسال الإيميل
                await _emailService.SendEmailAsync(email, "إعادة تعيين كلمة المرور - SmartWaste", emailBody);

                return Ok(new { message = "Verification code sent to email successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                // 🎯 أول ما الـ Repo تكتشف إن الإيميل مش موجود للـ Role ده، هتنور هنا وترجع 404 صريحة!
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "حدث خطأ أثناء إرسال الكود.", error = ex.Message });
            }
        }

        [HttpPost("ConfirmPasswordReset")]
        [SwaggerOperation(
            Summary = "Confirm OTP and reset password",
            Description = "Verifies the provided OTP code and updates the password for the user or recycler if valid and not expired.",
            Tags = new[] { "Account", "Password Reset" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Password reset successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid OTP, expired OTP, or mismatched passwords")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User or Recycler not found")]
        public async Task<IActionResult> ConfirmReset(string email, string code, string newPassword, string confirmPassword, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(role))
            {
                return BadRequest(new { message = "جميع الحقول مطلوبة للتأكيد." });
            }

            try
            {
                // 🚀 الـ await السحرية: اجبر السيستم يستنى الحفظ الفعلي في الداتابيز قبل ما يرجع 200!
                await _forgetPasswordService.VerifyOTPAndResetPasswordAsync(email, code, newPassword, confirmPassword, role);

                return Ok(new { message = "تم إعادة تعيين كلمة المرور بنجاح." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "حدث خطأ أثناء تأكيد تعديل كلمة المرور.", error = ex.Message });
            }
        }
    }
}