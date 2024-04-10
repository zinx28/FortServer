using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortXmpp.src.App.SERVER.Root
{
    public class Open
    {
        public async static void Init(WebSocket webSocket, bool DidUserLoginNotSure, string clientId)
        {
            try
            {
                string xmlMessage;
                byte[] buffer;
                XNamespace iqAuthNs = "http://jabber.org/features/iq-auth";
                XNamespace streamNs = "http://etherx.jabber.org/streams";
                XNamespace rosterNs = "urn:xmpp:features:rosterver";
                XNamespace tlsNs = "urn:ietf:params:xml:ns:xmpp-tls";
                XNamespace bindNs = "urn:ietf:params:xml:ns:xmpp-bind";
                XNamespace compressNs = "http://jabber.org/features/compress";
                XNamespace sessionNs = "urn:ietf:params:xml:ns:xmpp-session";
                XNamespace saslNs = "urn:ietf:params:xml:ns:xmpp-sasl";
                XNamespace ns = "urn:ietf:params:xml:ns:xmpp-framing";
                // ^^ Importants

                XElement openElement = new XElement(ns + "open",
                    new XAttribute("xmlns", "urn:ietf:params:xml:ns:xmpp-framing"),
                    new XAttribute("from", "prod.ol.epicgames.com"),
                    new XAttribute("id", clientId),
                    new XAttribute("version", "1.0"),
                    new XAttribute(XNamespace.Xml + "lang", "en")
                );

                xmlMessage = openElement.ToString();
                buffer = Encoding.UTF8.GetBytes(xmlMessage);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                XElement featuresElement;
                if (DidUserLoginNotSure)
                {

                    featuresElement = new XElement(streamNs + "features",
                        new XAttribute(XNamespace.Xmlns + "stream", streamNs),
                        new XElement(rosterNs + "ver"),
                        new XElement(tlsNs + "starttls"),
                        new XElement(bindNs + "bind"),
                        new XElement(compressNs + "compression",
                        new XElement(compressNs + "method", "zlib")),
                        new XElement(sessionNs + "session")
                    );
                }
                else
                {
                    featuresElement = new XElement(streamNs + "features",
                        new XAttribute(XNamespace.Xmlns + "stream", streamNs.NamespaceName),
                        new XElement(saslNs + "mechanisms",
                            new XElement(saslNs + "mechanism", "PLAIN")
                        ),
                        new XElement(rosterNs + "ver"),
                        new XElement(tlsNs + "starttls"),
                        new XElement(compressNs + "compression",
                            new XElement(compressNs + "method", "zlib")
                        ),
                        new XElement(iqAuthNs + "auth")
                    );

                }
                xmlMessage = featuresElement.ToString();
                buffer = Encoding.UTF8.GetBytes(xmlMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Open:INIT");
            }

        }
    }
}
