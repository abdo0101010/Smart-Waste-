using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.RecuclerDTOS
{
    public class RecyclerUpdateDTO
    {
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; } = null!;

        // 🚛 السواق بنزود عليه الـ VehicleInfo عشان لو غير عربيتة أو وسيلة الالتقاط
        [Required(ErrorMessage = "Vehicle information is required")]
        public string VehicleInfo { get; set; } = null!;
    }
}
