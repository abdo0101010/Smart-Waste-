using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.DTO.Register;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Models;
using System.Threading.Tasks;

namespace SmartWaste.Repositories
{
    public class RecyclerRepository : IRecyclerRepository
    {
        smartwasteContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RecyclerRepository(smartwasteContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public void AddRecycler(Recycler recycler)
        {
            _context.Recyclers.Add(recycler);
            SaveChanges();
        }
        public Recycler GetRecyclerById(int id)
        {
            return _context.Recyclers.Find(id);
        }
        public void UpdateRecycler(Recycler recycler)
        {
            _context.Recyclers.Update(recycler);
            SaveChanges();
        }
        public void DeleteRecycler(int id)
        {
            var recycler = _context.Recyclers.Find(id);
            if (recycler != null)
            {
                _context.Recyclers.Remove(recycler);
                SaveChanges();
            }
        }
        public Recycler GetRecyclerByEmail(string email)
        {
            return _context.Recyclers.FirstOrDefault(r => r.Email.ToLower() == email.ToLower());
        }
        public Recycler GetRecyclerByName(string Name)
        {
            return _context.Recyclers.FirstOrDefault(r => r.FullName.ToLower() == Name.ToLower());
        }
        public IEnumerable<Recycler> GetAllRecyclers()
        {
            return _context.Recyclers.ToList();
        }
        public List<Recycler> GetAllRecyclersWithPickupRequests()
        {
            return _context.Recyclers.Include(r => r.PickupRequests).ToList();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public decimal? GetAvgRatingRecyclers()
        {
            return _context.Recyclers.Average(r => r.Rating);
        }
        public int GetTotalRecyclers()
        {
            return _context.Recyclers.Count();
        }
        public int GetTotalRecyclingActive()
        {
            return _context.Recyclers.Count(r => r.Status == "Active");
        }
        public ReyclerDetailsAdimDto GetRecyclerDetailsById(int recyclerId)
        {
            var recycler = _context.Recyclers.Include(r => r.PickupRequests)
                .FirstOrDefault(r => r.RecyclerId == recyclerId);
            if (recycler == null)
            {
                return null;
            }
            var totalTripsCompleted = _context.PickupRequests
                .Count(p => p.RecyclerId == recyclerId && p.Status == "Completed");
            return new ReyclerDetailsAdimDto
            {
                RecyclerID = recycler.RecyclerId,
                FullName = recycler.FullName,
                Phone = recycler.Phone,
                VehicleInfo = recycler.VehicleInfo,
                Status = recycler.Status,
                Rating = recycler.Rating,
                TotalTripsCompleted = totalTripsCompleted
            };
        }

        public List <ReyclerDetailsAdimDto> GetAllRecyclersWithDetails()
        {
            var recyclersList = _context.Recyclers.Include(r => r.PickupRequests)
                       .Select(recycler => new ReyclerDetailsAdimDto
                       {
                           RecyclerID = recycler.RecyclerId,
                           FullName = recycler.FullName,
                           Phone = recycler.Phone,
                           VehicleInfo = recycler.VehicleInfo,
                           Status = recycler.Status,
                           Rating = recycler.Rating,
                           TotalTripsCompleted = _context.PickupRequests
                          .Count(p => p.RecyclerId == recycler.RecyclerId && p.Status == "Completed")
                       })
                       .ToList();
            return recyclersList;
        }
        public List<RecyclerWithTotaltripDTO> GetSortingRecyclersByRating()
        {
            return _context.Recyclers.OrderByDescending(r => r.Rating).Select(r => new RecyclerWithTotaltripDTO
            {
                RecyclerID = r.RecyclerId,
                FullName = r.FullName,
                TotalTrips = r.PickupRequests.Count(),
                Rating = r.Rating
            }).ToList();
        }
        public void UpdateRecyclerStatus(int recyclerId, string newStatus)
        {
            var recycler = _context.Recyclers.Find(recyclerId);
            if (recycler != null)
            {
                recycler.Status = newStatus;
                _context.Recyclers.Update(recycler);
                SaveChanges();
            }
        }
        public async Task CreateRecycler(RecyclerCreationDTO recyclerCreationDTO)
        {
            string? imagePath = null;

            if (recyclerCreationDTO.ProfilePictureUrl != null && recyclerCreationDTO.ProfilePictureUrl.Length > 0)
            {
                // ✅ التعديل هنا: لو الـ WebRootPath بـ null، بنستخدم مسار المشروع الحالي
                var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string uploadsFolder = Path.Combine(rootPath, "images", "recyclers");

                // التأكد إن الفولدرات موجودة (بيكريت السلسلة كلها لو مش موجودة)
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + recyclerCreationDTO.ProfilePictureUrl.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await recyclerCreationDTO.ProfilePictureUrl.CopyToAsync(fileStream);
                }

                imagePath = "/images/recyclers/" + uniqueFileName;
            }
            var recycler = new Recycler
            {
                FullName = recyclerCreationDTO.FullName,
                Phone = recyclerCreationDTO.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(recyclerCreationDTO.PasswordHash)
                ,
                Email = recyclerCreationDTO.Email,
                ProfilePictureUrl = imagePath

            };
            await _context.Recyclers.AddAsync(recycler);
            await _context.SaveChangesAsync();
        }
        public void RegisterRecycler(dataforregister recyclerCreationDTO)
        {
            var recycler = new Recycler
            {
                FullName = recyclerCreationDTO.FullName,
                PasswordHash  = BCrypt.Net.BCrypt.HashPassword(recyclerCreationDTO.PasswordHash),
                Role= recyclerCreationDTO.Role,
                    Phone = recyclerCreationDTO.Phone
                    ,
                    Email = recyclerCreationDTO.Email,
                   


            };
            _context.Recyclers.Add(recycler);
            SaveChanges();
        }
        public async Task<bool> UpdateRecyclerAsync(int recyclerId, RecyclerUpdateDTO dto)
        {
            var recycler = await _context.Recyclers.FirstOrDefaultAsync(r => r.RecyclerId == recyclerId);
            if (recycler == null) return false;
            recycler.FullName = dto.FullName;
            recycler.Phone = dto.Phone;
            recycler.VehicleInfo = dto.VehicleInfo;
            _context.Recyclers.Update(recycler);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string> UpdateRecyclerProfilePictureAsync(int id, IFormFile file)
        {
            var recycler = await _context.Recyclers.FirstOrDefaultAsync(r => r.RecyclerId == id);
            if (recycler == null) throw new KeyNotFoundException("Recycler not found");
            // 1. تجهيز الفولدر الخاص بصور السائقين
            var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(rootPath, "images", "recyclers");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. عمل اسم فريد للصورة لمنع التكرار
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // 3. حفظ الملف على السيرفر
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. مسح الصورة القديمة لو موجودة عشان نوفر مساحة على السيرفر (حركة هندسية ذكية)
            if (!string.IsNullOrEmpty(recycler.ProfilePictureUrl))
            {
                string oldFilePath = Path.Combine(rootPath, recycler.ProfilePictureUrl.TrimStart('/'));
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
            }

            // 5. تحديث المسار جوه الداتابيز والحفظ
            string dbImagePath = "/images/recyclers/" + uniqueFileName;
            recycler.ProfilePictureUrl = dbImagePath;

            await _context.SaveChangesAsync();

            return dbImagePath; // بنرجع المسار الجديد للفرونت إند عشان يعرضه فوراً
        }
    }
}
