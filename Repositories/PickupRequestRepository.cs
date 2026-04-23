using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class PickupRequestRepository : IPickupRequestRepository
    {
        smartwasteContext _context;
        public PickupRequestRepository(smartwasteContext context)
        {
            _context = context;
        }

        public void AddPickupRequest(PickupRequest pickupRequest)
        {
            _context.PickupRequests.Add(pickupRequest);
            SaveChanges();
        }

        public PickupRequest GetPickupRequestById(int id)
        {
            return _context.PickupRequests.Find(id);
        }

        public void UpdatePickupRequest(PickupRequest pickupRequest)
        {
            _context.PickupRequests.Update(pickupRequest);
            SaveChanges();
        }
        public void DeletePickupRequest(int id)
        {
            var pickupRequest = _context.PickupRequests.Find(id);
            if (pickupRequest != null)
            {
                _context.PickupRequests.Remove(pickupRequest);
                SaveChanges();
            }
        }

        public IEnumerable<PickupRequest> GetAllPickupRequests()
        {
            return _context.PickupRequests.ToList();
        }

        public List<PickupRequest> GetAllPickupRequestsWithRecyclersAndHubStaff()
        {
            return _context.PickupRequests
                .Include(p => p.Recycler)
                .Include(p => p.HubStaff)
                .ToList();
        }

        public int GetTotalPickupRequests()
        {
            return _context.PickupRequests.Count();
        }
        public decimal? TotalEaring()
        {
            return _context.PickupRequests.Sum(t => t.FinalPoints);
        }
      
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
