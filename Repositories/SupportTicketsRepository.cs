using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class SupportTicketsRepository: ISupportTicketsRepository
    {
            smartwasteContext _context;
        public SupportTicketsRepository(smartwasteContext context)
        {
            _context = context;
        }

        public List<TicketDTO> ShowSupportTicket(string status, string search)
        {
            var query = _context.SupportTickets
         .Include(t => t.Citizen)
         .Include(t => t.Driver)
         .AsQueryable();
            if (!string.IsNullOrEmpty(status) && status != "ALL")
                query = query.Where(t => t.Status.ToString() == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Subject.Contains(search) || t.Citizen.FullName.Contains(search));

            return query.Select(t => new TicketDTO 
            {
                TicketID = t.TicketID,
                Subject = t.Subject,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                CitizenName = t.Citizen.FullName,
                DriverName = t.Driver != null ? t.Driver.FullName : "No Driver Assigned",
                Rating = t.Rating
            }).ToList();
        }

    }
}
