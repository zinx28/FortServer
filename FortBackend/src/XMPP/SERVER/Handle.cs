using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.XMPP.Data;
using FortBackend.src.App.SERVER.Root;
using FortBackend.src.App.SERVER.Send;
using FortLibrary.XMPP;
using FortBackend.src.XMPP.SERVER;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities;
using FortLibrary;

namespace FortBackend.src.App.SERVER
{
    public class Handle
    {

        public static async Task HandleWebSocketConnection(WebSocket webSocket, HttpRequest context, string clientId, string IP, CancellationToken serverShutdownToken)
        {
            string receivedMessage = ""; // so skunky but works fine
            //string AccountId = ""; // for both clients to know the main
           // bool DidUserLoginNotSure = false; // for game only
            DataSaved UserDataSaved = new();
            try
            {
                //DataSaved_XMPP.connectedClients.TryAdd(clientId, webSocket); // Adds The data inside the handlewebsocekt!
                var buffer = new byte[0];
                XDocument xmlDoc = null;

                while (!serverShutdownToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
                {
                    buffer = new byte[1024];
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, serverShutdownToken);
                    while (!result.CloseStatus.HasValue)
                    {
                        string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        receivedMessage += chunk;
                        if (result.EndOfMessage)
                        {
                            //Console.WriteLine("Received WebSocket message: " + receivedMessage);
                            JToken test = "";
                            try
                            {
                                test = JToken.Parse(receivedMessage);
                                Logger.Error("DISCONNETING USER AS FAKE RESPONSE // JSON", "[XMPP]");
                                receivedMessage = "";
                                return;
                            }
                            catch { }

                            try
                            {
                                xmlDoc = XDocument.Parse(receivedMessage);
                            }
                            catch { }

                            if (xmlDoc != null)
                            {

                                //Console.WriteLine(xmlDoc.Root?.Name.LocalName);
                                switch (xmlDoc.Root?.Name.LocalName)
                                {
                                    // LOGIN IS USED BY THE LUNA LAUNCHER THIS WILL NOT WORK WITH OTHERS
                                    // case "login":
                                    //Login.Init(webSocket, xmlDoc, clientId, IP);
                                    // break;
                                    case "open":
                                        OpenHandler.Init(webSocket, UserDataSaved, clientId);
                                        break;
                                    case "auth":
                                        AuthHandler.Init(webSocket, xmlDoc, clientId, UserDataSaved, IP);
                                        break;
                                    case "iq":
                                        IqHandler.Init(webSocket, xmlDoc, clientId, UserDataSaved);
                                        break;
                                    case "message":
                                        MessageHandler.Init(webSocket, xmlDoc, clientId, UserDataSaved);
                                        break;
                                    case "presence":
                                        PresenceHandler.Init(webSocket, xmlDoc, clientId, UserDataSaved);
                                        break;
                                    default: break;
                                }


                                ClientFix.Init(webSocket, UserDataSaved, clientId);

                                receivedMessage = "";
                            }


                        }
                        break;
                    }
                }

                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket closed by client.", CancellationToken.None);
                }
            }

            catch (WebSocketException ex)
            {
                Logger.Error(ex.Message);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            finally
            {
                Logger.PlainLog("XMPP CLOSE");
                try
                {
                    if (!serverShutdownToken.IsCancellationRequested && webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Too Late Buddy I Turned You Off", CancellationToken.None);
                    }

                    webSocket.Dispose();
                }
                catch (WebSocketException ex)
                {
                    Logger.Error($"WebSocketException for client: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
                try
                {
                    Clients client = GlobalData.Clients.FirstOrDefault(c => c.Game_Client == webSocket)!;

                    if (client != null)
                    {
                        await KillGame.Init(client, UserDataSaved);

                        GlobalData.Clients.Remove(client);
                    }
                    else
                    {
                        Logger.Error("CLIENT IS NOT FOUND WTFFFF");
                    }

                    //}
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, "XMPP");
                }

            }
        }
    }
}
