using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using SmartWaste.Controllers;
using SmartWaste.Models;
using SmartWaste.Repositories;
using SmartWaste.Services;
using System.Text;
using System.Xml.Schema;
namespace SmartWaste
{
    public class Program
    {
        public static void Main(string[] args)
        {


            // Add services to the container.
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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
            builder.Services.AddScoped<IPickupRequestService, PickupRequestService>();
            builder.Services.AddScoped<IRequestItemService, RequestItemService>();
            builder.Services.AddScoped<IRequestItemRepository, RequestItemRepository>();
            builder.Services.AddScoped<ISupportTicketsRepository, SupportTicketsRepository>();
            builder.Services.AddScoped<ISupportTicketsServices, SupportTicketsServices>();
            builder.Services.AddScoped<IAdminRepository , AdminRepository>();  
            builder.Services.AddScoped<IAuthServices, AuthServices>();

           builder.Services.AddAuthentication(op=>op.DefaultAuthenticateScheme="MySchema")
.AddJwtBearer("MySchema", options =>
{
    string securityKey = "this is my custom Secret key for authentication";
    var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = symmetricSecurityKey,
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
};
});

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddDbContext<smartwasteContext>(option=>
            option.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "SmartWaste System - V1",
                    Version = "v1", 
                Description = "A sample API to demo SmartWaste System",
                    Contact = new OpenApiContact
                    {
                        Name = "AbdElrhman Mahmoud , Aya Hossam",
                        Url= new Uri("https://www.linkedin.com/in/abdelrahmanalashmouni"),

                Email = "abdelrhmanmahmoud0106@gmail.com",
               
                }
                    
                }
                );
            });


            builder.Services.AddEndpointsApiExplorer();

                        builder.Services.AddSwaggerGen();

            var app = builder.Build();

                app.UseSwagger();
                app.UseSwaggerUI();
                        if (app.Environment.IsDevelopment())
                         {

            }
            ;

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

                        //app.MapUserEndpoints();

            app.Run();
        }
    }
}
