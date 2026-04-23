using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class RequestItemRepository: IRequestItemRepository
    {
        smartwasteContext _context;
        public RequestItemRepository(smartwasteContext context)
        {
            _context = context;
        }

        public decimal GetTotalBottelsForAllUsers()
        {
            return _context.RequestItems.Sum(r => r.Quantity);
        }
      
    }
}
