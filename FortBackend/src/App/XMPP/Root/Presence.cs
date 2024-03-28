using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.XMPP.Helpers.Resources;
using FortBackend.src.App.XMPP.Helpers.Send;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.XMPP.Root
{
    public class Presence
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
                Console.WriteLine("TEST2 " + xmlDoc.Root?.Attribute("type")?.Value);
                Console.WriteLine("TEST3 " + xmlDoc.Root?.Attribute("type")?.Name);
                switch (xmlDoc.Root?.Attribute("type")?.Value)
                {
                    case "unavailable":
                        Console.WriteLine("UNNNNNNNNNNNNNN");
                        break;
                    default:
                        Console.WriteLine(xmlDoc.Root?.Attribute("type")?.Value);
                        XNamespace mucNamespace = "http://jabber.org/protocol/muc";
                        //XNamespace mucNamespace = "http://jabber.org/protocol/muc#user";
                        XElement MUCX = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name == mucNamespace + "x" || i.Name == "x")!;
                        if (MUCX == null)
                        {
                            Console.WriteLine("Blank");
                            break;
                        }
                        Console.WriteLine("NO FLIPPIMG WAYU");
                        if (string.IsNullOrEmpty(xmlDoc.Root?.Attribute("to")?.Value))
                        {
                            break;
                        }

                        var RoomName = xmlDoc.Root?.Attribute("to")?.Value.Split("@")[0];
                        Console.WriteLine("dfsfs");
                        if (!GlobalData.Rooms.ContainsKey(RoomName))
                        {
                            Console.WriteLine("dfsfs");
                            GlobalData.Rooms[RoomName] = new RoomsLessDyanmic();
                        }
                        Console.WriteLine("kfopdsfdsdfs");
                        var currentMembers = GlobalData.Rooms[RoomName].members;
                        Console.WriteLine("gdsgfds");
                        if (GlobalData.Rooms.ContainsKey(RoomName))
                        {
                            Console.WriteLine("fdsfdsfdsf");
                            foreach (var member in currentMembers)
                            {
                                if (member.accountId == dataSaved.AccountId)
                                {
                                    return;
                                }
                            }
                        }

                        Console.WriteLine("fdsfdsfdsfU");
                        currentMembers.Add(new MoreINfoINsideMembers { accountId = dataSaved.AccountId });


                        dataSaved.Rooms.Append(RoomName); // so we know what room they are in for future stuff!
                        Console.WriteLine("fdskfskopkfpodsfdsfds");
                        GlobalData.Rooms[RoomName].members = currentMembers;
                        //GlobalData.Rooms[RoomName]["Members"] = currentMembers;
                        Console.WriteLine("MUCX NOT NULL");


                        XNamespace clientNs = "jabber:client";
                        XNamespace fdsfdsdsfds = "http://jabber.org/protocol/muc#user";

                        XElement featuresElement = new XElement(clientNs + "presence",
                            new XAttribute("to", dataSaved.JID),
                            new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(dataSaved.DisplayName)}:{dataSaved.AccountId}:{dataSaved.Resource}"),
                            new XElement(fdsfdsdsfds + "x",
                                new XElement("item",
                                    new XAttribute("nick", $"{Uri.EscapeDataString(dataSaved.DisplayName)}:{dataSaved.AccountId}:{dataSaved.Resource}"),
                                    new XAttribute("jid", dataSaved.JID),
                                    new XAttribute("role", "participant"),
                                    new XAttribute("affiliation", "none")
                                )
                            ),
                            new XElement("status", new XAttribute("code", "110")),
                            new XElement("status", new XAttribute("code", "100")),
                            new XElement("status", new XAttribute("code", "170")),
                            new XElement("status", new XAttribute("code", "201"))
                        );

                        xmlMessage = featuresElement.ToString();
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        if (GlobalData.Rooms.TryGetValue(RoomName, out RoomsLessDyanmic? RoomData))
                        {
                            Console.WriteLine("TEST  " + RoomData);
                            foreach (var member in RoomData.members)
                            {
                                Clients ClientData = GlobalData.Clients.Find(i => i.accountId == member.accountId)!;
                                if (ClientData == null) { return; }
                                XElement presenceElement = new XElement(clientNs + "presence",
                                    new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(ClientData.displayName)}:{ClientData.accountId}:{ClientData.resource}"),
                                    new XAttribute("to", ClientData.jid),
                                    new XElement(fdsfdsdsfds + "x",
                                            new XElement("item",
                                            new XAttribute("nick", $"{Uri.EscapeDataString(ClientData.displayName)}:{ClientData.accountId}:{ClientData.resource}"),
                                            new XAttribute("jid", ClientData.jid),
                                            new XAttribute("role", "participant"),
                                            new XAttribute("affiliation", "none")
                                        )
                                    )
                                );

                                xmlMessage = presenceElement.ToString();
                                buffer = Encoding.UTF8.GetBytes(xmlMessage);
                                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                                if (dataSaved.AccountId == ClientData.accountId)
                                {
                                    return; // not normal user! well the person who owns the party
                                }

                                presenceElement = new XElement(clientNs + "presence",
                                    new XAttribute("from", $"{RoomName}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(dataSaved.DisplayName)}:{dataSaved.AccountId}:{dataSaved.Resource}"),
                                    new XAttribute("to", dataSaved.JID),
                                     new XElement(fdsfdsdsfds + "x",
                                        new XElement("item",
                                            new XAttribute("nick", $"{Uri.EscapeDataString(dataSaved.DisplayName)}:{dataSaved.AccountId}:{dataSaved.Resource}"),
                                            new XAttribute("jid", dataSaved.JID),
                                            new XAttribute("role", "participant"),
                                            new XAttribute("affiliation", "none")
                                        )
                                    )
                                );

                                xmlMessage = presenceElement.ToString();
                                buffer = Encoding.UTF8.GetBytes(xmlMessage);
                                await ClientData.Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                            }
                        }

                        // bool bindElement = xmlDoc.Root.Descendants().Any(i => i.Name == mucNamespace + "muc:x" || i.Name == mucNamespace +"x");
                        Console.WriteLine("TEST DEFECT");
                        //Console.WriteLine(xmlDoc.Root?.Descendants().ToLookup());
                        //   Console.WriteLine(bindElement);
                        // if (bindElement == null) return;



                        //GlobalData.Rooms.FirstOrDefault(test => test.Key == "");
                        //  if (hasMucXOrXChild)
                        // {
                        //  if (xmlDoc.Root.Attribute("to") == null)
                        //  {
                        //     return;
                        // }
                        //     Console.WriteLine("YEAH");
                        // }
                        break;

                }
                XElement findStatus = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name == "status")!;

                if (findStatus == null || string.IsNullOrEmpty(findStatus.Value))
                {
                    return;
                }
                object GrabbedStatus = "";
                try
                {
                    GrabbedStatus = JsonConvert.DeserializeObject(findStatus.Value)!;
                }
                catch (JsonReaderException ex)
                {
                    Logger.Error(ex.Message, "PRESENSE}:");
                    return; // wow
                }

                if (GrabbedStatus != null)
                {
                    string status = findStatus.Value;
                    bool away = xmlDoc.Root?.Descendants().Any(i => i.Name == "show") ?? false ? true : false;
                    Console.WriteLine("TEST + " + findStatus);

                    await XmppFriend.UpdatePresenceForFriends(webSocket, status, away, false);
                    await XmppFriend.GrabSomeonesPresence(dataSaved.AccountId, dataSaved.AccountId, false);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Presence:Init");
            }

        }
    }
}
