using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartWaste.DTO.EcoSnapUploadDTOS;
using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Hubs;
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
        ISupportTicketsServices _supportTicketsServices;
        IHubContext<ChatHub> hubContext;
        public UserController(IUserService userService, IEcoSnapService ecoSnapService,ISupportTicketsServices supportTicketsServices,IHubContext<ChatHub> hubContext)
        {
            _userService = userService;
            _ecoSnapService = ecoSnapService;
            _supportTicketsServices = supportTicketsServices;
            this.hubContext = hubContext;
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
        [HttpPut("/api/User/UpdateUser/{id:int}")]
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
        [HttpPost("/api/User/ForgetPassWord")]
        [SwaggerOperation(
            Summary = "Reset user password",
            Description = "Allows users and Driver to reset their password by providing their email, new password, confirm password, role, and phone number.",
            OperationId = "ForgetPassWord",
            Tags = new[] { "User" ,"Driver"}
            )]
        [SwaggerResponse(200, "Password reset successfully")]
        [SwaggerResponse(404, "User not found")]
        public IActionResult ForgetPassWord(string ?email, string newPassword, string confirmPassword, string role, string? Phone)
        {
            try
            {
                _userService.ForgetPassword(email, newPassword, confirmPassword, role, Phone);
                return Ok(new { message = "Password reset successfully" });
            }
            catch
            {
                                return NotFound(new { message = "User not found or invalid input" });
            }


        }
        [Authorize]
        [HttpPost("UploadEcoSnapImage")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadEcoSnapImage([FromForm] EcoSnapUploadDTO model)
        {
            if (model == null || model.File == null || model.File.Length == 0)
                return BadRequest(new { Message = "برجاء اختيار صورة صالحة." });

            var user = HttpContext.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Message = "User ID غير صالح أو غير موجود بالتوكن." });
            }

            try
            {
                // الـ Service هتكريت الطلب وترجع الـ ID الفريد
                int transactionId = await _ecoSnapService.ProcessUserUploadAsync(userId, model.File);

                return Ok(new
                {
                    Message = "تم إنشاء طلبك وحفظ الصورة في الـ Waiting Room بنجاح! 🎉",
                    TransactionId = transactionId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء معالجة الصورة", Details = ex.Message });
            }
        }
        [HttpGet("get userby email")]
        public IActionResult GetUserByEmail(string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }



        [HttpPost("/api/user/tickets/create")]
        [SwaggerOperation(Summary = "Citizen submits a new support ticket", Tags = new[] { "User Tickets" })]
        [SwaggerResponse(200, "Ticket submitted successfully to Admin!")]
        [SwaggerResponse(400, "Invalid ticket data")]

        public IActionResult CreateTicketFromUser([FromBody] CreateUserTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _supportTicketsServices.CreateTicket(dto);
            return Ok(new { message = "Ticket submitted successfully to Admin!" });
        }
        [HttpPost("rate-driver")]
        [SwaggerOperation(Summary = "Rate the driver/recycler for a specific pickup request", Description = "Allows users to add feedback for a specific pickup request by providing the request ID, rating, and comment.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Feedback added successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid feedback data or request not assigned to a driver.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized - User must be logged in.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Pickup request not found.")]
        public async Task<IActionResult> RateDriver([FromQuery] int requestId, [FromQuery] int rating, [FromQuery] string comment)
        {
            try
            {
                // نداء الـ Service
                await _userService.feedbackRating(requestId, rating, comment);

                return Ok(new { message = "تم تسجيل تقييمك للدرايفر بنجاح. شكراً لك!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = $"خطأ داخلي بالسيرفر: {ex.Message}" });
            }
        }

        [HttpPut("/api/User/UpdateProfilePicture/{id:int}")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Upload or update citizen profile picture", Tags = new[] { "User" })]
        [SwaggerResponse(200, "Profile picture updated successfully")]
        [SwaggerResponse(400, "Invalid image file")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> UpdateProfilePicture([FromRoute] int id, [FromForm] UserPhotoUploadDTO model)
        {
            if (model == null || model.ProfilePicture == null || model.ProfilePicture.Length == 0)
                return BadRequest(new { message = "برجاء اختيار صورة صالحة." });

            try
            {
                var newPath = await _userService.UpdateUserProfilePictureAsync(id, model.ProfilePicture);

                if (string.IsNullOrEmpty(newPath))
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "تم تحديث الصورة الشخصية بنجاح! 🎉", profilePictureUrl = newPath });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء رفع الصورة", details = ex.Message });
            }
        }
     

    }
}
