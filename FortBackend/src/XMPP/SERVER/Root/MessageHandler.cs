using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using FortBackend.src.App.SERVER.Send;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using FortLibrary;

namespace FortBackend.src.App.SERVER.Root
{
    public class MessageHandler
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
                XNamespace clientNs = "jabber:client";
                XElement featuresElement;

                var Saved_Clients = GlobalData.Clients.FirstOrDefault(e => e.accountId == UserDataSaved.AccountId);
                if (Saved_Clients == null) { await Client.CloseClient(webSocket); return; }

                string body = xmlDoc.Root!
                    .Descendants()
                    .FirstOrDefault(i => i.Name == "body")!.Value;

                if (string.IsNullOrEmpty(body))
                    return;

             //   string body = findBody.Value;
                // Console.WriteLine("TYPE " + xmlDoc.Root?.Attribute("type")?.Value);
                var TypeAtrribute = xmlDoc.Root?.Attribute("type");
                var Type = TypeAtrribute is null ? "" : TypeAtrribute.Value;
                switch (Type)
                {
                    case "chat":
                        Console.WriteLine("CGAT");
                        if (string.IsNullOrEmpty(xmlDoc.Root?.Attribute("to")?.Value)) break;
                        if (body.Length >= 300) break;

                        Clients Friend = GlobalData.Clients.Find(test => test.jid.Split("/")[0] == xmlDoc.Root?.Attribute("to")?.Value)!;

                        if (Friend == null || Friend.accountId == Saved_Clients.accountId) break;

                        featuresElement = new XElement(clientNs + "message",
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

                        foreach(MembersData roomData in MemberList)
                        {
                            var ClientData = GlobalData.Clients.Find(client => client.accountId == roomData.accountId);
                            if (ClientData == null) continue;

                            featuresElement = new XElement(clientNs + "message",
                                new XAttribute("to", ClientData.jid),
                                new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(Saved_Clients.displayName)}:{Saved_Clients.accountId}:{Saved_Clients.resource}"),
                                new XAttribute("type", "groupchat"),
                                new XElement("body", body)
                            );


                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await ClientData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }

                        break;
                }

                if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty((string)xmlDoc.Root?.Attribute("to")!) || string.IsNullOrEmpty((string)xmlDoc.Root.Attribute("id")!))
                    return;

                await XmppFriend.SendMessageToClient(Saved_Clients.jid, xmlDoc, body);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Message:Init");
            }

        }
    }
}
