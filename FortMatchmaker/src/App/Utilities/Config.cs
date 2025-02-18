using System.Text.Json.Serialization;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Matchmaker;
using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortMatchmaker.src.App.Utilities
{
    public class Saved
    {
        public static FortConfigMM DeserializeConfig = new FortConfigMM();
        public static CachedDataClass BackendCachedData = new CachedDataClass();
        public static ServerHotFixes serverHotFixes = new();

        public static List<Server> CurrentServers = new();
    }

    //public class Servers
    //{
    //    public string ID { get; set; } = "";
    //    public bool bHasStarted { get; set; } = false;
    //    public bool bJoinable { get; set; } = false;
    //    public bool bServersLaunching { get; set; } = true; // set to false if you want it to say waiting for server on gameserver injection
    //    public string PlaylistID { get; set; } = "playlist_defaultsolo";
    //    public string Region { get; set; } = "EU";
    //    public int Players { get; set; } = 0;
    //    public int AlivePlayers { get; set; } = -1; // idrk how to do this wihtout kids using http requests on gs
    //    public int MaxPlayers { get; set; } = 10; // hotfix chabnges this
    //}

    public class CachedDataClass
    {
        // AUTO CHANGES DURING RUN TIME
        public string DefaultProtocol { get; set; } = "http://";
    }

    public class ServerHotFixes
    {
        public int max_servers { get; set; } = 1;
        public int max_players { get; set; } = 100; // gs should do this but skunked matchmaker
        public int min_players { get; set; } = 20; // min players till new match open BUT if theres no matches open it will bypass this
        public List<ServerPlaylists> playlists { get; set; } = new List<ServerPlaylists>();
    }

    public class ServerPlaylists { public string playlistId { get; set; } = string.Empty; }
}
