namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class PickupInfoDTOS
    {
        public int SlaBreachedCount { get; set; }
        public int CompletedTodayCount { get; set; }
        public int InProgressCount { get; set; }
        public int OpenCount { get; set; }
    }
}
