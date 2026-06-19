using Microsoft.EntityFrameworkCore;
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

        public void AddWasteCategory(WasteCategoryCreationsDTO wasteCategory, string? imagePath)
        {
            var newWasteCategory = new WasteCategory
            {
              
                CategoryName = wasteCategory.CategoryName,
                PointsPerUnit = wasteCategory.PointsPerUnit,
                UnitType = wasteCategory.UnitType,
                ImagePath = imagePath

            };
            _context.WasteCategories.Add(newWasteCategory);
            SaveChanges();
        }

        public WasteCategory GetWasteCategoryById(int id)
        {
            return _context.WasteCategories.Find(id);
        }

        public void UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategory , string? imagePath)
        {

            var existingCategory = _context.WasteCategories.Find(wasteCategory.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.CategoryName = wasteCategory.CategoryName;
                existingCategory.PointsPerUnit = wasteCategory.PointsPerUnit;
                existingCategory.UnitType = wasteCategory.UnitType;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    existingCategory.ImagePath = imagePath;
                }
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

        public IEnumerable<WasteCategoryViewModelDTO> GetAllWasteCategories()   
        {
            return _context.WasteCategories.Select(c => new WasteCategoryViewModelDTO
            {
                CategoryName = c.CategoryName,
                PointsPerUnit = c.PointsPerUnit,
                ImagePath = c.ImagePath
            }).ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }


    }
}
