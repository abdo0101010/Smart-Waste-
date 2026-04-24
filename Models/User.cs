using SmartWaste.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;

namespace SmartWaste.Models;

public partial class User
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
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string PasswordHash { get; set; } = null!; [Required]
    [MinLength(30,ErrorMessage ="address must be more than 30 digit ")]
    [MaxLength(100,ErrorMessage = "address must be less than 100 digit")]
    public string? Address { get; set; }
    [Required]

    public string? Status { get; set; } ="Active";
    [Required]
    [Range(0.0,double.MaxValue)]
    public decimal? WalletPoints { get; set; }
    [Required]
    [DataType(DataType.DateTime)]

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public string Role { get; set; } = "User";
    public virtual ICollection<PickupRequest>? PickupRequests { get; set; } = new List<PickupRequest>();

    public virtual ICollection<UserRedemption> ?UserRedemptions { get; set; } = new List<UserRedemption>();
}


