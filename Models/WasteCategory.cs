using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartWaste.Models;

public partial class WasteCategory
{
    [Required]
    public int CategoryId { get; set; }
    [Required]
    [Display(Name = "Category Name")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    public string CategoryName { get; set; } = null!;
    [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Points per unit must be greater than or equal to 0.")]
        [Display(Name = "Points Per Unit")]

    public decimal PointsPerUnit { get; set; }

    [Required]
        [Display(Name = "Unit Type")]
        [StringLength(50, ErrorMessage = "Unit type cannot exceed 50 characters.")]

    public string? UnitType { get; set; }

    public virtual ICollection<RequestItem> RequestItems { get; set; } = new List<RequestItem>();
}
