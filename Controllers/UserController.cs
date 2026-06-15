using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.UserDTO;
using SmartWaste.Models;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [SwaggerTag("Operations related to users")] 
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        //[HttpGet]
        //public IActionResult GetAllUsers()
        //{
        //    var users = _userService.GetAllUsers()
        //    .Select(u => new {
        //        u.UserId,
        //        u.FullName,
        //        u.Email
        //    }).ToList();
        //    return Ok(users);
        //}
        //[HttpGet("{id:int}")]
        //public IActionResult GetUserById(int id)
        //{
        //    var user = _userService.GetUserById(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(user);
        //}
        //[HttpPost]
        //public IActionResult AddUser(User user)
        //{
        //    _userService.AddUser(user);
        //    return Ok();
        //}
        //[HttpPut]
        //public IActionResult EditUser(User user) {
        //    _userService.UpdateUser(user);
        //    return Ok();
        //}
        //[HttpGet("by-email/{email}")]
        //public IActionResult GetUserByEmail(string email) {
        // var User=   _userService.GetUserByEmail(email);
        //    return Ok(User);
        //}
        //[HttpDelete("{id:int}")]
        //public IActionResult DeleteUserById(int id) { 
        //    _userService.DeleteUser(id);
        //    return Ok();
        //}
        //[HttpGet("/api/User/GetallbyDto")]
        //public IActionResult GetAllUser ()
        //{
        // var users = _userService.GetAllUserDtos();
        //    return Ok(users);
        //}
        
        [HttpGet("/api/User/GetUserByIdWithDetails/{id:int}")]
        [SwaggerOperation(
            Summary = "Get user by ID with details",
            Description = "Retrieves a user by their ID, including related pickup requests and user redemptions.",
            OperationId = "GetUserByIdWithDetails",
            Tags = new[] { "User" }
        )]
        [SwaggerResponse(200, "Returns the user with details", typeof(User))]

        [SwaggerResponse(404, "User not found")]
        

        public IActionResult GetUserById(int id)
        {
            var userDto = _userService.GetUserByIdWithDetails(id);

            if (userDto == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(userDto);

        }

    }
}
