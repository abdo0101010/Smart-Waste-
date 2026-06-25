using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartWaste.Services;
using System.Security.Claims;

namespace SmartWaste.Controllers
{
    [Authorize] // لازم يكون عامل Login عشان يقدر يكلم الـ Controller ده
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        // عمل Inject للـ Service جوة الـ Controller
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            // 1. سحب الـ UserId من الـ Token (الـ Claim المسؤول عن الـ ID)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 2. سحب الـ Role من الـ Token (عشان نعرف هو Admin ولا User ولا Driver)
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // حماية إضافية: لو التوكن مش سليم أو البيانات ناقصة
            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("بيانات المستخدم غير معرفة في الـ Token");
            }

            int currentUserId = int.Parse(userIdClaim);

            // 3. نداء الـ Service وهي اللي هتتولى الفلترة بناءً على الـ Role
            var notifications = await _notificationService.GetNotificationsByRoleAsync(currentUserId, userRole);

            // 4. إرجاع البيانات للفرونت إند بـ Status Code 200 OK
            return Ok(notifications);
        }
    }
}