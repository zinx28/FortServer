using FortLibrary;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.Utilities.MongoDB;
using FortMatchmaker.src.App.Websockets;
using FortMatchmaker.src.App.WebSockets.Helpers;
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


namespace FortMatchmaker.src.App.Utilities
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

            new Thread(async () =>
            {
                await Breathe.Start();
            }).Start();
          
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "//")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        string clientId = Guid.NewGuid().ToString();
                        try
                        {
                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                            using (webSocket)
                            {
                                Console.WriteLine("User Connected!");
                                await NewConnection.Init(webSocket, context.Request, clientId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, "MATCHMAKER");
                            context.Response.StatusCode = 500;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
                else if (context.Request.Path == "/clients")
                {
                    if (!context.WebSockets.IsWebSocketRequest)
                    {
                        context.Response.ContentType = "application/json";
                        var Response = new
                        {
                            Amount = MatchmakerData.connected.Count,
                            Queuing = MatchmakerData.SavedData.Values.GroupBy(UserData => UserData.Region).ToDictionary(f => f.Key, f => f.Count())
                        };

                        var ChangeToString = System.Text.Json.JsonSerializer.Serialize(Response);
                        await context.Response.WriteAsync(ChangeToString);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
                else if (context.Request.Path == "/v1/devers/hotfixes")
                {
                    context.Response.ContentType = "application/json";
                    var Servers = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources\\json\\HotFixer_V2.json"));

                    await context.Response.WriteAsync(Servers);
                }
                else if (context.Request.Path == "/v1/devers/servers")
                {
                    context.Response.ContentType = "application/json";
                    var Servers = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Resources\\json\\servers.json"));

                    await context.Response.WriteAsync(Servers);
                }
                else
                {
                    await next();
                }
            });

            app.UseStatusCodePages(async (context) =>
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

            Logger.Log("Done Loading");
        }
    }

}
