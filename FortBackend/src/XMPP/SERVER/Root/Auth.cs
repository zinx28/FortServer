using FortLibrary.MongoDB.Module;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortLibrary;
using FortBackend.src.XMPP.Data;
using FortBackend.src.App.SERVER.Send;
using FortLibrary.XMPP;
using FortBackend.src.App.Utilities.MongoDB.Helpers;

namespace FortBackend.src.App.SERVER.Root
{
    public class Auth
    {
        public static HttpClient httpClient = new HttpClient();
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, /*string AccountId,*/ DataSaved dataSaved)
        {
            try
            {
                Console.WriteLine("CLIENTDI: " + clientId);
                string xmlMessage;
                byte[] buffer;
                if (clientId == null)
                {
                    await Client.CloseClient(webSocket);
                    return;
                }

                int startIndex = xmlDoc.ToString().IndexOf(">", StringComparison.Ordinal) + 1; // skunked
                int endIndex = xmlDoc.ToString().IndexOf("</auth>", StringComparison.Ordinal);
                string authElementContent = xmlDoc.ToString().Substring(startIndex, endIndex - startIndex);

                if (authElementContent != null)
                {
                    byte[] decodedBytes = Convert.FromBase64String(authElementContent);
                    string decodedContent = Encoding.UTF8.GetString(decodedBytes);
                    string[] splitContent = decodedContent.Split('\u0000');

                    if (splitContent.Length == 3)
                    {
                        var AccessToken = splitContent[2].Replace("eg1~", "");
                        Console.WriteLine(AccessToken);
                        TokenData AccessTokenClient = GlobalData.AccessToken.FirstOrDefault(i => i.token == splitContent[2])!;
                        if (AccessTokenClient == null/* && string.IsNullOrEmpty(AccessTokenClient.accountId)*/)
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }

                        Clients foundClient = GlobalData.Clients.FirstOrDefault(i => i.accountId == AccessTokenClient.accountId)!;
                        if (foundClient != null)
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }

                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(AccessTokenClient.accountId, false);

                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            if (!profileCacheEntry.UserData.banned)
                            {
                                dataSaved.DisplayName = profileCacheEntry.UserData.Username;
                                dataSaved.AccountId = profileCacheEntry.UserData.AccountId;
                                dataSaved.Token = AccessTokenClient.token;
                                if (decodedContent != "" && dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "" && splitContent.Length == 3)
                                {
                                    dataSaved.DidUserLoginNotSure = true;
                                    Console.WriteLine($"New Xmpp Client Logged In User Name Is As {dataSaved.DisplayName}");
                                    XNamespace streamNs = "urn:ietf:params:xml:ns:xmpp-sasl";
                                    var featuresElement = new XElement(streamNs + "success",
                                           new XAttribute(XNamespace.None + "xmlns", "urn:ietf:params:xml:ns:xmpp-sasl")
                                    );
                                    xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                                    buffer = Encoding.UTF8.GetBytes(xmlMessage);
                                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // wtfff!!
                            await Client.CloseClient(webSocket);
                            return;
                        }


                        /*   // lUNA prod was terribe fuck it
                           TokenData AccessTokenClient = GlobalData.AccessToken.FirstOrDefault(i => i.token == splitContent[2])!;
                       if (AccessTokenClient == null)
                       {
                           await Client.CloseClient(webSocket);
                           return;
                       }

                       Clients foundClient = GlobalData.Clients.FirstOrDefault(i => i.accountId == AccessTokenClient.accountId)!;
                       if (foundClient != null)
                       {
                           await Client.CloseClient(webSocket);
                           return;
                       }*/


                    }
                  



                   /* if (splitContent.Length == 3)
                    {
                        var AccessToken = splitContent[2].Replace("eg1~", "");
                        Console.WriteLine(AccessToken);
                        Clients foundClient = GlobalData.Clients.FirstOrDefault(i => i.token == AccessToken)!;
                        if (foundClient != null)
                        {
                            foundClient.Game_Client = webSocket;
                            AccountId = foundClient.accountId;
                            // for future calls if needed



                            Console.WriteLine($"New Xmpp Client Logged In User Name Is As {foundClient.displayName} ~ found old data from launcher");

                            XNamespace streamNs = "urn:ietf:params:xml:ns:xmpp-sasl";
                            var featuresElement = new XElement(streamNs + "success",
                                    new XAttribute(XNamespace.None + "xmlns", "urn:ietf:params:xml:ns:xmpp-sasl")
                            );
                            xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);
                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                            return;
                            
                        }else
                        {
                            Console.WriteLine("INVAILD ACCOUNT");
                        }
                    }*/
                   
                }
                await Client.CloseClient(webSocket);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Auth:INIT");
            }

            return;
        }
    }
}
