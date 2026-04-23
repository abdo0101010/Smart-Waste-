using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class HubStaff
{
    [Required]

    public int StaffId { get; set; }

    [Required]
    [MinLength(5, ErrorMessage ="full name must be more than 5")]
    [MaxLength(30,ErrorMessage = "full name must be less than 30")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "full name must contain only letters and spaces")]
    public string FullName { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!; 
    public string? Role { get; set; }= "HubStaff";


    public virtual ICollection<PickupRequest>? PickupRequests { get; set; } = new List<PickupRequest>();
}
