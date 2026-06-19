namespace SmartWaste.DTO.WasteCategoryDTOS
{
    public class WasteCategoryViewModelDTO
    {
            public string CategoryName { get; set; } = string.Empty;
            public decimal PointsPerUnit { get; set; }
            public string? ImagePath { get; set; } 
    }
}
