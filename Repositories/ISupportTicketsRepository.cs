using SmartWaste.DTO.TicketSDTOS;

namespace SmartWaste.Repositories
{
    public interface ISupportTicketsRepository
    {
        List<TicketDTO> ShowSupportTicket(string status, string search);
    }
}
