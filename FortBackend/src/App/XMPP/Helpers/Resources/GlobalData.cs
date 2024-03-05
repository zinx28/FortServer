using Newtonsoft.Json;
using System.Net.WebSockets;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

namespace FortBackend.src.App.XMPP.Helpers.Resources
{
    public class GlobalData
    {
        public static List<AccessToken> AccessToken { get; set; } = new List<AccessToken>();
        public static List<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();
        public static List<ExchangeCode> ExchangeCode { get; set; } = new List<ExchangeCode>();
        public static List<ClientToken> ClientToken { get; set; } = new List<ClientToken>();

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
        public string id { get; set; }
        public Dictionary<string, object> meta { get; set; }
    }

    public class Meta
    {
        public Dictionary<string, object> meta { get; set; }
        public List<Connection> connections { get; set; }
        public int revision { get; set; }
        public string updated_at { get; set; }
        public string joined_at { get; set; }
        public string role { get; set; }
    }

    public class Parties
    {
        public string id { get; set; } = string.Empty;
        public string privacy { get; set; } = "PUBLIC";
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public object config { get; set; }
        public List<Members> members { get; set; }
        public List<object> applicants { get; set; }
        public Dictionary<string, object> meta { get; set; }
        public List<object> invites { get; set; }
        public int revision { get; set; }
        public List<object> intentions { get; set; }
        

    }

    public class Party {

    }
    public class AccessToken
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class ExchangeCode
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class RefreshToken
    {
        public string token { get; set; } = string.Empty;
        public string creation_date { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class ClientToken
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

    public class Clients
    { 
        public WebSocket Client { get; set; }
        public string displayName { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string jid { get; set; } = string.Empty;
        public string resource { get; set; } = string.Empty;
        public lastPresenceUpdate lastPresenceUpdate { get; set; }
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
