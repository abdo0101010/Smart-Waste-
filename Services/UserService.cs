using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class UserService : IUserService
    {
        IUserRepository _UserRepository;
        public UserService(IUserRepository userRepository)
        {
            _UserRepository = userRepository;
        }
        public void AddUser(User user)
        {
            if (user != null)
            {
                _UserRepository.AddUser(user);
            }
        }
        public void DeleteUser(int userId)
        {
            if (userId > 0)
            {
                _UserRepository.DeleteUser(userId);
            }
        }
        public List<User> GetAllUsers()
        {
            return _UserRepository.GetAllUsers();
        }
        public User? GetUserByEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return _UserRepository.GetUserByEmail(email);
            }
            return null;
        }
        public User? GetUserById(int userId)
        {
            if (userId > 0)
            {
                return _UserRepository.GetUserById(userId);
            }
            return null;
        }
        public void SaveChanges()
        {
            _UserRepository.SaveChanges();
        }
        public void UpdateUser(User user)
        {
            if (user != null)
            {
                _UserRepository.UpdateUser(user);
            }
        }
        public List<UserDTo> GetAllUserDtos()
        {
          return  _UserRepository.GetAllUserDtos();
        }
        public int GetTotalUsers()
        {
            return _UserRepository.GetTotalUsers();
        }
        public int GetTotalActiveUsers()
        {
            return _UserRepository.GetTotalActiveUsers();
        }
            public decimal? GetTotalWalletPoints()
            {
                return _UserRepository.GetTotalWalletPoints();
        }
            public List<UserFilterAdminDTO> GetUsersByFilter(string KeyofFilter, string status)
            {
                return _UserRepository.GetUsersByFilter(KeyofFilter, status);
            }
        public User GetUserByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return _UserRepository.GetUserByName(name);
            }
            return null;
        }
    }
}
