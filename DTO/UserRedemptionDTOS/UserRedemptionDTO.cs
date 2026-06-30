namespace SmartWaste.DTO.UserRedemptionDTOS
{
    public class UserRedemptionDTO
    {
        public int RedemptionId { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "Cash-Out" أو "Voucher"
        public decimal Points { get; set; }                          // عدد النقاط المخصومة
        public double AmountEgp { get; set; }                        // القيمة بالجنيه (لو كاش)
        public DateTime TransactionDate { get; set; }                // تاريخ العملية
        public string Status { get; set; } = string.Empty;           // حالة العملية (Pending, Completed)
        public string Details { get; set; } = string.Empty;          // تفاصيل: رقم المحفظة أو كود الكوبون
    }
}