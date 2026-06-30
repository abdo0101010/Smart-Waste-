using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.TicketSDTOS;
using SmartWaste.Enums;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class SupportTicketsRepository : ISupportTicketsRepository
    {
        smartwasteContext _context;
        public SupportTicketsRepository(smartwasteContext context)
        {
            _context = context;
        }
        //بتعرض للأدمن كل الشكاوى اللي في السيستم، ويقدر يفلتر بالـ Status أو يبحث بالاسم والعنوان.
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
                CitizenPhone= t.Citizen.Phone,
                DriverName = t.Driver != null ? t.Driver.FullName : "No Driver Assigned",
                Rating = t.Rating
            }).ToList();
        }
        //بتعرض للسائق شكاويه هو بس اللي مربوطة بالـ DriverID بتاعه عشان يتابعها.
        public List<TicketDTO> GetRecyclerSupportTickets(int recyclerId, string status)
        {
            var query = _context.SupportTickets
                        .Include(t => t.Citizen)
                        .AsQueryable();
            query = query.Where(t => t.DriverID == recyclerId);
            if (!string.IsNullOrEmpty(status) && status != "ALL")
            {
                query = query.Where(t => t.Status.ToString() == status);
            }
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
       
        public void CreateTicket(CreateUserTicketDto dto)
        {
            var newTicket = new SupportTickets
            {
                Subject = dto.Subject,
                Description = dto.Description,
                CitizenID = dto.CitizenId, // اليوزر الإجباري صاحب الشكوى
                DriverID = dto.DriverId,   // السواق (null لو الشكوى عامة مش ضد سواق معين)
                Status = TicketStatus.Open, // بتبدأ مفتوحة تلقائياً
                Priority = TicketPriority.Low,
                CreatedAt = DateTime.Now
            };

            _context.SupportTickets.Add(newTicket);
            _context.SaveChanges();
        }
    }
}
