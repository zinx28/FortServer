using FortBackend.src.App.SERVER.Send;
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
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved_XMPP dataSaved)
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
                //  Console.WriteLine(xmlDoc.Root?.Attribute("id")?.Value);
                switch (xmlDoc.Root?.Attribute("id")?.Value)
                {
                    case "_xmpp_bind1":
                        if (dataSaved.Resource != "" || dataSaved.AccountId == "") return;
                        XElement bindElement = xmlDoc.Root?.Descendants().FirstOrDefault(i => i.Name.LocalName == "bind")!;
                        if (bindElement == null) return;

                        if (GlobalData.Clients.Any(e => e.accountId == dataSaved.AccountId))
                        {
                            Clients FindClient = GlobalData.Clients.FirstOrDefault(i => i.accountId == dataSaved.AccountId)!;
                            if (FindClient == null) { await Client.CloseClient(webSocket); break; }

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

                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        }

                      
                        break;
                    case "_xmpp_session1":
                        if (!dataSaved.clientExists) { await Client.CloseClient(webSocket); return; }
                        XNamespace YA = "jabber:client";

                        XElement featuresElement1 = new XElement(YA + "iq",
                            new XAttribute("to", dataSaved.JID),
                            new XAttribute("from", "prod.ol.epicgames.com"),
                            new XAttribute("id", "_xmpp_session1"),
                            new XAttribute("type", "result")
                        );

                        xmlMessage = featuresElement1.ToString(SaveOptions.DisableFormatting);
                        buffer = Encoding.UTF8.GetBytes(xmlMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        
                        Console.WriteLine(dataSaved.AccountId);

                        HttpClient httpClient = new HttpClient();
                        HttpResponseMessage response = await httpClient.GetAsync($"{Saved.DeserializeConfig.DefaultProtocol}127.0.0.1:{Saved.DeserializeConfig.BackendPort}/PRIVATE/DEVELOPER/DATA/{dataSaved.AccountId}");

                        if (response.IsSuccessStatusCode)
                        {
                            var datareturned = await response.Content.ReadAsStringAsync();
                            if (datareturned != null)
                            {
                                ProfileCacheEntry Data = JsonConvert.DeserializeObject<ProfileCacheEntry>(datareturned)!;
                                if (!string.IsNullOrEmpty(Data.AccountId) && Data.AccountData != null)
                                {
                                    User UserDataParsed = Data.UserData;
                                    UserFriends FriendsDataParsed = Data.UserFriends;

                                    List<FriendsObject> accepted = FriendsDataParsed.Accepted;

                                    foreach (FriendsObject friendToken in accepted)
                                    {
                                        string accountId = friendToken.accountId;
                                        Clients letssee = GlobalData.Clients.FirstOrDefault(client => client.accountId == accountId)!;

                                        if (letssee == null) return;

                                        XNamespace clientNs1 = "jabber:client";
                                        XElement presence = new XElement(clientNs1 + "presence",
                                            new XAttribute("to", dataSaved.JID),
                                            new XAttribute("from", letssee.jid),
                                            new XAttribute("type", "available")
                                        );

                                        if (letssee.lastPresenceUpdate.away)
                                        {
                                            presence.Add(new XElement("show", "away"));
                                            presence.Add(new XElement("status", letssee.lastPresenceUpdate.presence));
                                        }
                                        else
                                        {
                                            presence.Add(new XElement("status", letssee.lastPresenceUpdate.presence));
                                        }
                
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
                            }else
                            {

                            }
                        }
                        else
                        {
                            Console.WriteLine(response);
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
