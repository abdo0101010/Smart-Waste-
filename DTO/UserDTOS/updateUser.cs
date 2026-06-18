using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.UserDTOS
{
    public class updateUser
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "FullName must contain only letters and spaces")]
        public string FullName { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string PasswordHash { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [MinLength(30, ErrorMessage = "address must be more than 30 digit ")]
        [MaxLength(100, ErrorMessage = "address must be less than 100 digit")]
        public string Address { get; set; }
    }
}
