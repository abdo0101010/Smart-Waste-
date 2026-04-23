namespace SmartWaste.DTO.RecuclerDTOS
{
    public class ReyclerDetailsAdimDto
    {
        public int RecyclerID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string VehicleInfo { get; set; }
        public string Status { get; set; }
        public decimal? Rating { get; set; }
        public int TotalTripsCompleted { get; set; }
    }
}
