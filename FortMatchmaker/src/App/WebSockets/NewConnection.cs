using FortLibrary;
using FortLibrary.EpicResponses.Matchmaker;
using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.WebSockets.Helpers;
using FortMatchmaker.src.App.WebSockets.Modules;
using FortMatchmaker.src.App.WebSockets.Roots;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;

namespace FortMatchmaker.src.App.Websockets
{
    public class IDThing
    {
        public string ID { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    public class NewConnection
    {
        public static async Task Init(WebSocket webSocket, HttpRequest request, string clientId)
        {
            try
            {
                MatchmakerData.connected.TryAdd(clientId, webSocket);
                byte[] buffer = new byte[1024];
                MatchmakerTicket matchmakerTicket = new MatchmakerTicket();
                while (webSocket.State == WebSocketState.Open)
                {
                    var connectingTask = Messages.SendAsync(webSocket, JsonSavedData.connectingPayloadJson(), JsonSavedData.ConnectingDelay);
                  
                    matchmakerTicket = await Authorization.Init(webSocket, request.Headers);

                    if (!string.IsNullOrEmpty(matchmakerTicket.accountId))
                    {
                        var waitingTask = Messages.SendAsync(webSocket, JsonSavedData.waitingPayloadJson(MatchmakerData.connected.Count), JsonSavedData.WaitingDelay);

                        await Task.WhenAll(connectingTask, waitingTask);

                        // this is for season 1.11 but pretty sure they did a id v2 on season 3
                        if (matchmakerTicket.Playlist == "2") // solos
                            matchmakerTicket.Playlist = "Playlist_DefaultSolo";
                        else if (matchmakerTicket.Playlist == "10") // duos
                            matchmakerTicket.Playlist = "Playlist_DefaultDuo";
                        else if (matchmakerTicket.Playlist == "9") // squads
                            matchmakerTicket.Playlist = "Playlist_DefaultSquad";

                        UserData userData = new UserData
                        {
                            Playlist = matchmakerTicket.Playlist,
                            buildId = matchmakerTicket.BuildId,
                            Region = matchmakerTicket.Region,
                           // Ticket = Guid.NewGuid().ToString().Replace("-", ""),
                            AccountId = matchmakerTicket.accountId,
                            AccessToken = matchmakerTicket.AccessToken,
                            InsertionTime = DateTime.UtcNow,
                            CustomCode = matchmakerTicket.CustomKey ?? "NONE",
                            Queuing = true
                        };

                        MatchmakerData.SavedData.TryAdd(webSocket, userData);

                        var queuedTask = Messages.SendAsync(webSocket, JsonSavedData.queuedPayloadJson(userData, webSocket), 200);
                        //var searchTask = WServer.Search(webSocket, matchmakerTicket);

                        await Task.WhenAll(queuedTask/*, searchTask*/);
                        break;
                    }
                    else if(matchmakerTicket.IsHoster)
                    {
                        var Sigma = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        Console.WriteLine("Sigma: " + Encoding.UTF8.GetString(buffer, 0, Sigma.Count));

                        HosterJ hosterJ = JsonConvert.DeserializeObject<HosterJ>(Encoding.UTF8.GetString(buffer, 0, Sigma.Count))!;

                        if (hosterJ != null)
                        {
                            hosterJ.webSocket = webSocket;
                            MatchmakerData.matchmakerData.Add(hosterJ);
                        }

                        break;
                    }
                    
                }

                if(matchmakerTicket.IsHoster)
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        // ready?!?!
                        var Sigma = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        Console.WriteLine("Sigma: " + Encoding.UTF8.GetString(buffer, 0, Sigma.Count));
                        IDThing iDThing = JsonConvert.DeserializeObject<IDThing>(Encoding.UTF8.GetString(buffer, 0, Sigma.Count))!;

                        if(iDThing != null)
                        {
                            FortLibrary.EpicResponses.Matchmaker.Server server = Saved.CurrentServers.FirstOrDefault(e => e.Session == iDThing.ID)!;
                            if(server != null)
                            {
                                if(iDThing.Message == "LAUNCHING")
                                {
                                    
                                    server.bServersLaunching = true;
                                    Console.WriteLine("SERVERS LAUNCHING");
                                }
                                // only if dll doesnt use console
                                else if(iDThing.Message == "JOINABLE")
                                {
                                    server.bServersLaunching = false;
                                    server.bJoinable = true;
                                    Console.WriteLine("SERVERS JOINABLE");
                                }
                            }
                        }
                        //Sigma: {"ID":"54c7b5370cb549568991a6c32046b98e","Message":"LAUNCHING"}
                    }
                }


                Console.WriteLine(webSocket.State);
                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed Websocket please try to reconnect", CancellationToken.None);
                }

                if (webSocket.State == WebSocketState.Closed)
                {
                    webSocket.Dispose();
                }

                WebSocketReceiveResult result;
                do
                {
                    try
                    {
                        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        result = new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, WebSocketCloseStatus.InternalServerError, "An error occurred.");
                    }
                } while (!result.CloseStatus.HasValue);

                if (MatchmakerData.SavedData.ContainsKey(webSocket))
                {
                    MatchmakerData.SavedData.TryRemove(webSocket, out _);
                }

                MatchmakerData.connected.TryRemove(clientId, out _);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "NewConnection: [1]");
                try
                {
                    if (MatchmakerData.SavedData.ContainsKey(webSocket))
                    {
                        MatchmakerData.SavedData.TryRemove(webSocket, out _);
                    }
                    MatchmakerData.connected.TryRemove(clientId, out _);
                }
                catch (Exception ex1)
                {
                    Logger.Error(ex1.Message, "NewConnection: [2]");
                }
            }
        }
    }
}
