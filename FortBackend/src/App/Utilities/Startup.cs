using FortBackend.src.App.Utilities.MongoDB;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace FortBackend.src.App.Utilities
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            MongoDBStart.Initialize(services, Configuration);
        }
    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            Logger.Log("Loading all endpoints");

            app.UseEndpoints(endpoints =>
            {
                var IActionDescriptorCollectionProvider = app.ApplicationServices.GetRequiredService<IActionDescriptorCollectionProvider>();
                var ActionDescriptors = IActionDescriptorCollectionProvider.ActionDescriptors.Items;

                foreach (var actionDescriptor in ActionDescriptors)
                {
                    if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        var route = actionDescriptor.AttributeRouteInfo?.Template ?? actionDescriptor.RouteValues["action"];
                        var controller = actionDescriptor.RouteValues["controller"];
                       
                        var HttpMethod = controllerActionDescriptor.MethodInfo
                        .GetCustomAttributes(true)
                        .OfType<HttpMethodAttribute>()
                        .SelectMany(attr => attr.HttpMethods)
                        .Distinct();


                        Logger.Log($"/{route}", string.Join(",", HttpMethod));
                    }
                }

                endpoints.MapControllers();
            });

            Logger.Log("Done Loading");

            app.UseStatusCodePages(async (StatusCodeContext context) =>
            {
                Logger.Warn($"[{context.HttpContext.Request.Method}]: {context.HttpContext.Request.Path.ToString()}?{context.HttpContext.Request.Query}");
            });
        }
    }

}
