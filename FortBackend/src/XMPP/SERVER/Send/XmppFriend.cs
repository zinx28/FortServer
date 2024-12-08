
using FortBackend.src.App.SERVER.Root;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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

                ClientData.lastPresenceUpdate.away = away;
                ClientData.lastPresenceUpdate.presence = status;

                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(ClientData.accountId);
          
                if (profileCacheEntry.AccountData != null)
                {
                    UserFriends FriendsDataParsed = profileCacheEntry.UserFriends;
                    Console.WriteLine("TEST");
                    foreach (var friend in FriendsDataParsed.Accepted)
                    {
                        Console.WriteLine(friend.accountId);
                        Console.WriteLine(status);
                        var FriendsClientData = GlobalData.Clients.FirstOrDefault(client => client.accountId == friend.accountId);

                        if (FriendsClientData == null) continue; // friend is offline

                        XNamespace clientNs1 = "jabber:client";
                        XElement openElement = new XElement(clientNs1 + "presence",
                            new XAttribute("to", FriendsClientData.jid),
                            new XAttribute("from", ClientData.jid),
                            new XAttribute("type", offline ? "unavailable" : "available")
                        );

                        if (away)
                        {
                            openElement.Add(new XElement(clientNs1 + "show", "away"));
                        }

                        openElement.Add(new XElement(clientNs1 + "status", status));

                        xmlMessage = openElement.ToString();
                        Console.WriteLine("veryporper " + xmlMessage);
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);

                        await FriendsClientData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
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

                Logger.Warn(xmlMessage, "XMPPPRESNCE");
                buffer = Encoding.UTF8.GetBytes(xmlMessage);

                await ToAccountIdData.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Friends:GrabSomeonesPresence");
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


                    ShittyXmppClass shittyXmppClass = JsonConvert.DeserializeObject<ShittyXmppClass>(body);

                    if(shittyXmppClass.type == "com.epicgames.party.data")
                    {
                        Console.WriteLine(shittyXmppClass.payload.ToString());
                        PartyData partyData = JsonConvert.DeserializeObject<PartyData>(shittyXmppClass.payload.ToString());
                        if(partyData != null && !string.IsNullOrEmpty(partyData.partyId))
                        {
                            if(partyData.payload.Attrs.PartyState_s == "BattleRoyaleView")
                            {
                                Console.WriteLine("I WAnt to shut down the matchmaker for everything in the party!");
                                if (string.IsNullOrEmpty(partyData.payload.Attrs.MatchmakingInfoString_s))
                                {
                                    var PartyId = $"Party-{partyData.partyId}";
                                    

                                    var test = GlobalData.Rooms.FirstOrDefault(e => e.Key == PartyId).Value;
                                    if (test != null)
                                    {
                                        foreach(var member in test.members)
                                        {
                                            using (HttpClient client = new HttpClient())
                                            {
                                                Console.WriteLine($"{Saved.BackendCachedData.DefaultProtocol}{Saved.DeserializeConfig.MatchmakerIP}:{Saved.DeserializeConfig.MatchmakerPort}/fortmatchmaker/removeUser/{member.accountId}");
                                                client.DefaultRequestHeaders.Add("Authorization", $"bearer {Saved.DeserializeConfig.JWTKEY}");
                                                var response = await client.PostAsync($"{Saved.BackendCachedData.DefaultProtocol}{Saved.DeserializeConfig.MatchmakerIP}:{Saved.DeserializeConfig.MatchmakerPort}/fortmatchmaker/removeUser/{member.accountId}", null);
                                                var responseContent = await response.Content.ReadAsStringAsync();

                                                if (!response.IsSuccessStatusCode)
                                                {

                                                }
                                            }
                                        }
                                       
                                            //fortmatchmaker/removeUser
                                            Console.WriteLine($"how many peopel in party -> {test.members.Count}");
                                    }
                                }
                            }
                        }
                     
                    }


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
