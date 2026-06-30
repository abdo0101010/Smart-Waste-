using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.Register;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.DTO.UserDTO;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.DTO.UserRedemptionDTOS;
using SmartWaste.Models;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            // 🚀 AsNoTracking بتفرتك أي كاش وتجبر الـ EF تقرأ الـ PasswordHash الجديد حالا من الـ SQL Server
            return _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.FullName.ToLower() == name.ToLower());
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
                    //UserRedemptions = user.UserRedemptions.Select(r => new UserRedemptionDTO
                    //{
                    //    RedemptionId = r.RedemptionId,
                    //    UserId = r.UserId,
                    //    RewardId = r.RewardId,
                    //    RedeemedAt = r.RedeemedAt,
                    //    VoucherCode = r.VoucherCode
                    //}).ToList()
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
                ProfilePictureUrl = imagePath,
                Phone = userCreationDTO.Phone
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public GetSpecficUser GetUserByIdWithDetails(int userId)
        {
            var user= _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }
                GetSpecficUser NUesr = new GetSpecficUser
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                WalletPoints = user.WalletPoints,
                Phone = user.Phone

            };

            return NUesr;
        }
        public List<UserRankDTO> SortUsersByWalletPoints(string sortOrder)
        {
            var users = _context.Users.AsQueryable();
            if (sortOrder == "asc")
            {
                users = users.OrderBy(u => u.WalletPoints);


            }
            else if (sortOrder == "desc")
            {
                users = users.OrderByDescending(u => u.WalletPoints);
            }
            var usersList = users.ToList();
            List<UserRankDTO> sortedUsers = usersList.Select((u, index) => new UserRankDTO
            {
                UserId = u.UserId,
                Name = u.FullName,
                BottleCount=u.Bottle,
                WalletPoints = u.WalletPoints ?? 0
                ,Rank=index+1
            }).ToList();
            return sortedUsers;
        }
        public UserRankDTO GetRankingUser(int id, string sortOrder)
        {
            var users = SortUsersByWalletPoints(sortOrder);
            var rankedUsers = users.Select((u, index) => new UserRankDTO
            {
                UserId = u.UserId,               
                Name = u. Name,
                WalletPoints = u.WalletPoints, 
                Rank = index + 1,                
                BottleCount = u.BottleCount          
            }).ToList(); var rankedUser = rankedUsers.FirstOrDefault(u => u.UserId   == id);
            var targetUser = rankedUsers.FirstOrDefault(u => u.UserId == id);

            return targetUser;
        }
        public int GetAvgPointsUsers()
        {
            var avgPoints = _context.Users.Average(u => u.WalletPoints);
            return (int)avgPoints;
        }
        public void RegisterUser(dataforregister userCreationDTO)
        {
            User user = new User
            {

                FullName = userCreationDTO.FullName,
                Email = userCreationDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreationDTO.PasswordHash),
                Address = userCreationDTO.Address,
                Role = userCreationDTO.Role,
                Phone = userCreationDTO.Phone
            };
            _context.Users.Add(user);
            SaveChanges();
        }
        public void UpdateUser(updateUser newUser, int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                throw new KeyNotFoundException($"user is no exist");
            }

            if (!string.IsNullOrEmpty(newUser.PasswordHash))
            {
                if (newUser.PasswordHash != newUser.ConfirmPassword)
                {
                    throw new InvalidOperationException("Password and confirm password do not match.");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            }

            user.FullName = newUser.FullName;
            user.Address = newUser.Address;

          
            _context.Users.Update(user);

            _context.SaveChanges();
        }
        public async Task UpdateUserBottlesAndPointsAsync(int userId, int bottleCount, decimal pointsEarned)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) throw new KeyNotFoundException("User not found.");

            // تحويل حقل الـ Bottle الحالي لرقم وتزويد الجديد عليه
            int currentBottles = 0;
            if (!string.IsNullOrEmpty(user.Bottle))
            {
                int.TryParse(user.Bottle, out currentBottles);
            }

            user.Bottle = (currentBottles + bottleCount).ToString();
            user.WalletPoints = (user.WalletPoints ?? 0) + pointsEarned;

            await _context.SaveChangesAsync();
        }
        public void ForgetPassword(string? email, string newPassword, string confirmPassword, string role, string? Phone)
        {
            if (newPassword != confirmPassword)
            {
                throw new InvalidOperationException("Password and confirm password do not match.");
            }

            string hashedPass = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (role.Equals("Recycler", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(Phone))
                {
                    throw new ArgumentException("Phone number is required for Recycler password reset.");
                }

                var recycler = _context.Recyclers.FirstOrDefault(r => r.Phone == Phone);
                if (recycler == null)
                    throw new KeyNotFoundException($"Recycler with phone number '{Phone}' not found."); 

                recycler.PasswordHash = hashedPass;
                _context.Recyclers.Update(recycler);
            }
            else 
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email is required for User password reset.");
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                    throw new KeyNotFoundException($"User with email '{email}' not found.");

                user.PasswordHash = hashedPass;
                _context.Users.Update(user);
            }

            _context.SaveChanges();
        }
        public List<UserDetailsForAdminDTo> GetAllUsersWithDetailsForAdmin()
        {
            return _context.Users.Select(u => new UserDetailsForAdminDTo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Phone = u.Phone
            }).ToList();
        }
        public async Task feedbackRating(int requestId, int rating, string comment)
        {
            // 1. التشيك إن الـ Request موجود فعلياً ومربوط بدرايفر (Recycler)
            var request = await _context.PickupRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                throw new KeyNotFoundException($"طلب التجميع رقم {requestId} غير موجود في السيستم.");

            if (request.RecyclerId == null)
                throw new InvalidOperationException("لا يمكن تقييم هذا الطلب لأنه لم يتم تعيين سائق (Recycler) له بعد.");

            // 2. إنشاء كائن الـ Feedback بناءً على الـ Properties اللي عندك في الموديل بالظبط
            var feedback = new Feedback
            {
                RequestId = requestId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
                // ملحوظة: مش هنحط RecyclerId هنا لأن الموديل بتاعك مش بيدعمه مباشرة، هو مربوط بالـ Request والـ Request قايم بالواجب.
            };

            // 3. الإضافة والحفظ الأكيد في الـ SQL Server
            await _context.Feedbacks.AddAsync(feedback);
            await SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<string> UpdateUserProfilePictureAsync(int id, IFormFile file)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) throw new KeyNotFoundException("User not found");
            // 1. تجهيز مسار الـ wwwroot/images/users
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(rootPath, "images", "users");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            // 2. عمل اسم فريد للصورة ومنع التكرار
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // 3. حفظ الملف على السيرفر
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            // 4. تحديث المسار جوه الداتابيز والحفظ
            string dbImagePath = "/images/users/" + uniqueFileName;
            user.ProfilePictureUrl = dbImagePath;

            await _context.SaveChangesAsync();

            return dbImagePath; // بنرجع المسار الجديد عشان لو الفرونت إند حابب يعرضه فوراً
        }

    }
}
