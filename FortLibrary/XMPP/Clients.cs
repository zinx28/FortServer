using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.XMPP
{
    public class Clients
    {
        public WebSocket Launcher_Client { get; set; } = null!;
        public WebSocket Game_Client { get; set; } = null!;
        public DataSaved DataSaved { get; set; } = new DataSaved();
        public string DiscordId { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string jid { get; set; } = string.Empty;
        public string resource { get; set; } = string.Empty;
        public lastPresenceUpdate lastPresenceUpdate { get; set; } = new lastPresenceUpdate();
        public string accountId { get; set; } = string.Empty;

        // PARTY V2? stuff i think~
        public string id = "";
        public Dictionary<string, object> meta = new Dictionary<string, object>();
        public int revision = 0;
    }

    public class lastPresenceUpdate
    {
        public bool away = false;
        public string presence = "{}";
    }
}
