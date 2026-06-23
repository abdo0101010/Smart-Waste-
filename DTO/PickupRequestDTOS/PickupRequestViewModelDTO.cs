namespace SmartWaste.DTO.PickupRequestDTOS
{
    public class PickupRequestViewModelDTO
    {
        public int RequestId { get; set; }
        public string CitizenName { get; set; } 
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Zone { get; set; }
        public string CategoryName { get; set; } 
        public decimal TotalWeight { get; set; }
    }
}
