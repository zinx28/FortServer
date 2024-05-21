using System.Net.WebSockets;
using System.Text;

namespace FortMatchmaker.src.App.WebSockets.Helpers
{
    public class Messages
    {
        public static async Task Send(WebSocket webSocket, string message, int delay)
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
                Console.WriteLine(ex.ToString());
                webSocket.Dispose();
            }
        }
    }
}
