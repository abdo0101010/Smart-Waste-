using SmartWaste.Validations;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.UserDTOS
{
    public class GetSpecficUser
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "FullName must contain only letters and spaces")]
        public string FullName { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Please enter a valid email address (e.g. user@example.com)")]
        [UniqueEmail]
        public string Email { get; set; } = null!;
        [Range(0.0, double.MaxValue)]
        public decimal? WalletPoints { get; set; }
        public string Phone { get; set; }
    }
}
