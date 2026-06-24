using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
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
        public PickupInfoDTOS GetTodayPickupSummary()
        {
            //   تاريخ النهاردة بالظبط عشان نستخدمه في فلترة الطلبات اللي خلصت النهاردة
            var today = DateTime.Today;
            var openRequests = _context.PickupRequests.Count(p => p.Status == "Open");
            var inProgressRequests = _context.PickupRequests.Count(p => p.Status == "In Progress");
            var completedTodayRequests = _context.PickupRequests.Count(p => p.Status == "Completed"
                                                                       && p.PickupDate.HasValue
                                                                       && p.PickupDate.Value.Date == today);
            var slaBreachedRequests = _context.PickupRequests.Count(p => p.Status == "SLA Breached");
            return new PickupInfoDTOS
            {
                OpenCount = openRequests,
                InProgressCount = inProgressRequests,
                CompletedTodayCount = completedTodayRequests,
                SlaBreachedCount = slaBreachedRequests

            };


        }

        public List<PickupRequest> GetRecyclerRequestsWithFilters(string? search, string? status, string? priority ,string? zone, string? material)
        {
            var query = _context.PickupRequests
                           .Include(p => p.User)        //  بيانات المواطن عشان اسمه وعنوانه
                           .Include(p => p.Recycler)    //  بيانات السواق عشان اعرف مين اللي رايح
                           .Include(p => p.HubStaff)    //  بيانات موظف المخزن اللي استلم الأوردر في الآخر
                           .Include(p => p.RequestItems) // 1. بنعمل انكلود للجدول الوسيط الأول
                           .ThenInclude(ri => ri.Category)
                           .AsQueryable();
            // 2. فلترة الحالة (Pending, In Progress...) 
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.Where(p => p.Status == status);
            }
            if (!string.IsNullOrEmpty(priority) && priority != "all")
            {
                query = query.Where(p => p.Priority == priority); 
            }
            if (!string.IsNullOrEmpty(zone) && zone != "all")
            {
                query = query.Where(p => p.User.Address.Contains(zone));
            }
            if (!string.IsNullOrEmpty(material) && material != "all")
            {
                query = query.Where(p => p.RequestItems.Any(ri => ri.Category.CategoryName == material));
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.RequestId.ToString().Contains(search) ||
                                         p.Status.Contains(search) ||
                                         p.Priority.Contains(search) ||
                                        p.RequestItems.Any(ri => ri.Category.CategoryName.Contains(search))||
                                        p.User.FullName.Contains(search) ||
                                         p.User.Address.Contains(search));   
            }
            return query.ToList();
        }
        public bool AcceptPickupRequest(int requestId, int recyclerId)
        {
            // 1. بنجيب الطلب من الداتابيز باستخدام الـ ID بتاعه
            var request = _context.PickupRequests.FirstOrDefault(p => p.RequestId == requestId);

            // 2. لو الطلب مش موجود أو مش جاهز للاستلام بنرجع false
            if (request == null || request.Status != "Open")
            {
                return false;
            }

            // 3.  ربط الطلب بالسواق وتغيير الحالة لـ قيد التنفيذ
            request.RecyclerId = recyclerId;
            request.Status = "In Progress";          
            return true;
        }
        public async Task<IEnumerable<PickupRequest>> GetRequestsByUserIdAsync(int userId)
        {
            return await _context.PickupRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.User) // عشان نقدر نجيب اسم المواطن وعنوانه (Zone)
                .Include(r => r.RequestItems)
                    .ThenInclude(ri => ri.Category) // عشان نقدر نجيب الـ CategoryName للمخلفات
                .OrderByDescending(r => r.RequestDate) // الترتيب من أحدث طلب لأقدم طلب
                .ToListAsync();
        }
        public async Task<IEnumerable<PendingRequestFormDTO>> GetPendingHubRequestsAsync()
        {
            // سحب الطلبات المعلقة وعمل Join مع جدول المستخدمين لجلب الاسم
            var pendingRequests = await _context.PickupRequests
                .Include(p => p.User)
                .Where(p => p.Status == "Pending")
                .OrderByDescending(p => p.RequestDate) // الأحدث يظهر فوق
                .Select(p => new PendingRequestFormDTO
                {
                    RequestId = p.RequestId,
                    UserName = p.User != null ? p.User.FullName : "مستخدم غير معروف",
                    Status = p.Status,
                    // تنسيق التاريخ والوقت ليظهر بشكل كلين للـ Hub Staff
                    TimeAgo = p.RequestDate.HasValue ? p.RequestDate.Value.ToString("yyyy-MM-dd hh:mm tt") : "N/A"
                })
                .ToListAsync();

            return pendingRequests;
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
