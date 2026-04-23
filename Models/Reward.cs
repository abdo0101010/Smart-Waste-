using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class Reward
{
    [Required]
    public int RewardId { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
    [MaxLength(30, ErrorMessage = "Title cannot exceed 30 characters.")]

    public string Title { get; set; } = null!;
    [Required]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
        [MaxLength(150, ErrorMessage = "Description cannot exceed 150 characters.")]

    public string? Description { get; set; }
        [Required]
        [Range(0, 10000, ErrorMessage = "Cost points must be between 0 and 10000.")]

    public decimal CostPoints { get; set; }
    [Required]


    public int? StockQuantity { get; set; }

    public virtual ICollection<UserRedemption> UserRedemptions { get; set; } = new List<UserRedemption>();
}
