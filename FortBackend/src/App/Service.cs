using System.Net;
using FortBackend.src.App.Utilities.Saved;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Routes;
using System.Runtime.InteropServices;
using FortBackend.src.App.Utilities.Discord;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Shop;
using FortBackend.src.App.XMPP;
namespace FortBackend.src.App
{
    public class Service
    {
        public static void Intiliazation(string[] args)
        {
            Console.WriteLine(@"  ______         _   ____             _                  _ 
 |  ____|       | | |  _ \           | |                | |
 | |__ ___  _ __| |_| |_) | __ _  ___| | _____ _ __   __| |
 |  __/ _ \| '__| __|  _ < / _` |/ __| |/ / _ \ '_ \ / _` |
 | | | (_) | |  | |_| |_) | (_| | (__|   <  __/ | | | (_| |
 |_|  \___/|_|   \__|____/ \__,_|\___|_|\_\___|_| |_|\__,_|
                                                           
                                                           ");

            Logger.Log("MARVELCO IS LOADING (marcellowmellow)");
            Logger.Log($"Built on {RuntimeInformation.OSArchitecture}-bit");
            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);
         

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
            }else
            {
                Logger.Log("Loaded Config", "CONFIG");
            }
            
            startup.ConfigureServices(builder.Services);
        #if HTTPS
                Saved.DeserializeConfig.DefaultProtocol = "https://";
                builder.WebHost.UseUrls($"https://0.0.0.0:{Saved.DeserializeConfig.BackendPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, Saved.DeserializeConfig.BackendPort, listenOptions =>
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
                builder.WebHost.UseUrls($"http://0.0.0.0:{Saved.DeserializeConfig.BackendPort}");
            #endif


            var app = builder.Build();

            #if HTTPS
                app.UseHttpsRedirection();
            #endif
          
            app.UseRouting();

            startup.Configure(app, app.Environment);
            //Setup.Initialize(app);
            DiscordBot.Start(); // dont away... app.run does it for you

            var XmppServer = new Thread(async () =>
            {
                await Xmpp_Server.Intiliazation(args);
            });
            XmppServer.Start();

            var TCPXmppServer = new Thread(async () =>
            { 
                TcpServer testserver = new TcpServer(Saved.DeserializeConfig.TCPXmppPort);
                Task tcpServerTask = testserver.Start();
            });
            TCPXmppServer.Start();

            //var ItemShopGenThread = new Thread(async () =>
            //{
            //    await GenerateItemShop(0);
            //});
            //ItemShopGenThread.Start();

            //GenerateShop.Init();

            app.Run();

        }

        async static Task GenerateItemShop(int i)
        {
            await Task.Delay(1000);
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            if (dateTime.Hour == 17 && dateTime.Minute == 59)
            {
                if (dateTime.Second >= 59)
                {
                    var GeneraterShocked = new Thread(async () => {
                        await GenerateShop.Init();
                    });

                    GeneraterShocked.Start();
                }
            }
            GenerateItemShop(++i);
        }
    }
}
