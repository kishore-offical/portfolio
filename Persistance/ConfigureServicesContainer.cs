using ChatWe.Persistance.Context;
using ChatWe.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatWe.Persistance
{
    public static class ConfigureServicesContainer
    {
        public static void Startup(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddControllers();
            services.AddHttpContextAccessor();
            AddDBContext(services, configuration);

            AddIdentityService(services);

            AddScopedServices(services);
        }

        public static void AddDBContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChatWeContext>(opts =>
                    opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddIdentityService(IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ChatWeContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Account/Login";
                        options.LogoutPath = "/Account/Logout";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    });
        }

        public static void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<IChatWeContext, ChatWeContext>();
        }
    }
}