using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.EcoSnapUploadDTOS;
using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Models;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;


namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    //[Consumes("application/json")]
    [SwaggerTag("Operations related to users")] 
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        private readonly IEcoSnapService _ecoSnapService;
        public UserController(IUserService userService, IEcoSnapService ecoSnapService)
        {
            _userService = userService;
            _ecoSnapService = ecoSnapService;
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
        [HttpPost("/api/User/UpdateUser/{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] updateUser newUser)
        {
            if (newUser == null)
            {
                return BadRequest(new { message = "Invalid user data" });
            }

            try
            {
                _userService.UpdateUser(newUser, id);
                return Ok(new { message = "User updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("UploadEcoSnapImage")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> UploadEcoSnapImage( EcoSnapUploadDTO model)
        {
            // 1. التشيك الأول على الملف
            if (model == null || model.File == null || model.File.Length == 0)
                return BadRequest(new { Message = "برجاء اختيار صورة صالحة." });

            // 2. التشييك الأمني المضمون على الـ User ID
            var user = HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User not authorized or session expired." });
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId == 0)
            {
                return Unauthorized(new { Message = "User ID is invalid or missing from token." });
            }

            try
            {
                // كدة مستحيل يدخل هنا والـ userId بـ 0 أو null
                int detectedBottles = await _ecoSnapService.ProcessImageWithAIAsync(userId, model.File);

                return Ok(new
                {
                    Message = "Image processed and data saved successfully via EcoSnap! 🤖🎉",
                    BottlesDetected = detectedBottles,
                    PointsEarned = detectedBottles * 5
                });
            }
            catch (Exception ex)
            {
                // الـ Catch دي بتحمي البروجكت من إنه يقفل لو الـ AI أو الـ DB رموا أي إيرور
                return StatusCode(500, new { Message = "An error occurred while processing the image", Details = ex.Message });
            }
        }
    }
}
