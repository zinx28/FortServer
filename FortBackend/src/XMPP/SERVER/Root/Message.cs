using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using FortBackend.src.App.SERVER.Send;

namespace FortBackend.src.App.SERVER.Root
{
    public class Message
    {
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved UserDataSaved)
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

               
                var Saved_Clients = GlobalData.Clients.FirstOrDefault(e => e.accountId == UserDataSaved.AccountId);
                if (Saved_Clients == null) { await Client.CloseClient(webSocket); return; }

                XElement findBody = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name == "body")!;

                if (findBody == null || string.IsNullOrEmpty(findBody.Value))
                    return;

                string body = findBody.Value;
               // Console.WriteLine("TYPE " + xmlDoc.Root?.Attribute("type")?.Value);
                switch (xmlDoc.Root?.Attribute("type")?.Value)
                {
                    case "chat":
                        Console.WriteLine("CGAT");
                        if (string.IsNullOrEmpty(xmlDoc.Root?.Attribute("to")?.Value)) break;
                        if (body.Length >= 300) break;

                        Clients Friend = GlobalData.Clients.Find(test => test.jid.Split("/")[0] == xmlDoc.Root?.Attribute("to")?.Value)!;

                        if (Friend == null || Friend.accountId == Saved_Clients.accountId) break;


                        XNamespace clientNs = "jabber:client";


                        XElement featuresElement = new XElement(clientNs + "message",
                            new XAttribute("to", Friend.jid),
                            new XAttribute("from", Saved_Clients.jid),
                            new XAttribute("type", "chat"),
                            new XElement("body", body)
                        );


                        xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await Friend.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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
                        if (MemberList.Find(testc => testc.accountId == Saved_Clients.accountId) == null)
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
                                new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(Saved_Clients.displayName)}:{Saved_Clients.accountId}:{Saved_Clients.resource}"),
                                new XAttribute("type", "groupchat"),
                                new XElement("body", body)
                            );


                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await ClientData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

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

                    await XmppFriend.SendMessageToClient(Saved_Clients.jid, xmlDoc, body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Message:Init");
            }

        }
    }
}
