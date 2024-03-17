using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.Websocket;

namespace FortMatchmaker.src.App
{
    public class Service
    {
        public static void Intiliazation(string[] args)
        {
            Console.WriteLine(@"  ______         _   __  __       _       _                     _             
 |  ____|       | | |  \/  |     | |     | |                   | |            
 | |__ ___  _ __| |_| \  / | __ _| |_ ___| |__  _ __ ___   __ _| | _____ _ __ 
 |  __/ _ \| '__| __| |\/| |/ _` | __/ __| '_ \| '_ ` _ \ / _` | |/ / _ \ '__|
 | | | (_) | |  | |_| |  | | (_| | || (__| | | | | | | | | (_| |   <  __/ |   
 |_|  \___/|_|   \__|_|  |_|\__,_|\__\___|_| |_|_| |_| |_|\__,_|_|\_\___|_|   
                                                                              
                                                                              ");

            Logger.Log("MARVELCO MATCHMAKER IS LOADING (marcellowmellow)");
            var builder = WebApplication.CreateBuilder(args);


            var ReadConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "config.json"));
            if (ReadConfig == null)
            {
                Logger.Error("Couldn't find config", "CONFIG");
                throw new Exception("Couldn't find config");
            }

            Saved.DeserializeConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(ReadConfig);
            if (Saved.DeserializeConfig == null)
            {
                Logger.Error("Couldn't deserialize config", "CONFIG");
                throw new Exception("Couldn't deserialize config");
            }
            else
            {
                Logger.Log("Loaded Config", "CONFIG");
            }

#if HTTPS
                Saved.DeserializeConfig.DefaultProtocol = "https://";
                builder.WebHost.UseUrls($"https://0.0.0.0:{Saved.DeserializeConfig.MatchmakerPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, Saved.DeserializeConfig.MatchmakerPort, listenOptions =>
                    {
                        var certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "Certificates", "FortBackend.pfx");
                        if(!File.Exists(certPath)) {
                            Logger.Error("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp", CERTIFICATES);
                            throw new Exception("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp");
                        }
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        var certificate = new X509Certificate2(certPath);
                        listenOptions.UseHttps(certificate);
                    });
                });
            #else
                Saved.DeserializeConfig.DefaultProtocol = "http://";
                builder.WebHost.UseUrls($"http://0.0.0.0:{Saved.DeserializeConfig.MatchmakerPort}");
            #endif


            var app = builder.Build();


           #if HTTPS
                app.UseHttpsRedirection();
            #endif

            app.UseWebSockets();

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
                                Console.WriteLine("CONNECTED!");
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

                //app.UseRouting();



                app.Run();

        }
    }
}
