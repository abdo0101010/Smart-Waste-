namespace SmartWaste.DTO.HubStaffDTOS
{
    public class HubStaffPickupRequestDto
    {
        public int RequestID { get; set; }
        public int UserID { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Status { get; set; }
        public decimal? FinalPoints { get; set; }
        public int? FinalBottlesCount { get; set; }
        public string VerificationImageUrl { get; set; }
        public int TotalVerifiedRequests { get; set; }

        // إجمالي عدد الزجاجات التي تم التحقق منها عبر هذا الموظف
        public int TotalBottlesVerified { get; set; }

        // لستة تفصيلية بجميع الطلبات التاريخية التي ارتبطت به
        public List<HubStaffPickupRequestDto> VerifiedRequests { get; set; } = new List<HubStaffPickupRequestDto>();
    }
}
