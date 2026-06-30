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

        //[Authorize(Roles = "HubStaff,Admin")] // تأمين الـ Endpoint لليوزر الصح
        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
        [SwaggerResponse(200, "completed verfiy ", typeof(pickupverifyDto))]
        public async Task<IActionResult> VerifyRequestShipment(int transactionId, IFormFile fileAfter)
        {
            int userId = 1;
            try
            {
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
    }
        
        
}