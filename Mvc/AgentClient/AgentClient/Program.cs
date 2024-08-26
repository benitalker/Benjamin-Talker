using AgentClient.Service;

namespace AgentClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IGeneralService, GeneralService>();
            builder.Services.AddScoped<IMissionService, MissionService>();
            builder.Services.AddScoped<IAgentService, AgentService>();
            builder.Services.AddScoped<ITargetService, TargetService>();
            builder.Services.AddScoped<IMatrixService, MatrixService>();
            builder.Services.AddScoped<ILoginService, LoginService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
