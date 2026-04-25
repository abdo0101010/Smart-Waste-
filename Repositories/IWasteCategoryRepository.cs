using SmartWaste.DTO.WasteCategoryDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IWasteCategoryRepository
    {
        public void AddWasteCategory(WasteCategoryCreationsDTO wasteCategory);
        public WasteCategory GetWasteCategoryById(int id);
        public void UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategory);
        public void DeleteWasteCategory(int id);
        public IEnumerable<WasteCategory> GetAllWasteCategories();
        public void SaveChanges();
    }
}
