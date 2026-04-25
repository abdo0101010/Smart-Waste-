using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SmartWaste.DTO.WasteCategoryDTOS
{
    public class WasteCategoryCreationsDTO
    {
        [Required]
        [Display(Name = "Category ID")]
        [DebuggerHidden]
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
     

        public string? UnitType { get; set; }
    }

}
