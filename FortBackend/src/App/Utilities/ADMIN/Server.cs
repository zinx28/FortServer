namespace FortBackend.src.App.Utilities.ADMIN
{
    public class AdminServer
    {
        public static CachedAdminData CachedAdminData { get; set; } = new CachedAdminData();
        public static void Init(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();

            builder.WebHost.UseUrls($"http://0.0.0.0:{Saved.Saved.DeserializeConfig.AdminPort}");

            var app = builder.Build();

            app.UseSession();
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                Console.WriteLine(context.Request.Path);
                if (context.Request.Path.StartsWithSegments("/css") && context.Request.Path.Value.EndsWith(".css"))
                {
                    string cssFileName = Path.GetFileNameWithoutExtension(context.Request.Path.Value);

                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "src/App/Utilities/ADMIN/CSS", cssFileName + ".css");

                    Console.WriteLine(filePath);
                    if (!System.IO.File.Exists(filePath))
                    {
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        string cssContent = System.IO.File.ReadAllText(filePath);
                        
                        Console.WriteLine(cssFileName);
                        context.Response.ContentType = "text/css";
                        await context.Response.WriteAsync(cssContent);
                        

                        
                    }

                   
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Dashboard}/dashboard/{action=Index}/{id?}");
            });

            


            app.Start();
        }
    }
}
