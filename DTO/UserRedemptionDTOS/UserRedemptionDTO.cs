namespace SmartWaste.DTO.UserRedemptionDTOS
{
    public class UserRedemptionDTO
    {
        public int RedemptionId { get; set; }
        public int UserId { get; set; }
        public int RewardId { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public string? VoucherCode { get; set; }
    }
}
