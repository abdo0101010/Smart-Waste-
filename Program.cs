using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartWaste.Models;
using SmartWaste.Repositories;
using SmartWaste.Services;
using System.Text;

namespace SmartWaste
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. تفعيل الـ Controllers ومعالجة الـ Loops في الـ JSON
            builder.Services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // 2. تسجيل الـ Repositories والـ Services (Dependency Injection)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IWasteCategoryRepository, WasteCategoryRepository>();
            builder.Services.AddScoped<IRecyclerRepository, RecyclerRepository>();
            builder.Services.AddScoped<IRewardRepository, RewardRepository>();
            builder.Services.AddScoped<IHubStaffRepository, HubStaffRepository>();
            builder.Services.AddScoped<IPickupRequestRepository, PickupRequestRepository>();
            builder.Services.AddScoped<IPickupRequestService, PickupRequestService>();
            builder.Services.AddScoped<IHubStaffService, HubStaffService>();
            builder.Services.AddScoped<IRecyclerService, RecyclerService>();
            builder.Services.AddScoped<IRewardService, RewardService>();
            builder.Services.AddScoped<IWasteCategoryService, WasteCategoryService>();
            builder.Services.AddScoped<IRequestItemService, RequestItemService>();
            builder.Services.AddScoped<IRequestItemRepository, RequestItemRepository>();
            builder.Services.AddScoped<ISupportTicketsRepository, SupportTicketsRepository>();
            builder.Services.AddScoped<ISupportTicketsServices, SupportTicketsServices>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IEcoSnapService, EcoSnapService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IForgetPasswordRepository, ForgetPasswordRepository>();
            builder.Services.AddScoped<IForgetPasswordService, ForgetPasswordService>();
            builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddSignalR();
            // 3. إعدادات الـ Authentication والـ JWT Token
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme = "MySchema")
            .AddJwtBearer("MySchema", options =>
            {
                string securityKey = "this is my custom Secret key for authentication";
                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });

            // 4. الاتصال بالـ Database
            builder.Services.AddDbContext<smartwasteContext>(option =>
                option.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // 5. إعدادات الـ Swagger وحل مشكلة الـ Namespaces والتضارب
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations(); // تفعيل الـ Annotations لقراءة الـ Attributes
                c.CustomSchemaIds(type => type.FullName); // منع تضارب الـ DTOs المتشابهة

                c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "SmartWaste System - V1",
                    Version = "v1",
                    Description = "A sample API to demo SmartWaste System",
                    Contact = new Microsoft.OpenApi.OpenApiContact
                    {
                        Name = "AbdElrhman Mahmoud , Aya Hossam",
                        Url = new Uri("https://www.linkedin.com/in/abdelrahmanalashmouni"),
                        Email = "abdelrhmanmahmoud0106@gmail.com",
                    }
                });
            });

            // 6. سياسة الـ CORS لمنع إيرور الـ Failed to fetch
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });

            var app = builder.Build();

            // 7. ترتيب الـ Middleware Pipeline
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartWaste API V1"));

            app.UseHttpsRedirection();

            // تأمين الـ wwwroot برمجياً منعاً لأي كراش
            string wwwrootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            app.UseStaticFiles();

            // 🔥 الـ CORS في مكانه السحري قبل الـ Auth والـ Controllers
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<SmartWaste.Hubs.NotificationHub>("/notificationHub");
            app.Run();
        }
    }
}