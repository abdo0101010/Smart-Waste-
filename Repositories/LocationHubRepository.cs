using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SmartWaste.Repositories
{
    public class LocationHubRepository : Hub, ILocationHubRepository
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // 🔑 بنجيب الـ ScopeFactory بس عشان نضمن عزل ونظافة الخطوط والربط الفوري
        public LocationHubRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task UpdateDriverLocation(int driverId, double lat, double lng, string status, int capacity)
        {
            // 1. البث السريع لشاشات الأدمن عبر الـ SignalR (شغال تمام)
            await Clients.Group("Admins").SendAsync("ReceiveDriverLocation", new
            {
                DriverId = driverId,
                Latitude = lat,
                Longitude = lng,
                Status = status,
                Capacity = capacity
            });

            // 2. 🔑 التحديث الصاعق والمباشر في الداتابيز بدون كاش وبدون تعليق
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<smartwasteContext>();

                // أمر UPDATE مباشر وصريح بيروح للـ SQL Server في خطوة واحدة
                await db.Recyclers
                    .Where(r => r.RecyclerId == driverId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(r => r.CurrentLatitude, lat)
                        .SetProperty(r => r.CurrentLongitude, lng)
                        .SetProperty(r => r.CurrentStatus, status)
                        .SetProperty(r => r.CurrentCapacity, capacity)
                    );
            }
        }

        public async IAsyncEnumerable<object> GetTruckLocationStream(
    int driverId,
    [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            double? lastLat = null;
            double? lastLng = null;
            bool isFirstRun = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<smartwasteContext>();

                    var driver = await db.Recyclers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RecyclerId == driverId, cancellationToken);

                    if (driver == null) yield break;

                    // مقارنة الإحداثيات (بندعم مقارنة الـ null برضه)
                    if (isFirstRun || driver.CurrentLatitude != lastLat || driver.CurrentLongitude != lastLng)
                    {
                        lastLat = driver.CurrentLatitude;
                        lastLng = driver.CurrentLongitude;
                        isFirstRun = false;

                        // 🔑 الحل السحري: منع الـ null نهائياً وتبديلها بأصفار وقيم نصية واضحة عشان المتصفح يقبل البث
                        yield return new
                        {
                            DriverId = driver.RecyclerId,
                            Latitude = driver.CurrentLatitude ?? 0.0,            // لو null هينزل 0.0
                            Longitude = driver.CurrentLongitude ?? 0.0,          // لو null هينزل 0.0
                            Status = string.IsNullOrEmpty(driver.CurrentStatus) ? "Offline" : driver.CurrentStatus, // لو فاضي هينزل Offline
                            Capacity = driver.CurrentCapacity ?? 0               // لو null هينزل 0
                        };
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
    }
}