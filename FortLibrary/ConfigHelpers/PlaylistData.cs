namespace FortLibrary.ConfigHelpers
{
    public class PlaylistDataClass
    {
        public string PlaylistName { get; set; } = string.Empty;
        public Playlist_Access PlaylistAccess { get; set; } = new Playlist_Access();
    }

    public class Playlist_Access
    {
        public bool bEnabled { get; set; } = true;
        public bool bIsDefaultPlaylist { get; set; } = true;
        public bool bVisibleWhenDisabled { get; set; } = true;
        public bool bDisplayAsNew { get; set; } = false;
        public int CategoryIndex { get; set; } = 0;
        public bool bDisplayAsLimitedTime { get; set; } = false;
        public int DisplayPriority { get; set; } = 0;
    }
}
