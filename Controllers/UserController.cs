using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.UserDTO;
using SmartWaste.Models;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using SmartWaste.DTO.UserDTOS;


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
        [SwaggerResponse(200, "Returns the user with details", typeof(GetSpecficUser))]

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
        [HttpGet("/api/User/SortingUser")]
        [SwaggerOperation(
            Summary = "Sort users by wallet points",
            Description = "Sorts users based on their wallet points in ascending or descending order.",
            OperationId = "SortingUser",
            Tags = new[] { "User" }
        )]
        [SwaggerResponse(200, "Returns the sorted list of users", typeof(List<UserRankDTO>))]
        [SwaggerResponse(400, "Invalid sort order")]
        public IActionResult SortingUser(string sortOrder)
        {
            var sortedUsers = _userService.SortUsersByWalletPoints(sortOrder);
            return Ok(sortedUsers);
        }
        [HttpGet("/api/User/GetRankingUser/{id:int}")]
        [SwaggerOperation(
            Summary = "Get user ranking by ID",
            Description = "Retrieves the ranking of a user based on their wallet points.",
            OperationId = "GetRankingUser",
            Tags = new[] { "User" }
        )]
        [SwaggerResponse(200, "Returns the user ranking", typeof(UserRankDTO))]
        [SwaggerResponse(404, "User not found")]
        public IActionResult GetRankingUser(int id, string sortOrder)
        {
            var userRank = _userService.GetRankingUser(id, sortOrder);
            if (userRank == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(userRank);
        }
        [HttpGet("/api/User/Getavgpoint")]
        [SwaggerOperation(
            Summary = "Get average points of users",
            Description = "Retrieves the average wallet points of all users.",
            OperationId = "GetAvgPointsUsers",
            Tags = new[] { "User" }
        )]

        [SwaggerResponse(200, "Returns the average points of users", typeof(int))]
        [SwaggerResponse(400,"not found")]
        public IActionResult Getavgpoint()
        {
                   return Ok(_userService.GetAvgPointsUsers());
        }

    }
}
