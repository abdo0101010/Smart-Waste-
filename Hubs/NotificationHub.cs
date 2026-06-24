using Microsoft.AspNetCore.SignalR;
namespace SmartWaste.Hubs
{
    public class NotificationHub: Hub
    {
        // الميثود دي مسؤولة عن إرسال الإشعار لجميع الأجهزة المتصلة حالياً بالسيستم
        public async Task SendNotification(string title, string message, string type)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, message, type);
        }
    }
}
