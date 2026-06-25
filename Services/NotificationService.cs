using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationsRepository _notificationRepository;

        public NotificationService(INotificationsRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByRoleAsync(int currentUserId, string userRole)
        {
            IEnumerable<Notification> notificationsData;

            // 1. جلب البيانات بناءً على الـ Role من خلال الـ Repository
            if (userRole == "Admin")
            {
                notificationsData = await _notificationRepository.GetAllWithUsersAsync();
            }
            else if (userRole == "Recycler")
            {
                // 🔑 فلترة صريحة للريسيكلر بناءً على الـ RecyclerId
                notificationsData = await _notificationRepository.GetByConditionAsync(n => n.RecyclerId == currentUserId);
            }
            else
            {
                notificationsData = await _notificationRepository.GetByConditionAsync(n => n.UserId == currentUserId);
            }

            // 2. إذا كان الـ Repository بيعمل Include للـ User والـ Repository بيرجع Notification Entity
            // بنعمل Mapping للتأكيد، أو تقدر تعمل return للـ notificationsData علطول لو الـ Repository ظابطها.
            var notificationList = notificationsData.Select(n => new Notification
            {
                Id = n.Id,
                UserId = n.UserId,
                UserName = n.UserName ?? "System", // بيقرأ الـ UserName اللي متخزن في الـ Entity أو بيكتب System
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList(); // تحويلها لـ List لتجنب الـ Deferred Execution مشاكل الكويري

            return notificationList;
        }
    }
}