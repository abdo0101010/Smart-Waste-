namespace SmartWaste.DTO.RequestItemDTOS
{
    public class PendingRequestFormDTO
    {
        public int RequestId { get; set; }
        public string OrderNumber => $"ORD-{RequestId}"; // ORD-102
        public string UserName { get; set; }
        public string DriverName { get; set; }  // اسم سائق ديفولت أو مربوط بالـ Cycle
        public string TimeAgo { get; set; } // هيعرض الوقت (مثلاً: "منذ ساعتين" أو التاريخ الصريح)
        public string Status { get; set; }
    }
}
