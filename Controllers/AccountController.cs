using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartWaste.DTO.AccountDTOS;
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
        public AccountController(IAuthServices authServices, IUserService userService, IRecyclerService recyclerService)
        {
            _authServices = authServices;
            _userService = userService;
            _recyclerService = recyclerService;
        }
        //[HttpPost("Login")]
        //public IActionResult Login(UserData data)
        //{
        //    if (data.Name == "admin" && data.Password == "123")
        //    {
        //        List<Claim> USerINfo = new List<Claim>();
        //        string securityKey = "this is my custom Secret key for authentication";
        //        var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));
        //        var sgnr = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        //        USerINfo.Add(new Claim(ClaimTypes.Name, data.Name));
        //        USerINfo.Add(new Claim(ClaimTypes.Role, data.Role));
        //        USerINfo.Add(new Claim("Password", data.Password));
        //        var jwttoken = new JwtSecurityToken(
        //            claims: USerINfo,
        //            expires: DateTime.Now.AddDays(7),
        //            signingCredentials: sgnr
        //            );
        //        var token = new JwtSecurityTokenHandler().WriteToken(jwttoken);

        //        return Ok(token);
        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }
        //}
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
            // نداء الـ Service وأخذ النتيجة كاملة
            var authResult = _authServices.AuthenticateUser(data);

            if (authResult == null)
            {
                return Unauthorized();
            }

            List<Claim> USerINfo = new List<Claim>();
            string securityKey = "this is my custom Secret key for authentication";
            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));
            var sgnr = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // حقن البيانات جوه الـ Claims الخاصة بالتوكن
            USerINfo.Add(new Claim(ClaimTypes.NameIdentifier, authResult.UserId.ToString())); // الـ ID هنا مهم جداً للـ سيكيورتي
            USerINfo.Add(new Claim(ClaimTypes.Name, data.Name));
            USerINfo.Add(new Claim(ClaimTypes.Role, authResult.Role));

            var jwttoken = new JwtSecurityToken(
                claims: USerINfo,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: sgnr
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwttoken);

            // إرجاع الـ JSON النهائي للـ Frontend
            return Ok(new
            {
                Token = token,
                Role = authResult.Role,
                User = data.Name,
                UserId = authResult.UserId
            });
        }

        
        [HttpPost("Register")]
        [SwaggerOperation(
            Summary = "Register endpoint for user and driver registration",
            Description = "Registers a new user or driver based on the provided role",
            OperationId = "Register",
            Tags = new[] { "Account", "Registration" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a success message upon successful registration", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returns if the registration fails due to invalid role or data")] 
        public IActionResult Register(dataforregister userCreationDTO)
        {
            if (userCreationDTO.Role=="User")
            {
                _userService.RegisterUser(userCreationDTO);
                return Ok("User registered successfully");
            }
            else if (userCreationDTO.Role == "Driver")
            {
                _recyclerService.RegisterRecycler(userCreationDTO);
                return Ok("Driver registered successfully");
            }
            else
            {
                return BadRequest("Invalid role specified. Please use 'User' or 'Driver'.");
            }
        }
    }
}
