using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.HubStaffDTOS;
using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.DTO.WasteCategoryDTOS;
using SmartWaste.Models;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Policy;

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [SwaggerTag("Endpoints for administrative functions and management of recyclers, users, pickup requests, and support tickets")]
    public class AdminController : ControllerBase
    {
        IPickupRequestService _pickupRequestService;
        IRecyclerService _recyclerService;
        IUserService _userService;
        IRequestItemService _requestingService;
        ISupportTicketsServices _supportTicketsServices;
        IHubStaffService _hubStaffService;
        IWasteCategoryService _wasteCategoryService;

        public AdminController(IPickupRequestService pickupRequestService, IRecyclerService recyclerService, IUserService userService, IRequestItemService requestingService, ISupportTicketsServices supportTicketsServices, IHubStaffService hubStaffService, IWasteCategoryService wasteCategoryService)
        {
            _pickupRequestService = pickupRequestService;
            _recyclerService = recyclerService;
            _userService = userService;
            _requestingService = requestingService;
            _supportTicketsServices = supportTicketsServices;
            _hubStaffService = hubStaffService;
            _wasteCategoryService = wasteCategoryService;

        }


        [HttpGet("/api/admin/total-recyclers")]
        [SwaggerOperation(
Summary = "Gets the total number of recyclers",
Description = "Requires admin privileges",
OperationId = "GetTotalRecyclers",
Tags = new[] { "Admin", "Recyclers  " }

)]
        [SwaggerResponse(200, Description = "Total number of recyclers retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalRecycler()
        {
            var totalRecyclers = _recyclerService.GetTotalRecyclers();
            return Ok(totalRecyclers);
        }

        [HttpGet("/api/admin/total-recycling-active")]
        [SwaggerOperation(
            Summary = "Gets the total number of active recyclers",
            Description = "Requires admin privileges",
            OperationId = "GetTotalRecyclingActive",
            Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Total number of active recyclers retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]

        public IActionResult GetTotalRecyclingActive()
        {
            if (ModelState.IsValid)
            {
                var totalRecyclingActive = _recyclerService.GetTotalRecyclingActive();
                return Ok(totalRecyclingActive);
            }
            return BadRequest(ModelState);
        }
        [HttpGet("/api/admin/total-pickup-requests")]
        [SwaggerOperation(
                Summary = "Gets the total number of pickup requests",
                Description = "Requires admin privileges",
                OperationId = "GetTotalPickupRequests",
    Tags = new[] { "Admin", "Pickup Requests" })]
        [SwaggerResponse(200, Description = "Total number of pickup requests retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalPickupRequest()
        {
            var totalPickupRequest = _pickupRequestService.GetTotalPickupRequests();
            return Ok(totalPickupRequest);

        }
        [HttpGet("/api/admin/Total-Earing")]
        [SwaggerOperation(
                    Summary = "Gets the total earnings from pickup requests",
                    Description = "Requires admin privileges",
            OperationId = "GetTotalEaring",
            Tags = new[] { "Admin", "Earnings" })]
        [SwaggerResponse(200, Description = "Total earnings retrieved successfully", Type = typeof(decimal))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalEaring()
        {
            var totalearing = _pickupRequestService.TotalEaring();
            return Ok(totalearing);
        }
        [HttpGet("/api/admin/recycler-details")]
        [SwaggerOperation(
                        Summary = "Gets detailed information about recyclers",
                        Description = "Requires admin privileges",
                            OperationId = "GetRecyclerDetails",
            Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Recycler details retrieved successfully", Type = typeof(List<ReyclerDetailsAdimDto>))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetRecyclerDetails()
        {
            var recyclerDetails = _recyclerService.GetReyclerDetails();
            return Ok(recyclerDetails);
        }
        [HttpGet("/api/admin/recycler-with-total-trip")]
        [SwaggerOperation(
                            Summary = "Gets recyclers sorted by rating with total trips",
                            Description = "Requires admin privileges",
                            OperationId = "GetRecyclerWithTotalTrip",
            Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Recyclers with total trips retrieved successfully", Type = typeof(List<RecyclerWithTotaltripDTO>))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetRecyclerWithTotalTrip()
        {
            var recyclerWithTotalTrip = _recyclerService.GetSortingRecyclersByRating();
            return Ok(recyclerWithTotalTrip);
        }
        [HttpGet("/api/admin/total-users")]
        [SwaggerOperation(
                                Summary = "Gets the total number of users",
                                Description = "Requires admin privileges",
                                OperationId = "GetTotalUsers",
Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "Total number of users retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalUsers()
        {
            var totalUsers = _userService.GetTotalUsers();
            return Ok(totalUsers);

        }
        [HttpGet("/api/admin/total-active-users")]
        [SwaggerOperation(
                                        Summary = "Gets the total number of active users",
                                        Description = "Requires admin privileges",
                                        OperationId = "GetTotalActiveUsers",

                                            Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "Total number of active users retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]

        public IActionResult GetTotalActiveUsers()
        {
            var totalActiveUsers = _userService.GetTotalActiveUsers();
            return Ok(totalActiveUsers);
        }
        [HttpGet("/api/admin/total-requesting-items")]
        [SwaggerOperation(
                            Summary = "Gets the total number of requesting items",
                                        Description = "Requires admin privileges",
                                        OperationId = "GetTotalRequestingItems",
                                        Tags = new[] { "Admin", "Requests" })]
        [SwaggerResponse(200, Description = "Total number of requesting items retrieved successfully", Type = typeof(int))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalBottels()
        {
            var totalRequestingItems = _requestingService.GetTotalBottelsForAllUsers();
            return Ok(totalRequestingItems);
        }
        [HttpGet("/api/admin/total-wallet-points")]
        [SwaggerOperation(
                                        Summary = "Gets the total wallet points across all users",
                                        Description = "Requires admin privileges",
                                        OperationId = "GetTotalWalletPoints",
                                        Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "Total wallet points retrieved successfully", Type = typeof(decimal))]

        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetTotalWalletPoints()
        {
            var totalWalletPoints = _userService.GetTotalWalletPoints();
            return Ok(totalWalletPoints);
        }
        [HttpGet("/api/admin/users-by-filter")]
        [SwaggerOperation(
                                        Summary = "Gets users based on filter criteria",
                                        Description = "Requires admin privileges",
                                        OperationId = "GetUsersByFilter",
                                        Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "Users retrieved successfully based on filter criteria", Type = typeof(List<UserFilterAdminDTO>))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult GetUsersByFilter(string KeyofFilter, string status)
        {
            var filteredUsers = _userService.GetUsersByFilter(KeyofFilter, status);
            return Ok(filteredUsers);
        }
        [HttpGet("/api/admin/Show-Support-Ticket")]
        [SwaggerOperation(
                                        Summary = "Gets support tickets based on status and search criteria",
                                        Description = "Requires admin privileges",
                                        OperationId = "ShowSupportTicket",
                                        Tags = new[] { "Admin", "Support Tickets" })]
        [SwaggerResponse(200, Description = "Support tickets retrieved successfully", Type = typeof(List<TicketDTO>))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult ShowSupportTicket(string status, string search)
        {
            var supportTickets = _supportTicketsServices.ShowSupportTicket(status, search);
            return Ok(supportTickets);
        }
        [HttpPost("/api/admin/create-user")]
        [SwaggerOperation(
                                        Summary = "Creates a new user",
                                        Description = "Requires admin privileges",
                                        OperationId = "CreateUser",
                                        Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "User created successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateUser([FromForm] UserCreationDTO userCreationDTO)
        {
            try
            {
                await _userService.CreateUser(userCreationDTO);
                return Ok("تم الحفظ بنجاح");
            }
            catch (Exception ex)
            {
                // السطر ده هيخلي الـ API يرجع لك سبب القفلة بالظبط بدل ما يقفل
                return BadRequest(new
                {
                    Error = ex.Message,
                    Inner = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        [HttpPost("/api/admin/create-recycler")]
        [SwaggerOperation(
                                        Summary = "Creates a new recycler",
                                        Description = "Requires admin privileges",
                                        OperationId = "CreateRecycler",
                                        Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Recycler created successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]

        public IActionResult CreateRecycler(RecyclerCreationDTO recyclerCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _recyclerService.CreateRecycler(recyclerCreationDTO);
            return Ok(new { Message = "Recycler created successfully" });
        }
        [HttpPost("/api/admin/create-hub-staff")]
        [SwaggerOperation(
                                        Summary = "Creates a new hub staff member",
                                        Description = "Requires admin privileges",
                                        OperationId = "CreateHubStaff",
                                        Tags = new[] { "Admin", "Hub Staff" })]
        [SwaggerResponse(200, Description = "Hub staff created successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult CreateHubbStaff(HubstaffCreationsDto hubStaff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _hubStaffService.AddHubStaff(hubStaff);

            return Ok(new { Message = "Hub staff created successfully" });
        }
        [HttpPost("/api/admin/create-waste-category")]
        [SwaggerOperation(
                                        Summary = "Creates a new waste category",
                                        Description = "Requires admin privileges",
                                       OperationId = "CreateWasteCategory",
                                        Tags = new[] { "Admin", "Waste Categories" })]
        [SwaggerResponse(200, Description = "Waste category created successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]

        public IActionResult CreateWasteCategory(WasteCategoryCreationsDTO wasteCategoryCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _wasteCategoryService.AddWasteCategory(wasteCategoryCreationDTO);
            return Ok(new { Message = "Waste category created successfully" });
        }
        [HttpPut("/api/admin/update-waste-category")]
        [SwaggerOperation(
                                        Summary = "Updates an existing waste category",
                                        Description = "Requires admin privileges",
            OperationId = "UpdateWasteCategory",
                                        Tags = new[] { "Admin", "Waste Categories" })]
        [SwaggerResponse(200, Description = "Waste category updated successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategoryCreationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _wasteCategoryService.UpdateWasteCategory(wasteCategoryCreationDTO);
            return Ok(new { Message = "Waste category updated successfully" });
        }
        [HttpPut("/api/admin/update-recycler-status")]
        [SwaggerOperation(
                                        Summary = "Updates the status of a recycler",
                                        Description = "Requires admin privileges",
            OperationId = " UpdateRecyclerStatus",
                                        Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Recycler status updated successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult UpdateRecyclerStatus(int recyclerId, string newStatus)
        {
            _recyclerService.UpdateRecyclerStatus(recyclerId, newStatus);
            return Ok(new { Message = "Recycler status updated successfully" });
        }
        [HttpDelete("/api/admin/delete-user")]
        [SwaggerOperation(
                                        Summary = "Deletes a user",
                                        Description = "Requires admin privileges",
            OperationId = "DeleteUser",
                                        Tags = new[] { "Admin", "Users" })]
        [SwaggerResponse(200, Description = "User deleted successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult DeleteUser(int userId)
        {
            _userService.DeleteUser(userId);
            return Ok(new { Message = "User deleted successfully" });
        }
        [HttpDelete("/api/admin/delete-recycler")]
        [SwaggerOperation(
                                    Summary = "Deletes a recycler",
                                    Description = "Requires admin privileges",
        OperationId = "DeleteRecycler",
                                    Tags = new[] { "Admin", "Recyclers" })]
        [SwaggerResponse(200, Description = "Recycler deleted successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult DeleteRecycler(int recyclerId)
        {
            _recyclerService.DeleteRecycler(recyclerId);
            return Ok(new { Message = "Recycler deleted successfully" });
        }
        [HttpDelete("/api/admin/delete-hub-staff")]
        [SwaggerOperation(
                                        Summary = "Deletes a hub staff member",
                                        Description = "Requires admin privileges",
            Tags = new[] { "Admin", "Hub Staff" },
            OperationId = "DeleteHubStaff")]
        [SwaggerResponse(200, Description = "Hub staff deleted successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult DeleteHubStaff(int hubStaffId)
        {
            _hubStaffService.DeleteHubStaff(hubStaffId);
            return Ok(new { Message = "Hub staff deleted successfully" });
        }
        [HttpDelete("/api/admin/delete-waste-category")] 
        [SwaggerOperation(
                                        Summary = "Deletes a waste category",
                                        Description = "Requires admin privileges",
            Tags = new[] {"Admin", "Waste Categories" },    
            OperationId = "DeleteWasteCategory")]
        [SwaggerResponse(200, Description = "Waste category deleted successfully", Type = typeof(object))]
        [SwaggerResponse(401, Description = "Unauthorized access - admin privileges required")]
        public IActionResult DeleteWasteCategory(int wasteCategoryId)
        {
            _wasteCategoryService.DeleteWasteCategory(wasteCategoryId);
            return Ok(new { Message = "Waste category deleted successfully" });
        }
    }
}
