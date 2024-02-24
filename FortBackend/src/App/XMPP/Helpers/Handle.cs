using FortBackend.src.App.XMPP.Helpers.Resources;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.App.XMPP.Root;

namespace FortBackend.src.App.XMPP.Helpers
{
    public class Handle
    {

        public static async Task HandleWebSocketConnection(WebSocket webSocket, HttpRequest context, string clientId)
        {

            DataSaved dataSaved = new DataSaved(); // outside the try wow!
            try
            {
                DataSaved.connectedClients.TryAdd(clientId, webSocket); // Adds The data inside the handlewebsocekt!
                var buffer = new byte[0];
                XDocument xmlDoc;

                while (webSocket.State == WebSocketState.Open)
                {
                    buffer = new byte[1024];
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    while (!result.CloseStatus.HasValue)
                    {
                        string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        dataSaved.receivedMessage += chunk;
                        if (result.EndOfMessage)
                        {
                            Console.WriteLine("Received WebSocket message: " + dataSaved.receivedMessage);
                            JToken test = "";
                            try
                            {
                                test = JToken.Parse(dataSaved.receivedMessage);
                                Console.WriteLine("DISCONNETING USER AS FAKE RESPONSE // JSON");
                                dataSaved.receivedMessage = "";
                                return;
                            }
                            catch (JsonReaderException ex)
                            {
                                //return; // wow
                            }
                            xmlDoc = XDocument.Parse(dataSaved.receivedMessage);

                            switch (xmlDoc.Root?.Name.LocalName)
                            {
                                case "open":
                                    Open.Init(webSocket, dataSaved.DidUserLoginNotSure, clientId); // Pov Client Is Used For Anything (Not Gonna be skunky and use a random id)
                                    break;
                                case "auth":
                                    Auth.Init(webSocket, xmlDoc, clientId, dataSaved);
                                    break;
                                case "iq":
                                    Iq.Init(webSocket, xmlDoc, clientId, dataSaved);
                                    break;
                                case "message":
                                    Message.Init(webSocket, xmlDoc, clientId, dataSaved);
                                    break;
                                case "presence":
                                    Presence.Init(webSocket, xmlDoc, clientId, dataSaved);
                                    break;
                                default: break;
                            }

                            //smth else
                            dataSaved.receivedMessage = "";
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
            }
        }
    }
}
