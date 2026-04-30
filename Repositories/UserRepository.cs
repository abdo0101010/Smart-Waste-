using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;
using SmartWaste.Models;
using System.Linq;

namespace SmartWaste.Repositories
{
    public class UserRepository: IUserRepository
    {
         smartwasteContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserRepository(smartwasteContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public void AddUser(User user)
        {
            
                _context.Users.Add(user);
                SaveChanges();
            
        }
        public User GetUserByName(string name)
        {
            return _context.Users.FirstOrDefault(u => u.FullName.ToLower() == name.ToLower());
        }
        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            SaveChanges(); 
        }

        public void DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
               SaveChanges();
            }
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.Include(u=>u.PickupRequests).ToList();
        }
        public List<UserDTo> GetAllUserDtos()
        {
            List<User> users = _context.Users.Include(u => u.PickupRequests).Include(u => u.UserRedemptions).ToList();
            List<UserDTo> userDtos = new List<UserDTo>();
            foreach (var user in users)
            {
                UserDTo userDto = new UserDTo
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    WalletPoints = user.WalletPoints,
                    PickupRequests = user.PickupRequests.Select(p => new PickupRequestDTO
                    {
                        RequestId = p.RequestId,
                        EstimatedPoints = p.EstimatedPoints,
                        FinalPoints = p.FinalPoints,
                        HubStaffId = p.HubStaffId,
                        RecyclerId = p.RecyclerId,
                        RequestDate = p.RequestDate,
                        VerificationDate = p.VerificationDate,
                        UserId = p.UserId,
                        PickupDate = p.PickupDate,
                        Status = p.Status
                    }).ToList(),
                    UserRedemptions = user.UserRedemptions.Select(r => new UserRedemptionDTO
                    {
                        RedemptionId = r.RedemptionId,
                        UserId = r.UserId,
                        RewardId = r.RewardId,
                        RedeemedAt = r.RedeemedAt,
                        VoucherCode = r.VoucherCode
                    }).ToList()
                };  
              
                userDtos.Add(userDto);
            }

            //_context.Users.Select(u => new UserDTo
            //{
            //    UserId = u.UserId,
            //    FullName = u.FullName,
            //    Email = u.Email,
            //    Address = u.Address,
            //    WalletPoints = u.WalletPoints,
            //    PickupRequests = u.PickupRequests,
            //    UserRedemptions = u.UserRedemptions
            //}).ToList();
                return userDtos;
        }
        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }
        public int GetTotalActiveUsers()
        {
            return _context.Users.Count(u => string.IsNullOrEmpty(u.Status) || u.Status == "Active");
        }
        public decimal? GetTotalWalletPoints()
        {
            return _context.Users.Sum(u => u.WalletPoints);
        }

        public List<UserFilterAdminDTO> GetUsersByFilter(string KeyofFilter, string status)
        {
          
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(KeyofFilter))
            {
                query = query.Where(u => u.FullName.Contains(KeyofFilter) || u.Email.Contains(KeyofFilter));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.Status.ToLower() == status.ToLower());
            }

            
            var filteredUsers = query.Select(u => new UserFilterAdminDTO
            {
                UserId = u.UserId,
                Name = u.FullName,
                Email = u.Email,
                Address = u.Address,
                WalletPoints = u.WalletPoints,
                IsActive = u.Status,
                
                TotalRequests = u.PickupRequests.Count(),


                Quantity = u.PickupRequests
                            .SelectMany(p => p.RequestItems)
                            .Sum(ri => ri.Quantity)
            }).ToList(); 

            return filteredUsers;
        }
        public async Task CreateUser(UserCreationDTO userCreationDTO)
        {
            string? imagePath = null;

            if (userCreationDTO.ProfilePictureUrl != null && userCreationDTO.ProfilePictureUrl.Length > 0)
            {
                // ✅ التعديل هنا: لو الـ WebRootPath بـ null، بنستخدم مسار المشروع الحالي
                var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string uploadsFolder = Path.Combine(rootPath, "images", "users");

                // التأكد إن الفولدرات موجودة (بيكريت السلسلة كلها لو مش موجودة)
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + userCreationDTO.ProfilePictureUrl.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await userCreationDTO.ProfilePictureUrl.CopyToAsync(fileStream);
                }

                imagePath = "/images/users/" + uniqueFileName;
            }

            User user = new User
            {
                FullName = userCreationDTO.FullName,
                Email = userCreationDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreationDTO.Password),
                Address = userCreationDTO.Address,
                ProfilePictureUrl = imagePath
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}
