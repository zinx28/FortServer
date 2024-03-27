using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.XMPP.Helpers.Resources;
using FortBackend.src.App.XMPP.Helpers.Send;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.XMPP.Root
{
    public class Auth
    {
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved dataSaved)
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

                // I Need To Know If There Is A Better Method For This
                int startIndex = xmlDoc.ToString().IndexOf(">", StringComparison.Ordinal) + 1;
                int endIndex = xmlDoc.ToString().IndexOf("</auth>", StringComparison.Ordinal);
                string authElementContent = xmlDoc.ToString().Substring(startIndex, endIndex - startIndex);

                if (authElementContent != null)
                {
                    //Console.WriteLine("YO");
                    byte[] decodedBytes = Convert.FromBase64String(authElementContent);
                    string decodedContent = Encoding.UTF8.GetString(decodedBytes);
                    string[] splitContent = decodedContent.Split('\u0000');

                    TokenData foundClient = GlobalData.AccessToken.FirstOrDefault(client => client.token == splitContent[2]);
                    if (foundClient == null)
                    {
                        await Client.CloseClient(webSocket);
                        return;
                    }
                    Clients wow = GlobalData.Clients.FirstOrDefault(i => i.accountId == foundClient.accountId);
                    if (wow != null)
                    {
                        await Client.CloseClient(webSocket);
                        return;
                    }

                    var UserData = await Handlers.FindOne<User>("accountId", foundClient.accountId);
                    if (UserData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0];
                        if (UserDataParsed == null)
                        {
                            await Client.CloseClient(webSocket);
                            return;
                        }

                        if (UserDataParsed.banned == false)
                        {
                            dataSaved.DisplayName = UserDataParsed.Username;
                            dataSaved.AccountId = UserDataParsed.AccountId;
                            dataSaved.Token = foundClient.token;
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
                }
                await Client.CloseClient(webSocket);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Auth:INIT");
            }

            return;
        }
    }
}
