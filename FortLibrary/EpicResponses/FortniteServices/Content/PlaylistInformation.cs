namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class PlaylistInformation
    {
        public Conversionconfig conversion_config { get; set; } = new Conversionconfig();

        public bool is_tile_hidden { get; set; } = false;
        public string frontend_matchmaking_header_style { get; set; } = "Basic";
        public string _title { get; set; } = "playlistinformation";
        public string frontend_matchmaking_header_text_description { get; set; } = "Watch @ Legends Landing";
        public string frontend_matchmaking_header_text { get; set; } = "FNCS Last Chance Major";

        public Playlistinfo playlist_info { get; set; } = new Playlistinfo();

        public bool _noIndex { get; set; } = false;
        public string _activeDate { get; set; } = "2020-00-00T00:00:00.336Z";
        public string lastModified { get; set; } = "2023-08-21T03:49:48.336Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameMOTD";
    }

    public class Playlistinfo
    {
        public string _type { get; set; } = "Playlist Information";
        public List<PlayListObject> playlists { get; set; } = new List<PlayListObject>();
    }

    public class PlayListObject
    {
        public string image { get; set; } = string.Empty;
        public string playlist_name { get; set; } = string.Empty;
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "FortPlaylistInfo";
        public string description { get; set; } = string.Empty ;
        public string display_name { get; set; } = string.Empty;
    }

    public class Conversionconfig
    {
        public string containerName { get; set; } = "playlist_info";
        public string _type { get; set; } = "Conversion Config";
        public string contentName { get; set; } = "playlists";
        public bool enableReferences { get; set; } = true;
    }
}
