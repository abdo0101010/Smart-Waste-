using Microsoft.AspNetCore.Http.HttpResults;
using SmartWaste.DTO.Register;
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
            return _UserRepository.GetAllUserDtos();
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
        public async Task CreateUser(UserCreationDTO userCreationDTO)
        {
            if (userCreationDTO != null)
            {

                await _UserRepository.CreateUser(userCreationDTO);
            }
        }
        public GetSpecficUser GetUserByIdWithDetails(int userId)
        {

            if (userId > 0)
            {
                return _UserRepository.GetUserByIdWithDetails(userId);
            }
            return null;

        }
        public List<UserRankDTO> SortUsersByWalletPoints(string sortOrder)
        {
            if (!string.IsNullOrEmpty(sortOrder))
            {
                return _UserRepository.SortUsersByWalletPoints(sortOrder);
            }
            return null;
        }

        public UserRankDTO GetRankingUser(int id, string sortOrder)
        {
            if (id > 0 && !string.IsNullOrEmpty(sortOrder))
            {
                return _UserRepository.GetRankingUser(id, sortOrder);
            }
            return null;
        }
        public int GetAvgPointsUsers()
        {
            return _UserRepository.GetAvgPointsUsers();
        }
        public void RegisterUser(dataforregister userCreationDTO)
        {
            if (userCreationDTO != null)
            {
                _UserRepository.RegisterUser(userCreationDTO);
            }

        }
        public void UpdateUser(updateUser newUser, int id)
        {
            if (newUser != null && id > 0)
            {
                _UserRepository.UpdateUser(newUser, id);
            }
        }
        public async Task UpdateUserBottlesAndPointsAsync(int userId, int bottleCount, decimal pointsEarned)
        {
            if (userId > 0 && bottleCount >= 0 && pointsEarned >= 0)
            {
                await _UserRepository.UpdateUserBottlesAndPointsAsync(userId, bottleCount, pointsEarned);
            }
        }
        public void ForgetPassword(string? email, string newPassword, string confirmPassword, string role, string? Phone)
        {
            if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(role))
            {
                if ((role.Equals("Recycler", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(Phone)) ||
                    (role.Equals("User", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(email)))
                {
                    _UserRepository.ForgetPassword(email, newPassword, confirmPassword, role, Phone);
                }
                else
                {
                    throw new ArgumentException("Identifying information (Email/Phone) is missing for the selected role.");
                }
            }
            else
            {
                throw new ArgumentException("Password, Confirm Password, and Role are required.");
            }
        }
        public List<UserDetailsForAdminDTo> GetAllUsersWithDetailsForAdmin()
        {
            var users = _UserRepository.GetAllUsersWithDetailsForAdmin();
            if (users == null || !users.Any())
            {
                throw new InvalidOperationException("No users found with details for admin.");
            }
            return users;
        }
        public async Task feedbackRating(int requestId, int rating, string comment)
        {
            if (requestId > 0 && rating >= 1 && rating <= 5)
            {
                await _UserRepository.feedbackRating(requestId, rating, comment);
            }
            else
            {
                throw new ArgumentException("Invalid request ID or rating. Rating must be between 1 and 5.");
            }
        }
        public async Task SaveChangesAsync()
        {
            await _UserRepository.SaveChangesAsync();
        }
    }
}
