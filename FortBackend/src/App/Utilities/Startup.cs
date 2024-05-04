using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Net;


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

            MongoDBStart.Initialize(services, Configuration); // better if this was a new MongoDBStart();

            services.AddSingleton<CacheMiddleware>();
            services.AddHostedService<CacheMiddleware>();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddControllersWithViews();
            services.AddControllers();
        }
    
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #if DEBUG
                app.UseMiddleware<LoggingMiddleware>();
            #endif

           

            app.UseRouting();

            Logger.Log("Loading all endpoints");

            app.UseEndpoints(endpoints =>
            {
               
                var actionDescriptors = app.ApplicationServices.GetRequiredService<IActionDescriptorCollectionProvider>().ActionDescriptors.Items;
                foreach (var actionDescriptor in actionDescriptors)
                {
                    if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        var controllerNamespace = controllerActionDescriptor.ControllerTypeInfo.Namespace;
                        
                        if (controllerNamespace != null && controllerNamespace.Contains("App.Routes"))
                        {
                            //Console.WriteLine(controllerNamespace);
                            var route = actionDescriptor.AttributeRouteInfo?.Template ?? actionDescriptor.RouteValues["action"];
                            var controller = actionDescriptor.RouteValues["controller"];

                            var HttpMethod = controllerActionDescriptor.MethodInfo
                            .GetCustomAttributes(true)
                            .OfType<HttpMethodAttribute>()
                            .SelectMany(attr => attr.HttpMethods)
                            .Distinct();

                             Logger.Log($"/{route}", string.Join(",", HttpMethod));

                        
                                endpoints.MapControllerRoute(
                                    name: $"{controller}",
                                    pattern: $"/{route}",
                                    defaults: new { controller = controller }
                             );
                            
                             


                            Logger.Log($"/{route}", "Mapped");
                        }
                    }
                }


                //endpoints.MapControllers();
            });

            await GrabAdminData.GrabAllAdmin(); // cache service needs to be alive for this to wrok!

            Logger.Log("Done Loading");

            app.UseStatusCodePages(async (StatusCodeContext context) =>
            {
                var response = context.HttpContext.Response;
                Logger.Warn($"[{context.HttpContext.Request.Method}]: {context.HttpContext.Request.Path.ToString()}?{context.HttpContext.Request.Query}");
                if (context.HttpContext.Request.Path == "/")
                {
                    var responseObj = new
                    {
                        status = "OK"
                    };
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseObj);
                    context.HttpContext.Request.ContentType = "application/json";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await response.WriteAsync(jsonResponse);
                }
                if (response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await response.WriteAsJsonAsync(new
                    {
                        errorCode = "errors.com.epicgames.common.not_found",
                        errorMessage = "Sorry the resource you were trying to find could not be found",
                        numericErrorCode = 0,
                        originatingService = "Fortnite",
                        intent = "prod"
                    });
                }
            });
        }
    }

}
