using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.UserDTOS
{
    public class UserCreationDTO
    {
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "FullName must contain only letters and spaces")]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
         ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "Password is too weak!")]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(30, ErrorMessage = "address must be more than 30 digit")]
        public string Address { get; set; } = null!;
        
        public IFormFile ?ProfilePictureUrl { get; set; }
    }
}

