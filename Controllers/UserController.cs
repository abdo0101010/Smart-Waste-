using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.UserDTO;
using SmartWaste.Models;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;
using SmartWaste.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers()
            .Select(u => new {
                u.UserId,
                u.FullName,
                u.Email
            }).ToList();
            return Ok(users);
        }
        [HttpGet("{id:int}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            _userService.AddUser(user);
            return Ok();
        }
        [HttpPut]
        public IActionResult EditUser(User user) {
            _userService.UpdateUser(user);
            return Ok();
        }
        [HttpGet("by-email/{email}")]
        public IActionResult GetUserByEmail(string email) {
         var User=   _userService.GetUserByEmail(email);
            return Ok(User);
        }
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUserById(int id) { 
            _userService.DeleteUser(id);
            return Ok();
        }
        [HttpGet("/api/User/GetallbyDto")]
        public IActionResult GetAllUser ()
        {
         var users = _userService.GetAllUserDtos();
            return Ok(users);
        }



        }
}
