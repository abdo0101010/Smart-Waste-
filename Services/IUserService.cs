using SmartWaste.DTO.UserDTO;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface IUserService: IUserRepository
    {
        public void AddUser(User user);
        public User? GetUserByEmail(string email);
        public User? GetUserById(int userId);
        public void UpdateUser(User user);
        public void DeleteUser(int userId);
        public List<User> GetAllUsers();
        public void SaveChanges();
        public int GetTotalActiveUsers();
        public int GetTotalUsers();
        public List<UserDTo> GetAllUserDtos();

    }
}
