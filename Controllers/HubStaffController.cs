using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.Services;

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
        //[Authorize(Roles = "HubStaff")] // تأمين الـ Endpoint لموظفين الهاب بس
        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
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
