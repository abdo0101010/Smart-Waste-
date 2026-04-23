using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IWasteCategoryService
    {
        public void AddWasteCategory(WasteCategory wasteCategory);
        public WasteCategory GetWasteCategoryById(int id);
        public void UpdateWasteCategory(WasteCategory wasteCategory);
        public void DeleteWasteCategory(int id);
        public IEnumerable<WasteCategory> GetAllWasteCategories();
        public void SaveChanges();
    }
}
