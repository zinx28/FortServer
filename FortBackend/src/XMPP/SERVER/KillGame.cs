using FortBackend.src.App.SERVER.Send;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.XMPP.SERVER
{
    public class KillGame
    {
        public static async Task Init(Clients client, DataSaved dataSaved)
        {
            //int ClientIndex = GlobalData.Clients.FindIndex(e => e.Game_Client == client.Game_Client);
            //if (ClientIndex == -1)
            //{
            //    Console.WriteLine("OK WTFFFF");
            //    return;
            //}
            //var ClientData = GlobalData.Clients[ClientIndex];
             var presence = client.lastPresenceUpdate.presence;
            // WE CALL IT HERE SO IT ISNT {}

            // now update everyone 
            await XmppFriend.UpdatePresenceForFriends(client.Game_Client, "{}", false, true);

          
            //if (ClientIndex != -1)
            //{
            //    GlobalData.Clients[ClientIndex].Game_Client = null; // pooper
            dynamic ParsedPresence = "";
            try
            {
                ParsedPresence = JsonConvert.DeserializeObject(presence)!;

                foreach (var woah in dataSaved.Rooms)
                {
                    if (Array.IndexOf(dataSaved.Rooms, woah) != -1)
                    {
                        var MemberIndex = GlobalData.Rooms[woah].members.FindIndex(i => i.accountId == client.accountId);

                        if (MemberIndex != -1)
                        {
                            GlobalData.Rooms[woah]?.members.RemoveAt(MemberIndex);
                        }
                    }
                }

                try
                {
                    if (ParsedPresence != null && ParsedPresence.Properties != null)
                    {
                        //var ParsedPresence = JsonConvert.DeserializeObject(client.lastPresenceUpdate.presence);
                        //YOO TEST TEST {}
                        // Console.WriteLine("TEST 2 " + dataSaved.p)

                        // Console.WriteLine($"YOO {ParsedPresence.Properties}");
                        var PartyId = "";
                        foreach (var property in ParsedPresence.Properties)
                        {
                            //Console.WriteLine(property);
                            //Console.WriteLine(property);
                            string key = property.Name;
                            if (key.ToString().ToLower().Contains("party.joininfo"))
                            {
                                var partyJoinInfoData = ParsedPresence.Properties[key];

                                PartyId = partyJoinInfoData.partyId;
                                Console.WriteLine($"Party ID: {PartyId}");

                                break;
                            }
                        }

                        foreach (Clients Client in GlobalData.Clients)
                        {
                           
                            if (Client.accountId == client.accountId) continue;
                            Console.WriteLine(Client.accountId);
                            Console.WriteLine(client.accountId);

                            foreach(var test in Client.Rooms) {
                                Console.Write("test" + Client.Rooms);
                            }
                            // Check rooms so we dont tell everyone
                            Console.Write("a " + Client.Rooms);

                            byte[] buffer;
                            string xmlMessage;
                            XNamespace clientNs = "jabber:client";
                            XElement featuresElement = new XElement(clientNs + "message",
                                new XAttribute("id", Guid.NewGuid().ToString().Replace("-", "").ToUpper()),
                                new XAttribute("from", client.jid),
                                new XAttribute("xmlns", "jabber:client"),
                                new XAttribute("to", Client.jid),
                                new XElement("body", JsonConvert.SerializeObject(new
                                {
                                    type = "com.epicgames.party.memberexited",
                                    payload = new
                                    {
                                        partyId = PartyId,
                                        memberId = client.accountId,
                                        wasKicked = false
                                    },
                                    timestamp = DateTime.UtcNow.ToString("o")
                                }))
                            );
                            xmlMessage = featuresElement.ToString();
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await Client.Game_Client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    // Console.WriteLine($"YOO TEST TEST {ParsedPresence}"); // onmly for testing

                  
                  

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Average Erro! 
                }

                // what?
            }
            catch
            {
                    
                // GlobalData.Clients.Remove(client);
                //return; // wow
            }


            //}
            //else
            //{
            //    // return;
            //}
        }
    }
}
