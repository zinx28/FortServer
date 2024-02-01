using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            services.AddControllers();
        }
    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            Console.WriteLine("Loading all endpoints");

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


                        Console.WriteLine($"[{string.Join(",", HttpMethod)}]: /{route}");
                    }
                }

                endpoints.MapControllers();
            });

            Console.WriteLine("Done Loading");

            app.UseStatusCodePages(async (StatusCodeContext context) =>
            {
                Console.WriteLine($"[{context.HttpContext.Request.Method}]: {context.HttpContext.Request.Path.ToString()}");
            });
        }
    }

}
