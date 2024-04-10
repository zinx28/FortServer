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

                    TokenData tokenData = GlobalData.AccessToken.FirstOrDefault(client => client.token == splitContent[2])!;
                    if (tokenData == null)
                    {
                        await Client.CloseClient(webSocket);
                        return;
                    }
                    Clients foundClient = GlobalData.Clients.FirstOrDefault(i => i.accountId == tokenData.accountId)!;
                    if (foundClient != null)
                    {
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
