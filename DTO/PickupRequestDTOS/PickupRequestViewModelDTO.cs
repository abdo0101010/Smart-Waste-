
namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class PickupRequestViewModelDTO
    {
        public int RequestId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Zone { get; set; } = string.Empty;
        public double TotalWeight { get; set; } 
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string RequestImageUrl { get; set; } = string.Empty;
        public string? ArrivalImageUrl { get; set; }
        public int? BottlesCount { get; set; }
        public int PointsEarned { get; set; }
        public string CitizenName { get; set; } = string.Empty;
        public string? DriverName { get; set; }
        public string? HubStaffName { get; set; }
        public string UserName { get; internal set; }
        public string? Address { get; internal set; }
        public DateTime? RequestDate { get; internal set; }
        public DateTime? PickupDate { get; internal set; }
        public List<string> Categories { get; internal set; }
        public bool HasTicket { get; set; }          // هل العميل فتح تيكت دعم؟
        public string? TicketStatus { get; set; }     // حالة التيكت لو موجودة (Open, Closed)
        public bool HasFeedback { get; set; }        // هل العميل قيّم السواق وعمل Feedback؟
        public int? DriverRating { get; set; }
        public int? DriverId { get; set; }
        public string? DriverProfilePictureUrl { get; set; }

    }
}
