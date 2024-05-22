using FortLibrary;
using FortLibrary.EpicResponses.Matchmaker;
using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.WebSockets.Helpers;
using FortMatchmaker.src.App.WebSockets.Modules;
using FortMatchmaker.src.App.WebSockets.Roots;
using System;
using System.Net.WebSockets;
using Server = FortMatchmaker.src.App.WebSockets.Helpers.Server;

namespace FortMatchmaker.src.App.Websockets
{
    public class NewConnection
    {
        public static async Task Init(WebSocket webSocket, HttpRequest request, string clientId)
        {
            try
            {
                MatchmakerData.connected.TryAdd(clientId, webSocket);
                byte[] buffer = new byte[1024];

                while (webSocket.State == WebSocketState.Open)
                {
                    var connectingTask = Messages.Send(webSocket, JsonSavedData.connectingPayloadJson(), JsonSavedData.ConnectingDelay);
                    MatchmakerTicket matchmakerTicket = await Authorization.Init(webSocket, request.Headers);

                    if (!string.IsNullOrEmpty(matchmakerTicket.accountId))
                    {
                        var waitingTask = Messages.Send(webSocket, JsonSavedData.waitingPayloadJson(), JsonSavedData.WaitingDelay);

                        await Task.WhenAll(connectingTask, waitingTask);

                        UserData userData = new UserData
                        {
                            Playlist = matchmakerTicket.Playlist,
                            buildId = matchmakerTicket.BuildId,
                            Region = matchmakerTicket.Region,
                            Ticket = Guid.NewGuid().ToString().Replace("-", ""),
                            AccountId = matchmakerTicket.accountId,
                            AccessToken = matchmakerTicket.AccessToken,
                            InsertionTime = DateTime.UtcNow,
                            CustomCode = matchmakerTicket.CustomKey ?? "NONE",
                        };

                        MatchmakerData.SavedData.TryAdd(webSocket, userData);

                        var queuedTask = Messages.Send(webSocket, JsonSavedData.queuedPayloadJson(userData, webSocket), 200);
                        var searchTask = Server.Search(webSocket, matchmakerTicket);

                        await Task.WhenAll(queuedTask, searchTask);
                        break;
                    }
                }

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
