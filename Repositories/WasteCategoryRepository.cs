using SmartWaste.DTO.WasteCategoryDTOS;
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

        public void AddWasteCategory(WasteCategoryCreationsDTO wasteCategory)
        {
            var newWasteCategory = new WasteCategory
            {
              
                CategoryName = wasteCategory.CategoryName,
                PointsPerUnit = wasteCategory.PointsPerUnit,
                UnitType = wasteCategory.UnitType
            };
            _context.WasteCategories.Add(newWasteCategory);
            SaveChanges();
        }

        public WasteCategory GetWasteCategoryById(int id)
        {
            return _context.WasteCategories.Find(id);
        }

        public void UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategory)
        {

            var existingCategory = _context.WasteCategories.Find(wasteCategory.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.CategoryName = wasteCategory.CategoryName;
                existingCategory.PointsPerUnit = wasteCategory.PointsPerUnit;
                existingCategory.UnitType = wasteCategory.UnitType;
                SaveChanges();
            }
            else {
                throw new Exception($"Waste category with ID {wasteCategory.CategoryId} not found.");
            }
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
