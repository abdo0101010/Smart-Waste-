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

        [HttpPost("VerifyRequestShipment")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> VerifyShipment([FromForm] HubStaffVerifyDTO model)
        {
            if (model == null || model.FileAfter == null || model.TransactionId <= 0)
            {
                return BadRequest(new { Message = "برجاء رفع صورة الاستلام وإدخال رقم عملية صحيح." });
            }

            try
            {
                // نمرر الـ ID وصورة الهاب ستاف فقط، والـ Service تجيب الباقي
                int count = await _ecoSnapService.VerifyHubShipmentAsync(0, model.TransactionId, model.FileAfter);

                return Ok(new
                {
                    Message = "تم التحقق من الشحنة ومطابقتها بنجاح عبر الـ AI! ✅🤖",
                    FinalBottlesDetected = count,
                    PointsAwarded = count * 5
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "حدث خطأ أثناء معالجة الصورة وفحص الـ AI", Details = ex.Message });
            }
        }
    }
}