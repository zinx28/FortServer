using FortMatchmaker.src.App.Utilities;
using FortMatchmaker.src.App.Utilities.Classes;
using System;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.Websocket
{
    public class NewConnection
    {
        public static async Task Init(WebSocket webSocket, HttpRequest request, string clientId)
        {
            try
            {
                MatchmakerData.connected.TryAdd(clientId, webSocket);
                byte[] buffer = new byte[1024];

                // code that will make it connect and do stuff but not coded yet

                // Checks if state is closed after it connects
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
