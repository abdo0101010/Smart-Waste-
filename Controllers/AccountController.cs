using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartWaste.DTO.AccountDTOS;
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
        public AccountController(IAuthServices authServices)
        {
            _authServices = authServices;
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
            var role =_authServices.AuthenticateUser(data);
            if (role==null)
            {
                return Unauthorized();

            }
            else
            {

                List<Claim> USerINfo = new List<Claim>();
                string securityKey = "this is my custom Secret key for authentication";
                var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));
                var sgnr = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
                USerINfo.Add(new Claim(ClaimTypes.Name, data.Name));
                USerINfo.Add(new Claim(ClaimTypes.Role, role));

                var jwttoken = new JwtSecurityToken(
                    claims: USerINfo,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: sgnr
                    );
                var token = new JwtSecurityTokenHandler().WriteToken(jwttoken);

                return Ok(new
                {
                    Token = token,
                    Role = role,
                    User = data.Name
                });

            }

        }
    }
}
