using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.DTO.HubStaffDTOS;
using SmartWaste.Services;
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

        public HubStaffController(IEcoSnapService ecoSnapService)
        {
            _ecoSnapService = ecoSnapService;
        }

        //[Authorize(Roles = "HubStaff,Admin")] // تأمين الـ Endpoint لليوزر الصح
        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> VerifyShipment([FromForm] HubStaffVerifyDTO model)
        {
            if (model == null || model.FileAfter == null || model.TransactionId <= 0)
            {
                return BadRequest(new { Message = "برجاء رفع صورة الاستلام وإدخال رقم عملية صحيح." });
            }

            // لقط الـ ID بتاع موظف المخزن الحالي من الـ Token كـ int (للأمان والتوثيق)
            var hubStaffIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(hubStaffIdClaim, out int hubStaffId);

            try
            {
                // باصي الـ hubStaffId الحقيقي بدل الـ 0
                int count = await _ecoSnapService.VerifyHubShipmentAsync(hubStaffId, model.TransactionId, model.FileAfter);

                return Ok(new
                {
                    Message = "تم التحقق من الشحنة ومطابقتها بنجاح عبر الـ AI! ✅🤖",
                    FinalBottlesDetected = count,
                    PointsAwarded = count * 5
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء معالجة الصورة وفحص الـ AI", Details = ex.Message });
            }
        }
    }
}