using FortLibrary.XMPP;

namespace FortBackend.src.XMPP.Data
{
    public class GlobalData
    {
        public static List<TokenData> AccessToken { get; set; } = new List<TokenData>();
        public static List<TokenData> RefreshToken { get; set; } = new List<TokenData>();
        // public static List<ExchangeCode> ExchangeCode { get; set; } = new List<ExchangeCode>();
        public static List<TokenData> ClientToken { get; set; } = new List<TokenData>();

        public static List<Clients> Clients { get; set; } = new List<Clients>();
        public static Dictionary<string, RoomsData> Rooms { get; set; } = new Dictionary<string, RoomsData>();
        public static List<Members> members { get; set; } = new List<Members>();
        public static List<Parties> parties { get; set; } = new List<Parties>();

        public static List<Pings> pings { get; set; } = new List<Pings>();
    }
}

/*
 *  public class ExchangeCode
    {
        public string token { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
    }

*/