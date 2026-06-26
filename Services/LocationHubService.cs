using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWaste.Services
{
    public class LocationHubService : ILocationHubService
    {
        private readonly smartwasteContext _context;

        public LocationHubService(smartwasteContext context)
        {
            _context = context;
        }

        // 🟢 ميثود الـ POST: بتضرب UPDATE مباشر في الـ SQL Server بدون أي كراش
        public async Task UpdateDriverLocation(int driverId, double lat, double lng, string status, int capacity)
        {
            await _context.Recyclers
                .Where(r => r.RecyclerId == driverId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(r => r.CurrentLatitude, lat)
                    .SetProperty(r => r.CurrentLongitude, lng)
                    .SetProperty(r => r.CurrentStatus, status)
                    .SetProperty(r => r.CurrentCapacity, capacity)
                );
        }

        // 🟢 ميثود الـ GET: بتقرأ لايف كل ثانيتين أوتوماتيك من الداتابيز
        public async IAsyncEnumerable<object> GetTruckLocationStream(int driverId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            double? lastLat = null;
            double? lastLng = null;
            bool isFirstRun = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                // عمل خط غير متعقب (AsNoTracking) لضمان جلب البيانات الجديدة فوراً
                var driver = await _context.Recyclers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.RecyclerId == driverId, cancellationToken);

                if (driver == null) yield break;

                if (isFirstRun || driver.CurrentLatitude != lastLat || driver.CurrentLongitude != lastLng)
                {
                    lastLat = driver.CurrentLatitude;
                    lastLng = driver.CurrentLongitude;
                    isFirstRun = false;

                    yield return new
                    {
                        DriverId = driver.RecyclerId,
                        Latitude = driver.CurrentLatitude ?? 0.0,
                        Longitude = driver.CurrentLongitude ?? 0.0,
                        Status = string.IsNullOrEmpty(driver.CurrentStatus) ? "Offline" : driver.CurrentStatus,
                        Capacity = driver.CurrentCapacity ?? 0
                    };
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        public async Task JoinAdminGroup() => await Task.CompletedTask;
        public async Task JoinAdminGroup(string connectionId) => await Task.CompletedTask;
    }
}