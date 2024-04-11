using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FortBackend.src.XMPP.Data
{
    public class DataSaved_XMPP
    {
        public bool DidUserLoginNotSure = false;
        public string AccountId = string.Empty;
        public string DisplayName = string.Empty;
        public string receivedMessage = ""; // so skunky but works fine
        public bool clientExists = false;
        public string Token = string.Empty;
        public string JID = string.Empty;
        public string Resource = string.Empty;
        public string[] Rooms = new string[] { };

        public static ConcurrentDictionary<WebSocket, string> clientData = new ConcurrentDictionary<WebSocket, string>();
        public static ConcurrentDictionary<string, WebSocket> connectedClients = new ConcurrentDictionary<string, WebSocket>();
    }
}
