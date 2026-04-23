using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IWasteCategoryRepository
    {
        public void AddWasteCategory(WasteCategory wasteCategory);
        public WasteCategory GetWasteCategoryById(int id);
        public void UpdateWasteCategory(WasteCategory wasteCategory);
        public void DeleteWasteCategory(int id);
        public IEnumerable<WasteCategory> GetAllWasteCategories();
        public void SaveChanges();
    }
}
