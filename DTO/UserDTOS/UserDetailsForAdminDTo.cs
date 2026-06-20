using SmartWaste.Validations;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.UserDTOS
{
    public class UserDetailsForAdminDTo
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "FullName must contain only letters and spaces")]
        public string FullName { get; set; } = null!;
        [RegularExpression(@"^01[0125]\d{8}$")]
        public string? Phone { get; set; } = null!;
    }
}
