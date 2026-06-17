namespace SmartWaste.DTO.UserDTOS
{
    public class UserRankDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal WalletPoints { get; set; }
        public int Rank { get; set; }
        public string? BottleCount { get; set; } 

    }
}
