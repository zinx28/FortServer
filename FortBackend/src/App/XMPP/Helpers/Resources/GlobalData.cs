using Newtonsoft.Json;
using System.Net.WebSockets;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace FortBackend.src.App.XMPP.Helpers.Resources
{
    public class GlobalData
    {
        public static List<TokenData> AccessToken { get; set; } = new List<TokenData>();
        public static List<TokenData> RefreshToken { get; set; } = new List<TokenData>();
        public static List<ExchangeCode> ExchangeCode { get; set; } = new List<ExchangeCode>();
        public static List<TokenData> ClientToken { get; set; } = new List<TokenData>();

        public static List<Clients> Clients { get; set; } = new List<Clients>();
        public static Dictionary<string, RoomsLessDyanmic> Rooms { get; set; } = new Dictionary<string, RoomsLessDyanmic>();
        public static List<Members> members { get; set; } = new List<Members>();
        public static List<Parties> parties { get; set; } = new List<Parties>();

        public static List<Pings> pings { get; set; } = new List<Pings>();
    }

    public class Pings
    {
        public string sent_to { get; set; } = string.Empty;
        public string sent_by { get; set; } = string.Empty;
        public string time { get; set; } = string.Empty;
    }
    public class Members
    {
        public string account_id { get; set; } = string.Empty;
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public List<Dictionary<string, object>> connections { get; set; } = new List<Dictionary<string, object>>();
        public int revision { get; set; } = 0;
        public string updated_at { get; set; } = string.Empty;
        public string joined_at { get; set; } = string.Empty;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string role { get; set; } = string.Empty;
    }

    public class Connection
    {
        public string id { get; set; } = string.Empty;
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
    }

    public class Meta
    {
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public List<Connection> connections { get; set; } = new List<Connection>();
        public int revision { get; set; }
        public string updated_at { get; set; } = string.Empty;
        public string joined_at { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }

    public class Parties
    {
        public string id { get; set; } = string.Empty;
        public string privacy { get; set; } = "PUBLIC";
        public string created_at { get; set; } = string.Empty;
        public string updated_at { get; set; } = string.Empty;
        public object config { get; set; } = new object();
        public List<Members> members { get; set; } = new List<Members>();
        public List<object> applicants { get; set; } = new List<object>();
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public List<object> invites { get; set; } = new List<object>();
        public int revision { get; set; }
        public List<object> intentions { get; set; } = new List<object>();
        

    }

    public class Party {

    }

    public class ExchangeCode
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class TokenData
    {
        public string token { get; set; } = string.Empty;
        public string creation_date { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class Clients
    {
        public WebSocket Client { get; set; } = null!;
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

    public class MoreINfoINsideMembers
    {
        public string accountId { get; set; } = string.Empty;
    }
    public class RoomsLessDyanmic
    {
        public List<MoreINfoINsideMembers> members = new List<MoreINfoINsideMembers>();
    }
}
