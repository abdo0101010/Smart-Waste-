using SmartWaste.DTO.WasteCategoryDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IWasteCategoryRepository
    {
        public void AddWasteCategory(WasteCategoryCreationsDTO wasteCategory, string? imagePath);
        public WasteCategory GetWasteCategoryById(int id);
        public void UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategory , string? imagePath);
        public void DeleteWasteCategory(int id);
        public IEnumerable<WasteCategory> GetAllWasteCategories();
        public void SaveChanges();
    }
}
