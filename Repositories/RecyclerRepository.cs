using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.RecuclerDTOS;
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

        public List<ReyclerDetailsAdimDto> GetReyclerDetails()
        {
            return _context.Recyclers.Select(r => new ReyclerDetailsAdimDto
            {
                RecyclerID = r.RecyclerId,
                FullName = r.FullName,
                Phone = r.Phone,
                VehicleInfo = r.VehicleInfo,
                Status = r.Status,
                Rating = r.Rating,
                TotalTripsCompleted = r.PickupRequests.Count()
            }).ToList();
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
    }
}
