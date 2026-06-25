using Microsoft.AspNetCore.SignalR;

namespace SmartWaste.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } // مثل: "New pickup request" أو "Sensor offline"
        public string Message { get; set; } // مثل: "Citizen scheduled a glass pickup..."
        public string Type { get; set; } // "Pickup" أو "Sensor" عشان تحديد الأيقونة في الـ Front-end
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; }
        public bool IsRead { get; set; } = false;
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public int? RecyclerId { get; set; }
        public virtual Recycler Recycler { get; set; }
    }
}
