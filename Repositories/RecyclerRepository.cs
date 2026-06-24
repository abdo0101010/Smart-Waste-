using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.DTO.Register;
using SmartWaste.DTO.UserDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class RecyclerRepository : IRecyclerRepository
    {
        smartwasteContext _context;
        public RecyclerRepository(smartwasteContext context)
        {
            _context = context;
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

        public ReyclerDetailsAdimDto GetRecyclerByIdWithDetails(int recyclerId)
        {
            var recycler = _context.Recyclers.Include(r => r.PickupRequests)
                       .FirstOrDefault(r => r.RecyclerId == recyclerId); 
            if (recycler == null) return null;
            ReyclerDetailsAdimDto singleRecycler = new ReyclerDetailsAdimDto
            {
                RecyclerID = recycler.RecyclerId,
                FullName = recycler.FullName,
                Phone = recycler.Phone,
                VehicleInfo = recycler.VehicleInfo,
                Status = recycler.Status,
                Rating = recycler.Rating,

                TotalTripsCompleted = _context.PickupRequests.Count(p => p.RecyclerId == recyclerId && p.Status == "Completed")

            };
            return singleRecycler;
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
        public void CreateRecycler(RecyclerCreationDTO recyclerCreationDTO)
        {
            var recycler = new Recycler
            {
                FullName = recyclerCreationDTO.FullName,
                Phone = recyclerCreationDTO.Phone,
                PasswordHash = recyclerCreationDTO.PasswordHash
            };
            _context.Recyclers.Add(recycler);
            SaveChanges();
        }
        public void RegisterRecycler(dataforregister recyclerCreationDTO)
        {
            var recycler = new Recycler
            {
                FullName = recyclerCreationDTO.FullName,
                PasswordHash = recyclerCreationDTO.PasswordHash,
                Role= recyclerCreationDTO.Role,
                    Phone = recyclerCreationDTO.Phone


            };
            _context.Recyclers.Add(recycler);
            SaveChanges();
        }
    }
}
