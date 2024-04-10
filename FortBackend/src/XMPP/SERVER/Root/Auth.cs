using FortLibrary.MongoDB.Module;
using FortXmpp.src.App.Globals;
using FortXmpp.src.App.Globals.Data;
using FortXmpp.src.App.SERVER.Send;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using FortLibrary;

namespace FortXmpp.src.App.SERVER.Root
{
    public class Auth
    {
        public static HttpClient httpClient = new HttpClient();
        public async static void Init(WebSocket webSocket, XDocument xmlDoc, string clientId, DataSaved_XMPP dataSaved)
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
                    
                    if(splitContent.Length == 3)
                    {
                        var AccessToken = splitContent[2].Replace("eg1~", "");
                        Console.WriteLine(AccessToken);
                        Clients foundClient = GlobalData.Clients.FirstOrDefault(i => i.token == AccessToken)!;
                        if (foundClient != null)
                        {
                            dataSaved.DisplayName = foundClient.displayName;
                            dataSaved.AccountId = foundClient.accountId;
                            dataSaved.Token = foundClient.token;
                            if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "")
                            {
                                dataSaved.DidUserLoginNotSure = true;
                                dataSaved.clientExists = true;

                                Console.WriteLine($"New Xmpp Client Logged In User Name Is As {dataSaved.DisplayName} ~ found old data from launcher");

                                XNamespace streamNs = "urn:ietf:params:xml:ns:xmpp-sasl";
                                var featuresElement = new XElement(streamNs + "success",
                                       new XAttribute(XNamespace.None + "xmlns", "urn:ietf:params:xml:ns:xmpp-sasl")
                                );
                                xmlMessage = featuresElement.ToString(SaveOptions.DisableFormatting);
                                buffer = Encoding.UTF8.GetBytes(xmlMessage);
                                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                                return;
                            }
                        }else
                        {
                            Console.WriteLine("INVAILD ACCOUNT");
                        }
                    }
                   
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
