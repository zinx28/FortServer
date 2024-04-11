using FortLibrary.XMPP;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.SERVER.Send
{
    public class Client
    {
        public static async Task CloseClient(WebSocket webSocket)
        {
            XNamespace ns = "urn:ietf:params:xml:ns:xmpp-framing";
            XElement closeElement = new XElement(XName.Get("close", "urn:ietf:params:xml:ns:xmpp-framing"));
            string xmlMessage = closeElement.ToString(SaveOptions.DisableFormatting);
            byte[] buffer69 = Encoding.UTF8.GetBytes(xmlMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer69), WebSocketMessageType.Text, true, CancellationToken.None);
            webSocket.Dispose();
        }


        public static async Task SendClientMessage(Clients client, XElement xElement)
        {
            try
            {
                await client.Game_Client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(xElement.ToString())), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine(ex.Message);
            }
        }
    }
}
