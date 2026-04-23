namespace SmartWaste.DTO.RecuclerDTOS
{
    public class RecyclerWithTotaltripDTO
    {
        public int RecyclerID { get; set; }
        public string FullName { get; set; }
          public int TotalTrips { get; set; }
        public decimal? Rating { get; set; }
    }
}
