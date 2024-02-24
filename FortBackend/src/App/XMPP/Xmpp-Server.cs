using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;

namespace FortBackend.src.App.XMPP
{
    public class Xmpp_Server
    {
        public static async Task Intiliazation(string[] args)
        {
            Logger.Log("Initializing Xmpp", "Xmpp");
            var builder = WebApplication.CreateBuilder(args);

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

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                if(context.Request.Path == "//" && context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine("XMPP IS BEING CLALED");
                }else
                {
                    await next();
                }
            });

            app.Run();
        }
     }
}
