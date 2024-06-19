using System.Net;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities;
using System.Runtime.InteropServices;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Shop;
using FortBackend.src.App.XMPP_V2;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers;
using FortLibrary.EpicResponses.Profile.Query.Items;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using FortBackend.src.App.Utilities.Discord;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortLibrary.ConfigHelpers;
using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
namespace FortBackend.src.App
{
    public class Service
    {
        public static async void Intiliazation(string[] args)
        {
            Logger.PlainLog(@"  ______         _   ____             _                  _ 
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

            await CachedData.Init();

            startup.ConfigureServices(builder.Services);




#if HTTPS
                Saved.BackendCachedData.DefaultProtocol = "https://";
                builder.WebHost.UseUrls($"https://0.0.0.0:{Saved.DeserializeConfig.BackendPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, Saved.DeserializeConfig.BackendPort, listenOptions =>
                    {
                        var certPath = Path.Combine(PathConstants.BaseDir, "src", "Resources", "Certificates", "FortBackend.pfx");
                        if(!File.Exists(certPath)) {
                            Logger.Error("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp", "CERTIFICATES");
                            throw new Exception("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp");
                        }
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        var certificate = new X509Certificate2(certPath);
                        listenOptions.UseHttps(certificate);
                    });
                });
#else
                Saved.BackendCachedData.DefaultProtocol = "http://";
                builder.WebHost.UseUrls($"http://0.0.0.0:{Saved.DeserializeConfig.BackendPort}");
            #endif


            var app = builder.Build();

           

            // Fix ips not showing
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            //    ForwardedHeaders.XForwardedProto
            //});

#if HTTPS
                app.UseHttpsRedirection();
#endif

            app.UseRouting();

            startup.Configure(app, app.Environment);

          


            //Setup.Initialize(app);
#if !DEVELOPMENT
            var DiscordBotServer = new Thread(async () =>
            {
                await DiscordBot.Start(); // dont away... app.run does it for you
            });
            DiscordBotServer.Start();
#endif

          
            var XmppServer = new Thread(() =>
            {
                Xmpp_Server.Intiliazation(args);

               // FortBackend.src.App.XMPP_Server.XMPP.XmppServer.STOP();
            });
            XmppServer.Start();

            var LeadBoardLoop = new Thread(async () =>
            {
                await UpdateLeaderBoard.LeaderboardLoop();
            });
            LeadBoardLoop.Start();

            //var ItemShopGenThread = new Thread(async () =>
            //{
            //    await GenerateItemShop(0);
            //});
            //ItemShopGenThread.Start();

            //GenerateShop.Init();

        

            await app.StartAsync();

            var shutdownTask = app.WaitForShutdownAsync();

        
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                
                //XmppServer.Join(); ~ i can't trust 
             
                CacheMiddleware.ShutDown().Wait();

                Logger.Close();
                Console.WriteLine("Done");
            };
            //app.Run();
            await shutdownTask;

            Console.WriteLine("SHUTDOINW");
            FortBackend.src.App.XMPP_Server.XMPP.XmppServer.STOP();
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
            await GenerateItemShop(++i);
        }
    }
}
