using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.XMPP_Server.XMPP.Helpers.Send;
using FortBackend.src.App.XMPP_Server.Helpers.Globals.Data;
using FortBackend.src.App.XMPP_Server.Globals;
using FortBackend.src.App.XMPP_Server.XMPP.Root;

namespace FortBackend.src.App.XMPP_Server.XMPP.Helpers
{
    public class Handle
    {

        public static async Task HandleWebSocketConnection(WebSocket webSocket, HttpRequest context, string clientId)
        {

            DataSaved_XMPP dataSaved = new DataSaved_XMPP(); // outside the try wow!
            try
            {
                DataSaved_XMPP.connectedClients.TryAdd(clientId, webSocket); // Adds The data inside the handlewebsocekt!
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
                            catch { }
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

                            ClientFix.Init(webSocket, dataSaved, clientId);
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
                try
                {
                    DataSaved_XMPP.connectedClients.TryRemove(clientId, out _);
                    //DataSaved..TryRemove(clientId, out _);
                    await XmppFriend.UpdatePresenceForFriends(webSocket, "{}", false, true);

                    Clients client = GlobalData.Clients.FirstOrDefault(c => c.Client == webSocket)!;
                    if (client != null)
                    {


                        int ClientIndex = GlobalData.Clients.FindIndex(e => e.Client == webSocket);
                        var ClientData = GlobalData.Clients[ClientIndex];
                        Console.WriteLine(ClientData);
                        if (ClientIndex != -1)
                        {
                            object ParsedPresence = "";
                            try
                            {
                                ParsedPresence = JsonConvert.DeserializeObject(ClientData.lastPresenceUpdate.presence)!;


                                GlobalData.Clients.Remove(client);

                                foreach (var woah in dataSaved.Rooms)
                                {
                                    if (Array.IndexOf(dataSaved.Rooms, woah) != -1)
                                    {
                                        var MemberIndex = GlobalData.Rooms[woah].members.FindIndex(i => i.accountId == client.accountId);

                                        if (MemberIndex != -1)
                                        {
                                            GlobalData.Rooms[woah]?.members.RemoveAt(MemberIndex);
                                        }
                                    }
                                    //  dataSaved.Rooms.Remove(woah);
                                }

                                try
                                {
                                    if (ParsedPresence != null)
                                    {
                                        //var ParsedPresence = JsonConvert.DeserializeObject(client.lastPresenceUpdate.presence);

                                        Console.WriteLine($"YOO TEST TEST {ParsedPresence}"); // onmly for testing
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex.Message); // Average Erro! 
                                }

                                // what?
                            }
                            catch
                            {

                                GlobalData.Clients.Remove(client);
                                //return; // wow
                            }

 
                        }
                        else
                        {
                            // return;
                        }




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
