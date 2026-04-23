using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class Recycler
{
    [Required]
    public int RecyclerId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "full name must be more than 5")]
    [MaxLength(50, ErrorMessage = "full name cannot exceed 50 characters")]
    public string FullName { get; set; } = null!;
    [RegularExpression(@"^01[0125]\d{8}$")]
    public string Phone { get; set; } = null!;
    public string PasswordHash { get; set; } = null!; // خليناها كابيتال
    public string? VehicleInfo { get; set; }

    public string? Status { get; set; }

    [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
    public decimal? Rating { get; set; }
    public string Role { get; set; } = "Recycler";  

    public virtual ICollection<PickupRequest> PickupRequests { get; set; } = new List<PickupRequest>();
}
