using FortBackend.src.App.SERVER;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;

namespace FortBackend.src.App.XMPP_Server.XMPP
{
    public class XmppServer
    {
        public static void Intiliazation(string[] args)
        {
            Logger.Log("Initializing Xmpp", "Xmpp");


            var builder = WebApplication.CreateBuilder(args);

            #if HTTPS
                builder.WebHost.UseUrls($"https://0.0.0.0:{Saved.DeserializeConfig.XmppPort}");
                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, Saved.DeserializeConfig.XmppPort, listenOptions =>
                    {
                        var certPath = Path.Combine(PathConstants.BaseDir, "src", "Resources", "Certificates", "FortBackend.pfx");
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
                if (context.Request.Path == "//")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        try
                        {
                            var Ip = "";
                            if (Saved.DeserializeConfig.Cloudflare)
                            {
                                Ip = context.Request.Headers["CF-Connecting-IP"];
                            }
                            else
                            {
                                Ip = context.Connection.RemoteIpAddress!.ToString();
                            }

                            
                            string clientId = Guid.NewGuid().ToString();
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            await Handle.HandleWebSocketConnection(webSocket, context.Request, clientId, Ip);
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
                            Logger.Error("XMPP CONNECTION");
                        }
                        else
                        {
                            Logger.Error("TCP CONNECTION");
                        }
                    }

                }
                else if (context.Request.Path == "/clients" && !context.WebSockets.IsWebSocketRequest)
                {
                    var responseObj = new
                    {
                        //Amount = DataSaved_XMPP.connectedClients.Count,
                        Clients = GlobalData.Clients.ToList(),
                        Rooms = GlobalData.Rooms.ToList(),
                    };
                    var jsonResponse = System.Text.Json.JsonSerializer.Serialize(responseObj);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(jsonResponse);
                }else if(context.Request.Path == "/")
                {
                    var responseObj = new
                    {
                        status = "OK"
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

            Logger.Log("XMPP STARTING", "XMPP");
            app.Run();
        }

        public static void STOP()
        {
            Console.WriteLine("XMPP SHUTDOWN");
            foreach (var item in GlobalData.Clients)
            {
                if(item.Game_Client != null)
                {
                    try
                    {
                        item.Game_Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "SERVER SHUTDONW", CancellationToken.None);
                    }
                    catch { }
                }
            }
        }
    }
}
