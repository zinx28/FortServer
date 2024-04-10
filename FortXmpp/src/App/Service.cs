using FortXmpp.src.App.Globals;
using FortXmpp.src.App.Globals.Data;
using FortXmpp.src.App.SERVER;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace FortXmpp.src.App
{
    public class Service
    {
        public static async void Intiliazation(string[] args)
        {
            Console.WriteLine(@"  _____          _  __  __                      
 |  ___|__  _ __| |_\ \/ /_ __ ___  _ __  _ __  
 | |_ / _ \| '__| __|\  /| '_ ` _ \| '_ \| '_ \ 
 |  _| (_) | |  | |_ /  \| | | | | | |_) | |_) |
 |_|  \___/|_|   \__/_/\_\_| |_| |_| .__/| .__/ 
                                   |_|   |_|    
            ");


            var builder = WebApplication.CreateBuilder(args);

            var ReadConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "config.json"));
            if (ReadConfig == null)
            {
                Console.WriteLine("Couldn't find config", "CONFIG");
                throw new Exception("Couldn't find config");
            }

            Saved.DeserializeConfig = JsonConvert.DeserializeObject<Config>(ReadConfig)!;
            if (Saved.DeserializeConfig == null)
            {
                Console.WriteLine("Couldn't deserialize config", "CONFIG");
                throw new Exception("Couldn't deserialize config");
            }
            else
            {
                Console.WriteLine("Loaded Config", "CONFIG");
            }

#if HTTPS
                builder.WebHost.UseUrls($"https://0.0.0.0:{Saved.DeserializeConfig.XmppPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, Saved.DeserializeConfig.XmppPort, listenOptions =>
                    {
                        var certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "Certificates", "FortBackend.pfx");
                        if(!File.Exists(certPath)) {
                            Logger.Error("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp");
                            throw new Exception("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp");
                        }
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        var certificate = new X509Certificate2(certPath);
                        listenOptions.UseHttps(certificate);
                    });
                });
#else
            builder.WebHost.UseUrls($"http://0.0.0.0:{Saved.DeserializeConfig.XmppPort}");
#endif


            var app = builder.Build();

#if HTTPS
                app.UseHttpsRedirection();
#endif

            app.UseWebSockets();


            app.Use(async (context, next) =>
            {
                Console.WriteLine("TEST!! THIS COULD BE NEW CONNECTION BUT IDK");
                if (context.Request.Path == "//")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        Console.WriteLine("XMPP IS BEING CLALED");
                        try
                        {
                            string clientId = Guid.NewGuid().ToString();
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await Handle.HandleWebSocketConnection(webSocket, context.Request, clientId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("OH SUGAR :/ it crashed why -> " + ex.Message);
                        }
                    }
                    else
                    {
                        if (context.Request.Headers.TryGetValue("Upgrade", out var upgradeHeader) &&
                        string.Equals(upgradeHeader, "websocket", StringComparison.OrdinalIgnoreCase) &&
                        context.Request.Headers.TryGetValue("Connection", out var connectionHeader) &&
                        string.Equals(connectionHeader, "Upgrade", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("XMPP CONNECTION");
                        }
                        else
                        {
                            Console.WriteLine("TCP CONNECTION");
                        }
                    }

                }
                else if (context.Request.Path == "/clients" && !context.WebSockets.IsWebSocketRequest)
                {
                    var responseObj = new
                    {
                        Amount = DataSaved_XMPP.connectedClients.Count,
                        Clients = GlobalData.Clients.ToList(),
                        Rooms = GlobalData.Rooms.ToList(),
                    };
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseObj);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResponse);
                }
                else
                {
                    await next();
                }
            });

            Console.WriteLine("XMPP STARTING", "XMPP");
            app.Run();

        }
    }
}