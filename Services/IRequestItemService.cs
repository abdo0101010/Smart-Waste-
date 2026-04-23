using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface IRequestItemService: IRequestItemRepository
    {
        public decimal GetTotalBottelsForAllUsers();
    }
}
