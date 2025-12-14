using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.XMPP.SERVER.Send
{
    public class XmppGift
    {
        public static async Task NotifyUser(string SendMessageTo)
        {
            try
            {
                Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == SendMessageTo)!;

                if (Client != null)
                {
                    string xmlMessage;
                    byte[] buffer;
                    WebSocket webSocket = Client.Game_Client;
                    Logger.PlainLog(webSocket.State);
                    if (webSocket != null && webSocket.State == WebSocketState.Open)
                    {
                        XNamespace clientNs = "jabber:client";

                        var message = new XElement(clientNs + "message",
                          new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                          new XAttribute("to", SendMessageTo),
                          new XElement(clientNs + "body", JsonConvert.SerializeObject(new
                          {
                              payload = new { },
                              type = "com.epicgames.gift.received",
                              timestamp = DateTime.UtcNow.ToString("o")
                          }))
                        );

                        xmlMessage = message.ToString();
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);

                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
