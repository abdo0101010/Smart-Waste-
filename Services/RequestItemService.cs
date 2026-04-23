using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class RequestItemService: IRequestItemService
    {
        private readonly IRequestItemRepository _requestItemRepository;

        public RequestItemService(IRequestItemRepository requestItemRepository)
        {
            _requestItemRepository = requestItemRepository;
        }

        public decimal GetTotalBottelsForAllUsers()
        {
            return _requestItemRepository.GetTotalBottelsForAllUsers();
        }
    }
}
