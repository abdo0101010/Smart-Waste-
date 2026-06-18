using Microsoft.AspNetCore.Mvc;
using SmartWaste.Validations;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.Register
{
    public class dataforregister
    {
        
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
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string PasswordHash { get; set; } = null!; 
        [Required]
        [MinLength(30, ErrorMessage = "address must be more than 30 digit ")]
        [MaxLength(100, ErrorMessage = "address must be less than 100 digit")]
        public string Address { get; set; }
        [Required]
        public string Role { get; set; }
        [RegularExpression(@"^01[0125]\d{8}$")]
        public string? Phone { get; set; } = null!;


    }
}
