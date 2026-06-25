using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;
using System.Linq.Expressions;

namespace SmartWaste.Repositories
{
    public class NotificationsRepository : INotificationsRepository // 👈 متنساش تورث الـ Interface هنا
    {
        private readonly smartwasteContext _context;

        public NotificationsRepository(smartwasteContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllWithUsersAsync()
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Recycler) // 👈 ضيفنا Include لجدول الريسيكلر
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByConditionAsync(Expression<Func<Notification, bool>> expression)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Recycler)
                // 🔑 تعديل: بنعمل Include للـ Object مش الـ ID
                .Where(expression)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}