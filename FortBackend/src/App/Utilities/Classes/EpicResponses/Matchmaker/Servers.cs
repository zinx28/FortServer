namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Matchmaker
{
    public class Server
    {
        public string Playlist { get; set; }
        public string Ip { get; set; }
        public string buildId { get; set; }
        public string Session { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public bool JoinAble { get; set; }
        public int MaxPlayers { get; set; }
        public int Current { get; set; }
        public string Region { get; set; }
        public string CustomCode { get; set; } = "NONE";
    }

    public class Servers
    {
        public List<Server> eu { get; set; }
        public List<Server> nae { get; set; }
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
