using SmartWaste.DTO.RequestItemDTOS;

namespace SmartWaste.DTO.UserDTOS
{
    public class UserFilterAdminDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string IsActive { get; set; }
        public decimal? WalletPoints { get; set; }
       public RequestItemQuantityDTO TotalRecycledQuantity { get; set; } = new RequestItemQuantityDTO();
        public decimal? Quantity { get; set; }
        public int TotalRequests { get; set; }

    }
}
