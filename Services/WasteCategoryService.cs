using SmartWaste.DTO.WasteCategoryDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class WasteCategoryService : IWasteCategoryService
    {
        IWasteCategoryRepository _WasteCategoryRepository;
        public WasteCategoryService(IWasteCategoryRepository wasteCategoryRepository)
        {
            _WasteCategoryRepository = wasteCategoryRepository;
        }
        public void AddWasteCategory(WasteCategoryCreationsDTO wasteCategory)
        {
            if (wasteCategory != null)
            {
                _WasteCategoryRepository.AddWasteCategory(wasteCategory);
            }
        }

        public void DeleteWasteCategory(int id)
        {
            if (id > 0)
            {
                _WasteCategoryRepository.DeleteWasteCategory(id);
            }
        }

        public IEnumerable<WasteCategory> GetAllWasteCategories()
        {
            return _WasteCategoryRepository.GetAllWasteCategories();
        }

        public WasteCategory GetWasteCategoryById(int id)
        {
            if (id > 0)
            {
                return _WasteCategoryRepository.GetWasteCategoryById(id);
            }
            return null;
        }

        public void SaveChanges()
        {
            _WasteCategoryRepository.SaveChanges();
        }

        public void UpdateWasteCategory(WasteCategoryCreationsDTO wasteCategory)
        {
            if (wasteCategory != null)
            {
                _WasteCategoryRepository.UpdateWasteCategory(wasteCategory);
            }

        }
    }
}
