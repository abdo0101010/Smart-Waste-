namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class PickupRequestDTO
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int? RecyclerId { get; set; }
        public int? HubStaffId { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal? EstimatedPoints { get; set; }
        public decimal? FinalPoints { get; set; }
    }
}
