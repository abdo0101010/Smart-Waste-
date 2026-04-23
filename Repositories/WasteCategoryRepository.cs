using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class WasteCategoryRepository: IWasteCategoryRepository
    {
        smartwasteContext _context;
        public WasteCategoryRepository(smartwasteContext context)
        {
            _context = context;
        }

        public void AddWasteCategory(WasteCategory wasteCategory)
        {
            _context.WasteCategories.Add(wasteCategory);
            SaveChanges();
        }

        public WasteCategory GetWasteCategoryById(int id)
        {
            return _context.WasteCategories.Find(id);
        }

        public void UpdateWasteCategory(WasteCategory wasteCategory)
        {
            _context.WasteCategories.Update(wasteCategory);
            SaveChanges();
        }

        public void DeleteWasteCategory(int id)
        {
            var wasteCategory = _context.WasteCategories.Find(id);
            if (wasteCategory != null)
            {
                _context.WasteCategories.Remove(wasteCategory);
                SaveChanges();
            }
        }

        public IEnumerable<WasteCategory> GetAllWasteCategories()
        {
            return _context.WasteCategories.ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }


    }
}
