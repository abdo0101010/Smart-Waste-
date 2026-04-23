namespace SmartWaste.DTO.TicketSDTOS
{
    public class TicketDTO
    {
        public int TicketID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CitizenName { get; set; }
        public string DriverName { get; set; }
        public int? Rating { get; set; }
    }
}
