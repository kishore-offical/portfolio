using ChatWe.Hubs;
using Microsoft.Extensions.FileProviders;

namespace ChatWe.Persistance
{
    public static class ConfigureContainer
    {
        public static async Task Startup(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapFallbackToFile("index.html");
            });
            app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Assets")), RequestPath = "/Assets" });
        }
    }
}