using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.HubStaffDTOS;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.Models;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubStaffController : ControllerBase
    {
        private readonly IEcoSnapService _ecoSnapService;
        private readonly IHubStaffService _hubStaffService;

        public HubStaffController(IEcoSnapService ecoSnapService, IHubStaffService hubStaffService)
        {
            _ecoSnapService = ecoSnapService;
            _hubStaffService = hubStaffService;
        }

        [Authorize(Roles = "HubStaff,Admin")] // تأمين الـ Endpoint لليوزر الصح
        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
        [SwaggerResponse(200, "completed verify", typeof(pickupverifyDto))]
        public async Task<IActionResult> VerifyRequestShipment(int transactionId, IFormFile fileAfter)
        {
            try
            {
                // 🎯 السطر السحري: سحب الـ ID الحقيقي لموظف الهاب ستاف لايف من الـ JWT Claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "فشلت قراءة هوية الموظف من التوكن النشط." });
                }

                int userId = int.Parse(userIdClaim);

                // تمرير الـ ID الحقيقي واللايف للسيرفيس
                var result = await _ecoSnapService.VerifyHubShipmentAsync(userId, transactionId, fileAfter);

                if (result == null) return BadRequest(new { message = "فشلت العملية" });

                // إذا كانت الحالة FAILED، نرجعها للفرونت بكود BadRequest (400) ومعه تفاصيل الـ Mismatch كاملة
                if (result.Status == "FAILED")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get Hub Staff by ID",
            Description = "Retrieve a Hub Staff member by their ID.",
            OperationId = "GetHubStaffById",
            Tags = new[] { "HubStaff" }
            )]
        [SwaggerResponse(200, "Successfully retrieved Hub Staff", typeof(HubStaff))]
        [SwaggerResponse(404, "Hub Staff not found")]

        public IActionResult GetHubStaffById(int id)
        {
            var hubStaff = _hubStaffService.GetHubStaffById(id);
            if (hubStaff == null)
            {
                return NotFound(new { Message = "Not Found Hub Staff" });
            }
            return Ok(hubStaff);
        }
        [HttpGet("{id}/history")]
        public IActionResult GetHistory([FromRoute] int id)
        {
            var history = _hubStaffService.GetHubStaffHistory(id);

            if (history == null)
            {
                return NotFound(new { message = "History not found or invalid HubStaff ID." });
            }

            return Ok(history);
        }
        [HttpGet("allHubStaff")]
        [SwaggerOperation(
            Summary = "Get All Hub Staff",
            Description = "Retrieve a list of all Hub Staff members.",
            OperationId = "GetAllHubStaff",
            Tags = new[] { "HubStaff" }
        )]
        [SwaggerResponse(200, "Successfully retrieved list of Hub Staff", typeof(IEnumerable<ListHubStaffDTO>))]
        [SwaggerResponse(404, "No Hub Staff found")]
        public IActionResult GetAllHubStaff()
        {
            var hubStaffList = _hubStaffService.GetAllHubStaff();
            if (hubStaffList == null || !hubStaffList.Any())
            {
                return NotFound(new { message = "No Hub Staff found" });
            }
            return Ok(hubStaffList);
        }
    }
        
        
}