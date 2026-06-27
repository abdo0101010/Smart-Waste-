using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.PickupRequestDTOS;
using SmartWaste.DTO.RequestItemDTOS;
using SmartWaste.Hubs;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class PickupRequestRepository : IPickupRequestRepository
    {
        smartwasteContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PickupRequestRepository(smartwasteContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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
        public async Task<bool> AcceptBulkPickupRequestsAsync(List<int> requestIds, int recyclerId)
        {
            // 🎯 الشرط السحري لتقليل التكلفة: الحد الأدنى لعدد الطلبات في المشوار الواحد
            int minRequiredRequests = 5; // لازم السائق يلم 3 بيوت أو طلبات على الأقل في النزلة

            if (requestIds == null || requestIds.Count < minRequiredRequests)
            {
                throw new InvalidOperationException($"عذراً، لتقليل تكلفة الشحن، لا يمكنك النزول لطلب أو اثنين فقط. يجب أن تختار {minRequiredRequests} طلبات على الأقل معاً لتشكيل خط سير مجمع.");
            }

            // جلب كل الطلبات المبعوثة من الداتابيز للتأكد إنها لسه "Open" وموجودة
            var requests = await _context.PickupRequests
                .Where(p => requestIds.Contains(p.RequestId) && p.Status == "Open")
                .ToListAsync();

            // لو عدد الطلبات المتاحة فعلياً في الداتابيز أقل من اللي السواق اختارهم (لو حد خطف طلب منهم مثلاً)
            if (requests.Count < requestIds.Count)
            {
                throw new Exception("بعض الطلبات التي اخترتها تم قبولها بالفعل من سائق آخر أو غير متاحة.");
            }

            // ----------------------------------------------------
            // لو كله تمام والعدد مستوفي الشروط، بنحدثهم كلهم Loop واحدة 🚀
            // ----------------------------------------------------
            foreach (var request in requests)
            {
                request.RecyclerId = recyclerId;
                request.Status = "In Progress";

                try
                {
                    // تسجيل إشعار في الداتابيز لكل مواطن ميكلبش في خط السير ده
                    var notification = new Notification
                    {
                        Title = "Driver is on the way! 🚛",
                        Message = $"A driver has accepted your request (ORD-{request.RequestId}) in a combined route.",
                        Type = "Pickup",
                        CreatedAt = DateTime.UtcNow,
                        UserId = request.UserId
                    };
                    _context.Notifications.Add(notification);
                }
                catch (Exception) { /* تخطي إيرور الإشعار الفردي لضمان حفظ الطلب */ }
            }

            // حفظ كل الطلبات والإشعارات في خطوة واحدة قوية جوه الـ SQL Server
            await _context.SaveChangesAsync();

            // ضخ إشعار عام للـ SignalR إن فيه طلبات اتجمعت واتقبلت
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Route Started", $"{requests.Count} requests have been batched into a new route.", "Pickup");

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
        public async Task<IEnumerable<PickupRequest>> GetRequestsByRecyclerIdAsync(int recyclerId)
        {
            return await _context.PickupRequests
                .Where(r => r.RecyclerId == recyclerId)
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
        public async Task<IEnumerable<PickupRequestViewModelDTO>> GetRecyclerHistoryAsync(int recyclerId)
        {
            return await _context.PickupRequests
                .Where(r => r.RecyclerId == recyclerId)
                .Include(r => r.User) // عشان نقدر نجيب اسم المواطن وعنوانه (Zone)
                .Include(r => r.RequestItems)
                    .ThenInclude(ri => ri.Category) // عشان نقدر نجيب الـ CategoryName للمخلفات
                .OrderByDescending(r => r.RequestDate) // الترتيب من أحدث طلب لأقدم طلب
                .Select(r => new PickupRequestViewModelDTO
                {
                    RequestId = r.RequestId,
                    UserName = r.User != null ? r.User.FullName : "مستخدم غير معروف",
                    Address = r.User != null ? r.User.Address : "عنوان غير معروف",
                    Status = r.Status,
                    Priority = r.Priority,
                    RequestDate = r.RequestDate,
                    PickupDate = r.PickupDate,
                    Categories = r.RequestItems.Select(ri => ri.Category.CategoryName).ToList()
                })
                .ToListAsync();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
