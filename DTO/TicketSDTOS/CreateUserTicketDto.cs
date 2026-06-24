using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.TicketSDTOS
{
    public class CreateUserTicketDto
    {
       
            [Required(ErrorMessage = "Subject is required.")]
            [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
            public string Subject { get; set; } = null!; // عنوان الشكوى

            [Required(ErrorMessage = "Description is required.")]
            public string Description { get; set; } = null!; // تفاصيل الشكوى

            [Required(ErrorMessage = "Citizen ID is required.")]
            public int CitizenId { get; set; } // ID المواطن اللي باعت الشكوى (إجباري)

            public int? DriverId { get; set; } // ID السواق (اختياري: لو بيشتكي من سواق معين، لو مش ضد سواق بتبعت null)
        
    }
}
