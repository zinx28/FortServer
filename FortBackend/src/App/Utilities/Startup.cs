using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
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

            services.AddCors(options =>
            {
                if (Saved.Saved.DeserializeConfig.AllowAllCores)
                {
                    options.AddPolicy("AllowAllWithCredentials", policy =>
                    {
                        policy.SetIsOriginAllowed(_ => true)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                }
                else
                {
                    options.AddPolicy("DynamicCORS", policy =>
                    {
                        policy.WithOrigins("http://localhost:2222", Saved.Saved.DeserializeConfig.DashboardUrl)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                }
            });

            MongoDBStart.Initialize(services, Configuration);

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

            if (Saved.Saved.DeserializeConfig.LogLevel > 1)
            {
                Logger.Log("Logs are enabled", "SETUP");
                app.UseMiddleware<LoggingMiddleware>();
            }

            if (Saved.Saved.DeserializeConfig.AllowAllCores)
                app.UseCors("AllowAllWithCredentials");
            else
                app.UseCors("DynamicCORS");

            app.UseRouting();

            Logger.Log("Loading all endpoints");

            app.Use(async (context, next) =>
            {
                await next();

                var response = context.Response;
                if (response.StatusCode == (int)HttpStatusCode.MethodNotAllowed)
                {
                    response.ContentType = "application/json";
                    await response.WriteAsJsonAsync(new
                    {
                        errorCode = "errors.com.epicgames.common.method_not_allowed",
                        errorMessage = "Sorry the resource you were trying to access cannot be accessed with the HTTP method you used.",
                        numericErrorCode = 1009,
                        originatingService = "Fortnite",
                        intent = "prod"
                    });
                }
            });

            app.UseEndpoints(endpoints =>
            {
                int MappedNum = 0;
                var actionDescriptors = app.ApplicationServices.GetRequiredService<IActionDescriptorCollectionProvider>().ActionDescriptors.Items;
                
                foreach (var actionDescriptor in actionDescriptors)
                {
                    if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                    {
                        var controllerNamespace = controllerActionDescriptor.ControllerTypeInfo.Namespace;
                       
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

                        MappedNum += 1;
                    }
                }

                Logger.Log($"Mapped {MappedNum}/{actionDescriptors.Count()}", "Mapped");
            });

         

            app.UseStatusCodePages(async (StatusCodeContext context) =>
            {
                var response = context.HttpContext.Response;
                Logger.Warn($"{context.HttpContext.Request.Path.ToString()}?{context.HttpContext.Request.Query}", context.HttpContext.Request.Method);
               
                if (context.HttpContext.Request.Path == "/")
                {
                    context.HttpContext.Request.ContentType = "application/json";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await response.WriteAsJsonAsync(new
                    {
                        status = "OK"
                    });
                }
                if (response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await response.WriteAsJsonAsync(new
                    {
                        errorCode = "errors.com.epicgames.common.not_found",
                        errorMessage = "Sorry the resource you were trying to find could not be found",
                        numericErrorCode = 1004,
                        originatingService = "Fortnite",
                        intent = "prod"
                    });
                }
            });

            await GrabAdminData.GrabAllAdmin(); // cache service needs to be alive for this to work!

            Logger.Log("Done Loading");
        }
    }

}
