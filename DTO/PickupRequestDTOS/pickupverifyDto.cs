namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class pickupverifyDto
    {

   public  string Status { get; set; }
   public int    FinalBottlesCount { get; set; }
   public   decimal FinalPoints { get; set; }
   public DateTime VerificationDate { get; } = DateTime.UtcNow;
    }
}
