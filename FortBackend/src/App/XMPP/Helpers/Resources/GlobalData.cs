using System.Net.WebSockets;

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
        public List<Members> members { get; set; } = new List<Members>();
        public List<Parties> parties { get; set; } = new List<Parties>();
    }

    public class Members
    {
        public string account_id { get; set; } = string.Empty;
        public dynamic meta { get; set; } = string.Empty;
        public List<dynamic> connections { get; set; } = new List<dynamic> { };
        public int revision { get; set; } = 0;
        public string updated_at { get; set; } = string.Empty;
        public string joined_at { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }
    public class Parties
    {
        public string id { get; set; } = string.Empty;
        public string privacy { get; set; } = "PUBLIC";
        public List<dynamic> members { get; set; } = new List<dynamic>();
        public Party party { get; set; } = new Party();
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
