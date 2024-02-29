using ChatWe.Persistance;

namespace ChatWe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            ConfigureServicesContainer.Startup(builder.Services, builder.Configuration);

            var app = builder.Build();

            app.MapControllers();
            app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=Login}");

            ConfigureContainer.Startup(app).GetAwaiter().GetResult();

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}