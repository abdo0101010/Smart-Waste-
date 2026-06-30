namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class pickupverifyDto
    {

   public  string Status { get; set; }
   public int    FinalBottlesCount { get; set; }
   public   decimal FinalPoints { get; set; }
   public DateTime VerificationDate { get; } = DateTime.UtcNow;
        public string Message { get; set; } // أضف هذا الحقل لتمرير رسالة الخطأ للمستخدم
        public int CountBefore { get; set; }
        public int CountAfter { get; set; }
        public double SimilarityScore { get; set; }
    }
}
