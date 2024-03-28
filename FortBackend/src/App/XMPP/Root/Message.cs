using FortBackend.src.App.XMPP.Helpers.Resources;
using FortBackend.src.App.XMPP.Helpers.Send;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.App.Utilities;

namespace FortBackend.src.App.XMPP.Root
{
    public class Message
    {
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved dataSaved)
        {
            try
            {
                string xmlMessage;
                byte[] buffer;
                if (clientId == null)
                {
                    await Client.CloseClient(webSocket);
                    return;
                }

                XElement findBody = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name == "body")!;

                if (findBody == null || string.IsNullOrEmpty(findBody.Value))
                    return;

                string body = findBody.Value;
                Console.WriteLine("TYPE " + xmlDoc.Root?.Attribute("type")?.Value);
                switch (xmlDoc.Root?.Attribute("type")?.Value)
                {
                    case "chat":
                        Console.WriteLine("CGAT");
                        if (string.IsNullOrEmpty(xmlDoc.Root?.Attribute("to")?.Value))
                        {
                            break;
                        }
                        if (body.Length >= 300)
                        {
                            break;
                        }

                        Clients Friend = GlobalData.Clients.Find(test => test.jid.Split("/")[0] == xmlDoc.Root?.Attribute("to")?.Value)!;

                        if (Friend == null || Friend.accountId == dataSaved.AccountId) break;


                        XNamespace clientNs = "jabber:client";


                        XElement featuresElement = new XElement(clientNs + "message",
                            new XAttribute("to", Friend.jid),
                            new XAttribute("from", dataSaved.JID),
                            new XAttribute("type", "chat"),
                            new XElement("body", body)
                        );


                        xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await Friend.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        break;
                    case "groupchat":
                        Console.WriteLine("GC");
                        if (string.IsNullOrEmpty(xmlDoc.Root?.Attribute("to")?.Value))
                        {
                            break;
                        }
                        if (body.Length >= 300)
                        {
                            break;
                        }

                        string RoomName = xmlDoc.Root?.Attribute("to")?.Value.Split("@")[0]!;
                        var RoomsFound = GlobalData.Rooms[RoomName];
                        if (RoomsFound == null)
                        {
                            break;
                        }
                        var MemberList = GlobalData.Rooms[RoomName].members;
                        if (MemberList.Find(testc => testc.accountId == dataSaved.AccountId) == null)
                        {
                            break;
                        }

                        MemberList.ForEach(async member =>
                        {
                            var ClientData = GlobalData.Clients.Find(client => client.accountId == member.accountId);
                            if (ClientData == null)
                            {
                                return;
                            }

                            XNamespace clientNs = "jabber:client";


                            XElement featuresElement = new XElement(clientNs + "message",
                                new XAttribute("to", ClientData.jid),
                                new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(dataSaved.DisplayName)}:{dataSaved.AccountId}:{dataSaved.Resource}"),
                                new XAttribute("type", "groupchat"),
                                new XElement("body", body)
                            );


                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await ClientData.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        });
                        break;
                }

                JToken test = "";
                try
                {
                    test = JToken.Parse(body);
                }
                catch
                {
                    return; // wow
                }

                if (test is JObject)
                {
                    if (test["type"] == null || string.IsNullOrEmpty((string)xmlDoc.Root?.Attribute("to")!) || string.IsNullOrEmpty((string)xmlDoc.Root.Attribute("id")!))
                        return;

                    await XmppFriend.SendMessageToClient(dataSaved.JID, xmlDoc, body);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Message:Init");
            }

        }
    }
}
