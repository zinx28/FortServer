using FortBackend.src.App.TCP_XMPP.Helpers.Resources;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.TCP_XMPP.Helpers.Send
{
    public class Client
    {
        public static async Task CloseClient(TcpClient webSocket)
        {
            XNamespace ns = "urn:ietf:params:xml:ns:xmpp-framing";
            XElement closeElement = new XElement(XName.Get("close", "urn:ietf:params:xml:ns:xmpp-framing"));
            string xmlMessage = closeElement.ToString(SaveOptions.DisableFormatting);
            byte[] buffer69 = Encoding.UTF8.GetBytes(xmlMessage);
            await webSocket.Client.SendAsync(new ArraySegment<byte>(buffer69), SocketFlags.None);
            webSocket.Dispose();
        }


        public static async Task SendClientMessage(Clients client, XElement xElement)
        {
            try
            {
                await client.Client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(xElement.ToString())), SocketFlags.None);
                //await client.Client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(xElement.ToString())), SocketFlags.None);
                //await client.Client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(xElement.ToString())), WebSocketMessageType.Text, true, CancellationToken.None);
                //await webSocket.Client.SendAsync(new ArraySegment<byte>(buffer69), SocketFlags.None);
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine(ex.Message);
            }
        }
    }
}
