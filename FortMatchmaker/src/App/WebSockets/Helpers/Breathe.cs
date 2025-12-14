using FortLibrary;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Matchmaker;
using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.WebSockets.Modules;
using Newtonsoft.Json;
using SharpCompress.Compressors.Xz;
using System.Net.WebSockets;
using System.Text;

namespace FortMatchmaker.src.App.WebSockets.Helpers
{
    public class Breathe
    {
        public class TempDATA
        {
            public string Region { get; set; } = "";
            public string Playlist { get; set; } = "";
        }
        public async static Task Start()
        {
            Logger.Log("Loaded!", "QueueChecker");
            while (true)
            {
                int PeopleInQueue = 0;
                List<TempDATA> Region = new();
                foreach (var Websocket in MatchmakerData.SavedData.Keys)
                {
                    //Console.WriteLine(Websocket);
                    try
                    {
                        if (MatchmakerData.SavedData.TryGetValue(Websocket, out var userData))
                        {

                            // if they are not queingf it wont do nothing
                            if (userData.Queuing)
                            {
                                if (Region.FirstOrDefault(e => e.Playlist == userData.Playlist && e.Region == userData.Region) == null)
                                    Region.Add(new TempDATA
                                    {
                                        Region = userData.Region,
                                        Playlist = userData.Playlist
                                    });

                                PeopleInQueue++;
                                if (Websocket.State == WebSocketState.Open)
                                {
                                    if (string.IsNullOrEmpty(userData.Ticket))
                                        await Websocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSavedData.queuedPayloadJson(userData, Websocket))), WebSocketMessageType.Text, true, CancellationToken.None);
                                    else
                                        Messages.Send(Websocket, JsonSavedData.sessionAssignmentPayloadJson(userData.Ticket));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }
                }

                if (PeopleInQueue > 0)
                {
                    foreach (var item in Region)
                    {
                        // we find the hoster that is hosting for that playlist and region
                        Logger.PlainLog(item.Region);
                        Logger.PlainLog(item.Playlist);
                        var Data = MatchmakerData.matchmakerData.FirstOrDefault(e => e.Region == item.Region && e.Playlist == item.Playlist);

                        if(Data != null && Data?.webSocket != null)
                        {
                            Logger.Log($"FortHoster is connected with {MatchmakerData.SavedData.Values.Select(e => e.Region).ToList().Count}");
                            Logger.PlainLog(Saved.serverHotFixes.max_servers);
                            
                            var ServerToUpdate = Saved.CurrentServers.FindAll(e => e.bServersLaunching == true && e.CurrentPlayers < e.MaxPlayers);

                            if (ServerToUpdate != null && ServerToUpdate.Count > 0)
                            {
                                
                                foreach (var e in ServerToUpdate)
                                {
                                    // todo need to remove the player when they leave queue!
                                    if(e.CurrentPlayers >= e.MaxPlayers) continue;

                                    if(string.IsNullOrEmpty(e.MatchID))
                                    {
                                        e.MatchID = RandomHash.CalculateMd5Hash("2" + DateTimeOffset.Now.ToUnixTimeMilliseconds());
                                    }

                                    foreach (var item1 in MatchmakerData.SavedData)
                                    {
                                        if (!item1.Value.Queuing) continue;
                                        if (item1.Value.Playlist != item.Playlist) continue;
                                        if (item1.Value.Region != item.Region) continue;
                                        Logger.PlainLog(item1.Value.Ticket);
                                        if (!string.IsNullOrEmpty(item1.Value.Ticket)) continue;

                                        item1.Value.Ticket = e.Session;
                                        
                                        e.CurrentPlayers++;
                                        // ik this code is like bad
                                        Messages.Send(item1.Key, JsonSavedData.sessionAssignmentPayloadJson(e.MatchID));
                                    }
                                };
                            }

                            var ServersToJoin = Saved.CurrentServers.FindAll(e => e.bJoinable);

                            if (ServersToJoin != null && ServersToJoin.Count > 0)
                            {
                                Logger.Log("JOIN ABLE");
                                foreach (var e in ServersToJoin)
                                {
                                    string joinPayload = JsonConvert.SerializeObject(new
                                    {
                                        payload = new
                                        {
                                            matchId = e.MatchID,
                                            sessionId = e.Session,
                                            joinDelaySec = 1
                                        },
                                        name = "Play"
                                    });

                                    Logger.Log("2");
                                    foreach (var item1 in MatchmakerData.SavedData)
                                    {
                                        if (!item1.Value.Queuing) continue;
                                        if (item1.Value.Playlist != item.Playlist) continue;
                                        if (item1.Value.Region != item.Region) continue;
                                        if (item1.Value.Ticket != e.Session)
                                        {
                                            if(string.IsNullOrEmpty(item1.Value.Ticket))
                                            {
                                                if(e.CurrentPlayers != e.MaxPlayers)
                                                {
                                                    item1.Value.Ticket = e.Session;
                                                    e.CurrentPlayers++;
                                                }
                                                else continue;
                                            }
                                            else continue;

                                        };

                                        await Messages.SendAsync(item1.Key, JsonSavedData.sessionAssignmentPayloadJson(e.MatchID), 2000);
                                        await Messages.SendAsync(item1.Key, joinPayload, 3000);

                                        try
                                        {
                                            await item1.Key.CloseAsync(WebSocketCloseStatus.NormalClosure, "Message By Fortnite Gamer! - He Was Here", CancellationToken.None);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex.Message, "[FortniteGamer]");
                                        }

                                        item1.Key.Dispose();
                                    }
                                };
                            }


                            if (Saved.CurrentServers.FindAll(e => e.Region == Data.Region && e.Playlist == Data.Playlist).Count >= Saved.serverHotFixes.max_servers)
                            {
                                //if()
                                // tell user to join match
                                continue; // too much servers are up for that playlist or region... bad vps
                            }

                            Server servers = new()
                            {
                                Session = Guid.NewGuid().ToString().Replace("-", ""),
                                Playlist = Data.Playlist,
                                Region = Data.Region,
                                MaxPlayers = Saved.serverHotFixes.max_players,
                                // am i stupid HAHAHHA
                                Ip = Data.IP,
                                Port = Data.Port,
                                Name = "FortBackend"
                            };

                            Saved.CurrentServers.Add(servers);

                            // tell hoster to host
                            try
                            {
                                await Data.webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                                {
                                    ID = servers.Session,
                                    Message = "HOST"
                                }))), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                                // weird.....
                                // remove matchmakerdata....
                                MatchmakerData.matchmakerData.Remove(Data);
                                Saved.CurrentServers.Remove(servers);
                            }
                           
                        }
                    }
                }

                Logger.Log("Logging queue metrics...", "MatchMetrics");
                Logger.Log($"Number of players in queue: {PeopleInQueue}", "MatchMetrics");
                Logger.Log("Number of active matches: N/A", "MatchMetrics");
                Logger.Log("Average match duration: N/A", "MatchMetrics");

                await Task.Delay(18000);
            }
        }
    }
}
