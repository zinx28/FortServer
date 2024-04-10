namespace FortLibrary.EpicResponses.Matchmaker
{
    public class Server
    {
        public string Playlist { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public string buildId { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool JoinAble { get; set; }
        public int MaxPlayers { get; set; }
        public int Current { get; set; }
        public string Region { get; set; } = string.Empty;
        public string CustomCode { get; set; } = "NONE";
    }

    public class Servers
    {
        public List<Server> eu { get; set; } = new List<Server>();
        public List<Server> nae { get; set; } = new List<Server>();
        public static readonly object queueLock = new object();

        public static CancellationTokenSource FortniteYeahBBG = new CancellationTokenSource();



        public static List<string> VaildCodes = new List<string>
        {
            "LateGame",
            "LateGame2",
            "LateGame3",
            "DevGame",
            "DevGame2",
            "DevGame3"
        };
    }
}
