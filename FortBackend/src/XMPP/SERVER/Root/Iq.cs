using FortBackend.src.App.SERVER.Send;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.SERVER.Root
{
    public class Iq
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
                //Console.WriteLine(xmlDoc.Root?.Attribute("id")?.Value);
               // var Clients = GlobalData.Clients.FirstOrDefault(e => e.accountId == dataSaved.AccountId);
                //if (Clients == null) { await Client.CloseClient(webSocket); return; }
                Console.WriteLine(xmlDoc.Root?.Attribute("id")?.Value);
                switch (xmlDoc.Root?.Attribute("id")?.Value)
                {
                    case "_xmpp_bind1":

                        if (dataSaved.Resource != "" || dataSaved.AccountId == "") return;
                        //Console.WriteLine("HI");
                        XElement bindElement = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name.LocalName == "bind")!;
                        if (bindElement == null) return;
                        //Console.WriteLine("2");
                        if (!GlobalData.Clients.Any(e => e.accountId == dataSaved.AccountId)) // NOT found !! WW
                        {
                            XElement resourceElement = bindElement.Descendants().FirstOrDefault(i => i.Name.LocalName == "resource")!;
                            if (resourceElement == null || string.IsNullOrEmpty(resourceElement.Value)) return;

                            dataSaved.Resource = resourceElement.Value;
                            dataSaved.JID = $"{dataSaved.AccountId}@prod.ol.epicgames.com/{dataSaved.Resource}";
                          

                            XNamespace clientNs = "jabber:client";
                            XNamespace bindNs = "urn:ietf:params:xml:ns:xmpp-bind";

                            XElement featuresElement = new XElement(clientNs + "iq",
                                new XAttribute("to", dataSaved.JID),
                                new XAttribute("id", "_xmpp_bind1"),
                                new XAttribute(XNamespace.Xmlns + "jabber", clientNs.NamespaceName),
                                new XAttribute("type", "result"),
                                new XElement(bindNs + "bind",
                                    new XAttribute(XNamespace.Xmlns + "bind", bindNs.NamespaceName),
                                    new XElement(bindNs + "jid", dataSaved.JID)
                                )
                            );
                            Console.WriteLine("SNEDINGIN");
                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        }

                      
                        break;
                    case "_xmpp_session1":
                        if (!dataSaved.clientExists)
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }
                        XNamespace ClientNamespace = "jabber:client";

                        XElement featuresElement1 = new XElement(ClientNamespace + "iq",
                            new XAttribute("to", dataSaved.JID),
                            new XAttribute("from", "prod.ol.epicgames.com"),
                            new XAttribute("id", "_xmpp_session1"),
                            new XAttribute("type", "result")
                        );

                        xmlMessage = featuresElement1.ToString(SaveOptions.DisableFormatting);
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(dataSaved.AccountId);
                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId) && !profileCacheEntry.UserData.banned)
                        {
                            User UserDataParsed = profileCacheEntry.UserData;
                            UserFriends FriendsDataParsed = profileCacheEntry.UserFriends;

                            foreach (FriendsObject friendToken in FriendsDataParsed.Accepted)
                            {
                                Clients friendsClient = GlobalData.Clients.FirstOrDefault(client => client.accountId == friendToken.accountId)!;

                                if (friendsClient == null) continue; // if 1 is offline then it won't destroy it

                                XElement presence = new XElement(ClientNamespace + "presence",
                                    new XAttribute("to", dataSaved.JID),
                                    new XAttribute("type", "available"),
                                    new XAttribute("from", friendsClient.jid)
                                );

                                if (friendsClient.lastPresenceUpdate.away)
                                {
                                    presence.Add(new XElement("show", "away"));
                                }

                                presence.Add(new XElement("status", friendsClient.lastPresenceUpdate.presence));

                                xmlMessage = presence.ToString();
                                buffer = Encoding.UTF8.GetBytes(xmlMessage);
                                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);


                                //var TestIG691 = await MongoDBUser.FindOne<User>(database, "accountId", accountId);
                                //if(TestIG691.Message != "Error")
                                //{
                                //    var JsonArrayIg = JArray.Parse(TestIG691.Message)[0];

                                //    JArray accepted1 = (JArray)jsonArray["Friends"]["Accepted"];
                                //}
                                // Else nothing ig?
                            }
                        }
                        else
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }  
                        
                        break;

                    default:
                        if (!dataSaved.clientExists)
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }

                        XNamespace YA1 = "jabber:client";
                        XElement featuresElement2 = new XElement(YA1 + "iq",
                            new XAttribute("to", dataSaved.JID),
                            new XAttribute("from", "prod.ol.epicgames.com"),
                            new XAttribute("id", (string)xmlDoc.Root?.Attribute("id")!),
                            //new XAttribute(XNamespace.Xmlns + "xmlns", YA1),
                            new XAttribute("type", "result")
                        );
                        xmlMessage = featuresElement2.ToString();
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);


                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "IQ:Init");
            }

        }
    }
}
