using Microsoft.AspNetCore.SignalR;
using SmartWaste.Models;

namespace SmartWaste.Hubs
{
    public class ChatHub: Hub
    {
        smartwasteContext _smartwasteContext;
        public ChatHub(smartwasteContext smartwasteContext )
        {
            _smartwasteContext = smartwasteContext;

        }
        public override Task OnConnectedAsync()
        {

            return base.OnConnectedAsync();
        }

        //public async Task addgroup(string groupName)
        //{
        //    var user=await _smartwasteContext.Users.FindAsync(Context.UserIdentifier);
        //    if (groupName != null)
        //    {
        //       if (!_smartwasteContext.groups.Any(g => g.Name == groupName))
        //        {
        //            var userconnectionId = new UserConnection { UserId = Context.UserIdentifier, ConnectionId = Context.ConnectionId };
        //            var UserinGroup = new UserinGroups { UserId = Context.UserIdentifier, GroupId = group.Id.ToString() };
        //            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        //            var group = new groups { Name = groupName };
        //            await Clients.Group(groupName).SendAsync("GroupCreated", group);
        //        }
        //       else
        //        {
        //            await _smartwasteContext.groups.AddAsync(group);
        //            var userconnectionId = new UserConnection { UserId = Context.UserIdentifier, ConnectionId = Context.ConnectionId };
        //            await Clients.OthersInGroup(groupName).SendAsync($"{user.FullName}  Joined", Context.UserIdentifier);
        //            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //            await _smartwasteContext.SaveChangesAsync();
        //        }
        //    }

        public async Task addgroup(string groupName)
        {
            // 1. التأكد من أن الـ UserIdentifier موجود ومسجل (تعديل لـ return فقط)
            if (string.IsNullOrEmpty(Context.UserIdentifier)) return;

            int parsedUserId = int.Parse(Context.UserIdentifier);

            // سحب بيانات المستخدم من الداتا بيز
            var user = await _smartwasteContext.Users.FindAsync(parsedUserId);
            if (user == null) return; // تعديل لـ return فقط

            if (!string.IsNullOrEmpty(groupName))
            {
                // تحقق هل الجروب موجود في الداتا بيز أم لا
                var existingGroup = _smartwasteContext.groups.FirstOrDefault(g => g.Name == groupName);

                if (existingGroup == null)
                {
                    // --- السيناريو الأول: الجروب مش موجود (إنشاء جديد) ---

                    // أ. إنشاء الجروب وحفظه أولاً للحصول على الـ Id
                    var group = new groups { Name = groupName };
                    await _smartwasteContext.groups.AddAsync(group);
                    await _smartwasteContext.SaveChangesAsync(); // حفظ لتوليد الـ ID

                    // ب. إنشاء علاقة اليوزر بالجروب الجديد وحفظها
                    var userInGroup = new UserinGroups
                    {
                        UserId = parsedUserId.ToString(),
                        GroupId = group.Id.ToString()
                    };
                    await _smartwasteContext.UserinGroups.AddAsync(userInGroup); // إضافة للـ Context

                    // ج. حفظ الـ Connection الحالي لليوزر
                    var userConnection = new UserConnection
                    {
                        UserId = parsedUserId.ToString(),
                        ConnectionId = Context.ConnectionId
                    };
                    await _smartwasteContext.UserConnections.AddAsync(userConnection); // إضافة للـ Context

                    // د. حفظ باقي التغييرات في الداتا بيز
                    await _smartwasteContext.SaveChangesAsync();

                    // هـ. إضافة جهاز العميل لجروب الـ SignalR
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                    // و. إبلاغ الجميع بالجروب الجديد
                    await Clients.Group(groupName).SendAsync("GroupCreated", group);
                }
                else
                {
                    // --- السيناريو الثاني: الجروب موجود بالفعل (انضمام فقط) ---

                    // أ. تسجيل الـ Connection لليوزر
                    var userConnection = new UserConnection
                    {
                        UserId = parsedUserId.ToString(),
                        ConnectionId = Context.ConnectionId
                    };
                    await _smartwasteContext.UserConnections.AddAsync(userConnection);
                    await _smartwasteContext.SaveChangesAsync();

                    // ب. إضافة العميل لجروب الـ SignalR
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                    // ج. إبلاغ باقي أعضاء الجروب (Others) بأن اليوزر ده دخل معاهم
                    await Clients.OthersInGroup(groupName).SendAsync("UserJoined", user.FullName, parsedUserId);
                }
            }
        } // 👈 تم إضافة قوس الإغلاق هنا للدالة بنجاح

    }
}
