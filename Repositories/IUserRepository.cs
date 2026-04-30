using Microsoft.EntityFrameworkCore;
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


    }
}
