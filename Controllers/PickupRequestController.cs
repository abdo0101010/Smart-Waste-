using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [SwaggerTag("Endpoints for managing, tracking, and viewing pickup requests dashboard statistics")]
    [ApiController]
    public class PickupRequestsController : ControllerBase
    {
        private readonly IPickupRequestService _pickupRequestService;

        public PickupRequestsController(IPickupRequestService pickupRequestService)
        {
            _pickupRequestService = pickupRequestService;
        }
        [HttpGet("/api/recycler/pickup-requests/summary")]
        [SwaggerOperation(
        Summary = "Gets today's pickup summary for the logged-in recycler",
        Description = "Retrieves stats for Open, In Progress, Completed Today, and SLA Breached requests from the driver's perspective.",
        OperationId = "GetRecyclerPickupSummary",
        Tags = new[] { "Recycler", "Pickup Requests" })]
        [SwaggerResponse(200, Description = "Summary statistics retrieved successfully", Type = typeof(PickupInfoDTOS))]
        [SwaggerResponse(401, Description = "Unauthorized - Recycler token required")]
        public IActionResult GetTodaySummary()
        {
            var summary = _pickupRequestService.GetTodayPickupSummary();
            return Ok(summary);
        }
        [HttpGet("/api/recycler/pickup-requests/search")]
        [SwaggerOperation(
          Summary = "Gets filtered pickup requests for the driver's table",
          Description = "Retrieves requests based on search query (ID, citizen name, address) , status, priority, and zone filters.",
          OperationId = "GetFilteredPickupRequests",
          Tags = new[] { "Recycler", "Pickup Requests" })]
        public IActionResult GetRequestsByFilter([FromQuery] string? search,
                                                 [FromQuery] string?status,
                                                 [FromQuery] string? priority, 
                                                 [FromQuery] string? zone, [FromQuery] string? material)

        {
            var filteredRequests = _pickupRequestService.GetRecyclerRequestsWithFilters(search, status, priority,zone, material);
            return Ok(filteredRequests);
        }
        [HttpPut("/api/recycler/pickup-requests/accept-bulk")]
        [SwaggerOperation(
    Summary = "Accepts multiple pickup requests together to reduce shipping cost",
    Description = "Enforces a minimum limit of requests per route to optimize driver trips.")]
        [SwaggerResponse(200, Description = "Pickup request accepted successfully")]
        [SwaggerResponse(400, Description = "Request could not be accepted (already taken or invalid)")]
        [SwaggerResponse(404, Description = "Pickup request not found")]
        // 🎯 1. تحويل الـ Return Type لـ async Task
        public async Task<IActionResult> AcceptBulkRequests([FromBody] List<int> requestIds, [FromQuery] int recyclerId)
        {
            try
            {
                var result = await _pickupRequestService.AcceptBulkPickupRequestsAsync(requestIds, recyclerId);

                return Ok(new { Message = $"Successfully assigned {requestIds.Count} requests to your current route! Drive safely. 🚛" });
            }
            catch (InvalidOperationException ex)
            {
                // هيرجع 400 لو السواق استهبل واختار طلب أو اتنين بس
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء معالجة خط السير المجمع", Details = ex.Message });
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User ID is missing or invalid in Token.");
            }

            var history = await _pickupRequestService.GetUserHistoryAsync(userId);
            return Ok(history);
        }

        // 2. للأدمن أو موظف المخزن: الـ Parameter هنا أصبح int صريح
        [Authorize(Roles = "Admin,HubStaff")]
        [HttpGet("user-history/{userId:int}")] // وضعنا :int هنا كـ Route Constraint للحماية
        public async Task<IActionResult> GetUserHistoryForAdmin(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("A valid User ID is required.");
            }

            var history = await _pickupRequestService.GetUserHistoryAsync(userId);
            return Ok(history);
        }

        //[Authorize(Roles = "HubStaff,Admin")]
        [HttpGet("GetPendingRequests")]
        [SwaggerOperation(
            Summary = "Fetches all pending pickup requests for hub staff or admim",
            Description = "Retrieves a list of all pending pickup requests that require attention from hub staff or admin.",
            OperationId =" GetPendingPickupRequests",
            Tags = new[] { "HubStaff", "Admin", "Pickup Requests" }
            )]
        [SwaggerResponse(200, Description = "Pending pickup requests retrieved successfully", Type = typeof(IEnumerable<PendingRequestFormDTO>))]
                [SwaggerResponse(204, Description = "No pending pickup requests found")]
        [SwaggerResponse(500, Description = "An error occurred while fetching pending pickup requests")]
        [SwaggerResponse(401, Description = "Unauthorized - HubStaff or Admin token required")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var requests = await _pickupRequestService.GetPendingHubRequestsAsync();

                if (requests == null || !requests.Any())
                {
                    return Ok(new { Message = "لا توجد طلبات معلقة حالياً. 📭" });
                }

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء جلب الطلبات المعلقة", Details = ex.Message });
            }
        }
        [HttpGet("GetRequestsByRecyclerId/{recyclerId:int}")]
        [SwaggerOperation(
            Summary = "Fetches all pickup requests assigned to a specific recycler",
            Description = "Retrieves a list of all pickup requests that have been assigned to the recycler with the given ID.",
            OperationId = "GetRequestsByRecyclerId",
            Tags = new[] { "Recycler", "Pickup Requests" }
        )]
        [SwaggerResponse(200, Description = "Pickup requests retrieved successfully", Type = typeof(IEnumerable<PickupRequestViewModelDTO>))]
        [SwaggerResponse(404, Description = "No pickup requests found for the specified recycler ID")]
        public async Task<IActionResult> GetRequestByRecyclerId(int recyclerId)
        {
            var requests = await _pickupRequestService.GetRequestsByRecyclerIdAsync(recyclerId);
            if (requests == null || !requests.Any())
            {
                return NotFound(new { Message = "No pickup requests found for the specified recycler ID." });
            }
            return Ok(requests);
        }
        [HttpGet("GetRecyclerHistory/{recyclerId:int}")]
        [SwaggerOperation(
            Summary = "Fetches the history of pickup requests for a specific recycler",
            Description = "Retrieves a list of all past pickup requests that have been completed or processed by the recycler with the given ID.",
            OperationId = "GetRecyclerHistory",
            Tags = new[] { "Recycler", "Pickup Requests" }
        )]
        [SwaggerResponse(200, Description = "Recycler history retrieved successfully", Type = typeof(IEnumerable<PickupRequestViewModelDTO>))]
        [SwaggerResponse(404, Description = "No history found for the specified recycler ID")]
        public async Task<IActionResult> GetRecyclerHistory(int recyclerId)
        {
            var history = await _pickupRequestService.GetRecyclerHistoryAsync(recyclerId);
            if (history == null || !history.Any())
            {
                return NotFound(new { Message = "No history found for the specified recycler ID." });
            }
            return Ok(history);
        }

    }
}
