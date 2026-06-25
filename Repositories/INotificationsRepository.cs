using SmartWaste.Models;
using System.Linq.Expressions;

namespace SmartWaste.Repositories
{
    public interface INotificationsRepository
    {
        // ميثود تجيب كل الإشعارات مع بيانات الـ User (للأدمن)
        Task<IEnumerable<Notification>> GetAllWithUsersAsync();

        // ميثود تجيب الإشعارات بناءً على شرط معين (لليوزر والدرايفر)
        Task<IEnumerable<Notification>> GetByConditionAsync(Expression<Func<Notification, bool>> expression);
    }
}
