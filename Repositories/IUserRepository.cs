using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.Register;
using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IUserRepository
    {
        public void AddUser(User user);



        public User? GetUserByEmail(string email);


        public User? GetUserById(int userId);

        public void UpdateUser(User user);


        public void DeleteUser(int userId);

        public List<User> GetAllUsers();
        public User GetUserByName(string name);

        public void SaveChanges();
        public  Task CreateUser(UserCreationDTO userCreationDTO);
        public int GetTotalUsers();
        public int GetTotalActiveUsers();
        public List<UserDTo> GetAllUserDtos();
                public decimal? GetTotalWalletPoints();
        public List<UserFilterAdminDTO> GetUsersByFilter(string KeyofFilter, string status);
        public GetSpecficUser GetUserByIdWithDetails(int userId);
        public List<UserRankDTO> SortUsersByWalletPoints(string sortOrder);
        public UserRankDTO GetRankingUser(int id, string sortOrder);
        public int GetAvgPointsUsers();
        public void RegisterUser(dataforregister userCreationDTO);
        public void UpdateUser(updateUser newUser, int id);
            Task UpdateUserBottlesAndPointsAsync(int userId, int bottleCount, decimal pointsEarned);
        public void ForgetPassword(string? email, string newPassword, string confirmPassword, string role, string? Phone);
        public List<UserDetailsForAdminDTo> GetAllUsersWithDetailsForAdmin();








    }
}
