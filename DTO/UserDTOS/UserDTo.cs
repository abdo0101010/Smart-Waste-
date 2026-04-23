using SmartWaste.Models;
using SmartWaste.DTO;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;

namespace SmartWaste.DTO.UserDTO
{
    public class UserDTo
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
        public decimal? WalletPoints { get; set; }
        public  ICollection<PickupRequestDTO>? PickupRequests  = new List<PickupRequestDTO>();
        public  ICollection<UserRedemptionDTO>? UserRedemptions  = new List<UserRedemptionDTO>();
    }
}
