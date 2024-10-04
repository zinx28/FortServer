using System.Text.Json.Serialization;
using FortLibrary.ConfigHelpers;
using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortMatchmaker.src.App.Utilities
{
    public class Saved
    {
        public static FortConfigMM DeserializeConfig = new FortConfigMM();
        public static CachedDataClass BackendCachedData = new CachedDataClass();
    }

    public class CachedDataClass
    {
        // AUTO CHANGES DURING RUN TIME
        public string DefaultProtocol { get; set; } = "http://";
    }

    public class ServerHotFixes
    {
        public int max_servers { get; set; } = 1;
        public int min_players { get; set; } = 20;
        public List<ServerPlaylists> playlists { get; set; } = new List<ServerPlaylists>();
    }

    public class ServerPlaylists { public string playlistId { get; set; } = string.Empty; }
}
