using System.Net;
using FortBackend.src.App.Utilities.Saved;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

namespace FortBackend.src.App
{
    public class Service
    {
        public static void Intiliazation(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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

            app.Run();

        }
    }
}
