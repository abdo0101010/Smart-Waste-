using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsByRoleAsync(int currentUserId, string userRole);
    }
}
