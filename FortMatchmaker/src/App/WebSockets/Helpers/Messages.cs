using FortLibrary;
using System.Net.WebSockets;
using System.Text;

namespace FortMatchmaker.src.App.WebSockets.Helpers
{
    public class Messages
    {
        public static void Send(WebSocket webSocket, string message)
        {
            try
            {
                if (webSocket.State != WebSocketState.Open) { webSocket.Dispose(); return; }

                var responseBytes = Encoding.UTF8.GetBytes(message);
                webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                webSocket.Dispose();
            }
        }

        public static async Task SendAsync(WebSocket webSocket, string message, int delay)
        {
            try
            {
                if (webSocket.State != WebSocketState.Open) { webSocket.Dispose(); return; }

                var responseBytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                await Task.Delay(delay);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                webSocket.Dispose();
            }
        }
    }
}
