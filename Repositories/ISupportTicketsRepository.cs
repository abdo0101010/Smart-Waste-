using SmartWaste.DTO.TicketSDTOS;

namespace SmartWaste.Repositories
{
    public interface ISupportTicketsRepository
    {
        List<TicketDTO> ShowSupportTicket(string status, string search);
        List<TicketDTO> GetRecyclerSupportTickets(int recyclerId, string status);
       public void CreateTicket(CreateUserTicketDto dto);
        public List<TicketDTO> GetAllTickets();

    }
}
