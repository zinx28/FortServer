
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.MongoDB.Module;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.SERVER.Send
{
    public class XmppFriend
    {
        public static async Task UpdatePresenceForFriends(WebSocket webSocket, string status, bool away, bool offline)
        {
            try
            {
                string xmlMessage;
                byte[] buffer;

                int ClientIndex = GlobalData.Clients.FindIndex(testc => testc.Game_Client == webSocket);

                if (ClientIndex == -1) return;
                
                var ClientData = GlobalData.Clients[ClientIndex];

                GlobalData.Clients[ClientIndex].lastPresenceUpdate.away = away;
                GlobalData.Clients[ClientIndex].lastPresenceUpdate.presence = status;

                //HttpClient httpClient = new HttpClient();
                //HttpResponseMessage response = await httpClient.GetAsync($"{Saved.BackendCachedData.DefaultProtocol}127.0.0.1{Saved.DeserializeConfig.BackendPort}/PRIVATE/DEVELOPER/DATA/{ClientData.accountId}");

                //if (response.IsSuccessStatusCode)
                //{
                //ProfileCacheEntry
                //ar datareturned = await response.Content.ReadAsStringAsync();
                //if (datareturned != null)
                // {
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(ClientData.accountId);
          
                        if (profileCacheEntry.AccountData != null)
                        {
                            UserFriends FriendsDataParsed = profileCacheEntry.UserFriends;
                            Console.WriteLine("TEST");
                            foreach (var friend in FriendsDataParsed.Accepted)
                            {
                                Console.WriteLine(friend.accountId);
                                Console.WriteLine(status);
                                var FriendsClientData = GlobalData.Clients.FirstOrDefault(client => client.accountId == FriendsDataParsed.AccountId);

                                if (FriendsClientData == null)
                                {
                                    Console.WriteLine("WHYYYY!");
                                    return;
                                }
                                XNamespace clientNs1 = "jabber:client";
                                XElement openElement = new XElement(clientNs1 + "presence",
                                  new XAttribute("to", FriendsClientData.jid),
                                  new XAttribute("from", ClientData.jid),
                                  new XAttribute("type", offline ? "unavailable" : "available")
                                );

                                if (away)
                                {
                                    openElement.Add(new XElement("show", "away"));
                                    openElement.Add(new XElement("status", status));
                                }
                                else
                                {
                                    openElement.Add(new XElement("status", status));
                                }

                                xmlMessage = openElement.ToString();
                                buffer = Encoding.UTF8.GetBytes(xmlMessage);

                                await FriendsClientData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }

                   // }

               //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Friends:UpdatePresenceForFriends");
            }
        }

        public static async Task GrabSomeonesPresence(string FromAccountId, string ToAccountId, bool offline)
        {
            try
            {

                string xmlMessage;
                byte[] buffer;
                var FromAccountIdData = GlobalData.Clients.FirstOrDefault(no => no.accountId == FromAccountId);
                var ToAccountIdData = GlobalData.Clients.FirstOrDefault(no => no.accountId == ToAccountId);

                if (FromAccountIdData == null || ToAccountIdData == null)
                {
                    Console.WriteLine("NOT FOUND");
                    return; // invalid data not found?
                }

                XNamespace clientNs1 = "jabber:client";
                XElement openElement = new XElement(clientNs1 + "presence",
                  new XAttribute("to", ToAccountIdData.jid),
                  new XAttribute("from", FromAccountIdData.jid),
                  new XAttribute("type", offline ? "unavailable" : "available")
                );

                if (FromAccountIdData.lastPresenceUpdate.away)
                {
                    openElement.Add(new XElement("show", "away"));
                }

                openElement.Add(new XElement("status", FromAccountIdData.lastPresenceUpdate.presence));

                xmlMessage = openElement.ToString();
                buffer = Encoding.UTF8.GetBytes(xmlMessage);

                await ToAccountIdData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Friends:GrabSomeonesPresence");
            }
        }

        public static async Task SendMessageToClient(string JID, XDocument xmlDoc, string body)
        {
            try
            {

                string xmlMessage;
                byte[] buffer;
                var receiver = GlobalData.Clients.FirstOrDefault(i =>
                {
                    string[] jidParts = i.jid.Split('/');
                    return jidParts[0] == (string)xmlDoc.Root?.Attribute("to")! || i.jid == (string)xmlDoc.Root?.Attribute("to")!;
                });
                if (receiver != null)
                {

                    XNamespace clientNs1 = "jabber:client";
                    XElement message = new XElement(clientNs1 + "message",
                        new XAttribute("from", JID),
                        new XAttribute("id", (string)xmlDoc.Root?.Attribute("id")!),
                        new XAttribute("to", receiver.jid),
                        new XElement("body", body)
                    );

                    xmlMessage = message.ToString();
                    buffer = Encoding.UTF8.GetBytes(xmlMessage);

                    await receiver.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine("THIS IS NULLW HY!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Friends:GrabSomeonesPresence");
            }
        }


        public static async Task SendMessageToId(object body, string Id)
        {
            try
            {

                string xmlMessage;
                byte[] buffer;
                var receiver = GlobalData.Clients.FirstOrDefault(i => i.accountId == Id);
                if (receiver != null)
                {

                    XNamespace clientNs1 = "jabber:client";
                    XElement message = new XElement(clientNs1 + "message",
                        new XAttribute("from", "xmpp-admin@prod.ol.epicgames.com"),
                        new XAttribute("to", receiver.jid),
                        new XElement("body", body.ToString())
                    );

                    xmlMessage = message.ToString();
                    buffer = Encoding.UTF8.GetBytes(xmlMessage);

                    await receiver.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message, "Friends:GrabSomeonesPresence");
            }
        }
    }
}
