using System.ComponentModel.DataAnnotations;

namespace SmartWaste.DTO.HubStaffDTOS
{
    public class HubstaffCreationsDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
        [MaxLength(30, ErrorMessage = "full name must be less than 30")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "full name must contain only letters and spaces")]
        public string FullName { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercas")]


        public string PasswordHash { get; set; } = null!;

    }
}
