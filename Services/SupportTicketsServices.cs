using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class SupportTicketsServices : ISupportTicketsServices
    {
        private readonly ISupportTicketsRepository _repository;

        public SupportTicketsServices(ISupportTicketsRepository repository)
        {
            _repository = repository;
        }

        public List<TicketDTO> ShowSupportTicket(string status, string search)
        {
            return _repository.ShowSupportTicket(status, search);
        }
    }
}
