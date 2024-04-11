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

namespace FortBackend.src.App.SERVER
{
    public class Handle
    {

        public static async Task HandleWebSocketConnection(WebSocket webSocket, HttpRequest context, string clientId)
        {
            string receivedMessage = ""; // so skunky but works fine
            string AccountId = ""; // for both clients to know the main
            bool DidUserLoginNotSure = false; // for game only
            try
            {
                //DataSaved_XMPP.connectedClients.TryAdd(clientId, webSocket); // Adds The data inside the handlewebsocekt!
                var buffer = new byte[0];
                XDocument xmlDoc;

                while (webSocket.State == WebSocketState.Open)
                {
                    buffer = new byte[1024];
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    while (!result.CloseStatus.HasValue)
                    {
                        string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        receivedMessage += chunk;
                        if (result.EndOfMessage)
                        {
                            Console.WriteLine("Received WebSocket message: " + receivedMessage);
                            JToken test = "";
                            try
                            {
                                test = JToken.Parse(receivedMessage);
                                Console.WriteLine("DISCONNETING USER AS FAKE RESPONSE // JSON");
                                receivedMessage = "";
                                return;
                            }
                            catch { }
                            xmlDoc = XDocument.Parse(receivedMessage);

                            switch (xmlDoc.Root?.Name.LocalName)
                            {
                                // LOGIN IS USED BY THE LUNA LAUNCHER THIS WILL NOT WORK WITH OTHERS
                                case "login":
                                    Login.Init(webSocket, xmlDoc, clientId);
                                    break;
                                case "open":
                                    Open.Init(webSocket, DidUserLoginNotSure, clientId);
                                    break;
                                case "auth":
                                    Auth.Init(webSocket, xmlDoc, clientId, AccountId);
                                    break;
                                case "iq":
                                    Iq.Init(webSocket, xmlDoc, clientId, AccountId);
                                    break;
                                case "message":
                                    Message.Init(webSocket, xmlDoc, clientId, AccountId);
                                    break;
                                case "presence":
                                    Presence.Init(webSocket, xmlDoc, clientId, AccountId);
                                    break;
                                default: break;
                            }

                            receivedMessage = "";
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
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("XMPP CLOSE");
                try
                {
                    if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Too Late Buddy I Turned You Off", CancellationToken.None);
                    }

                    webSocket.Dispose();
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocketException for client: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                try
                {
                  //  DataSaved_XMPP.connectedClients.TryRemove(clientId, out _);

                    Clients launcher_client = GlobalData.Clients.FirstOrDefault(c => c.Launcher_Client == webSocket)!;
                    if (launcher_client != null)
                    {
                        if(launcher_client.Game_Client != null)
                            KillGame.Init(launcher_client, launcher_client.DataSaved);
          
                        GlobalData.Clients.Remove(launcher_client);
                    }
                    else
                    {
                        Clients client = GlobalData.Clients.FirstOrDefault(c => c.Game_Client == webSocket)!;

                        if (client != null)
                            KillGame.Init(client, client.DataSaved);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

        }
        }
    }
}
