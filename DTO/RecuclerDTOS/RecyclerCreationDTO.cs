using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.RecuclerDTOS
{
    public class RecyclerCreationDTO
    {
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]

        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; } = null!;
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password is too weak!")]

        public string PasswordHash { get; set; } = null!;

    }
 } 
