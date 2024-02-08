namespace FortBackend.src.App.XMPP
{
    public class GlobalData
    {
        public static List<AccessToken> AccessToken { get; set; } = new List<AccessToken>();
        public static List<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();
        public static List<ExchangeCode> ExchangeCode { get; set; } = new List<ExchangeCode>();
        public static List<ClientToken> ClientToken { get; set; } = new List<ClientToken>();
    }

    public class AccessToken
    {
        public string token { get; set; }
        public string accountId { get; set; }
    }

    public class ExchangeCode
    {
        public string token { get; set; }
        public string accountId { get; set; }
    }

    public class RefreshToken
    {
        public string token { get; set; }
        public string accountId { get; set; }
    }

    public class ClientToken
    {
        public string token { get; set; }
        public string accountId { get; set; }
    }
}
