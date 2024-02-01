using System.Net;
using FortBackend.src.App.Utilities.Saved;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Routes;
using System.Runtime.InteropServices;

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

            Console.WriteLine("MARVELCO IS LOADING (marcellowmellow)");
            var builder = WebApplication.CreateBuilder(args);
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var ReadConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "config.json"));
            if (ReadConfig == null)
            {
                Console.WriteLine("Returning... temp response");
                throw new Exception("Couldn't find config");
            }

            Config DeserializeConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(ReadConfig);
            if (DeserializeConfig == null)
            {
                Console.WriteLine("Returning... temp response");
                throw new Exception("Couldn't deserialize config");
            }

            Console.WriteLine($"Built on {RuntimeInformation.OSArchitecture}-bit");
            #if HTTPS
            builder.WebHost.UseUrls($"https://0.0.0.0:{DeserializeConfig.BackendPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, DeserializeConfig.BackendPort, listenOptions =>
                    {
                        var certPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "Certificates", "FortBackend.pfx");
                        if(!File.Exists(certPath)) {
                            Console.WriteLine("Returning... temp response");
                            throw new Exception("Couldn't find FortBackend.pfx -> make sure you removed .temp from FortBackend.pfx.temp");
                        }
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        var certificate = new X509Certificate2(certPath);
                        listenOptions.UseHttps(certificate);
                    });
                });
            #else
            builder.WebHost.UseUrls($"http://0.0.0.0:{DeserializeConfig.BackendPort}");
            #endif


            var app = builder.Build();

            #if HTTPS
                app.UseHttpsRedirection();
            #endif

            app.UseRouting();

            startup.Configure(app, app.Environment);
            //Setup.Initialize(app);

            app.Run();

        }
    }
}
