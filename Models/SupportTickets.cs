using SmartWaste.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartWaste.Models
{
    public class SupportTickets
    {
        [Key]
        public int TicketID { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public TicketPriority Priority { get; set; } = TicketPriority.Low;

    
        [Range(1, 5)]
        public int? Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        [Required]
        public int CitizenID { get; set; }

        [ForeignKey("CitizenID")]
        public virtual User Citizen { get; set; }

        public int? DriverID { get; set; }

        [ForeignKey("DriverID")]
        public virtual Recycler Driver { get; set; }
    }
}
