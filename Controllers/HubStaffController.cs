using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartWaste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubStaffController : ControllerBase
    {
        private readonly IHubStaffService _hubStaffService;
        public HubStaffController(IHubStaffService hubStaffService)
        {
            _hubStaffService = hubStaffService;
        }
        //[Authorize(Roles = "HubStaff")] //
        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Verify shipment request using AI",
            Description = "Accepts two images (before and after) and a transaction ID to verify the shipment using AI.",
            OperationId = "VerifyRequestShipment"

        )]
        [SwaggerResponse(200, "Shipment verified successfully")]
        [SwaggerResponse(400, "Invalid request or verification failed")]
        [SwaggerResponse(500, "Internal server error")]

        public async Task<IActionResult> VerifyShipment(IFormFile fileBefore, IFormFile fileAfter, [FromForm] int transactionId)
        {
            if (fileBefore == null || fileAfter == null || transactionId <= 0)
            {
                return BadRequest(new { Message = "برجاء رفع الصورتين وإدخال رقم عملية صحيح." });
            }

            try
            {
                bool isVerified = await _hubStaffService.VerifyShipmentWithAIAsync(fileBefore, fileAfter, transactionId);

                if (isVerified)
                {
                    return Ok(new { Message = "تم التحقق من الشحنة ومطابقتها بنجاح عبر الـ AI! ✅🤖" });
                }
                else
                {
                    return BadRequest(new { Message = "فشل التحقق.. الصورتين غير متطابقتين أو هناك اختلاف في العدد." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء الاتصال بموديل التحقق", Details = ex.Message });
            }
        }
    }
}
