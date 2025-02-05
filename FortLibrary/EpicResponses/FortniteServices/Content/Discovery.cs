using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class Discovery
    {
        public List<PanelArray> Panels { get; set; } = new();
        public string[] TestCohorts { get; set; } = new string[0];
        public object ModeSets { get; set; } = new();
    }

    public class PanelArray
    {
        public string PanelName { get; set; } = "FortBackend :3";
        public List<PagesArray> Pages { get; set; } = new();
    }

    public class PagesArray
    {
        public List<ResultsArray> results { get; set; } = new();
        public bool hasMore { get; set; } = false;
    }

    public class ResultsArray
    {
        public LinkDataObject linkData { get; set; } = new();
        public bool isFavorite { get; set; } = false;
        public string lastVisited { get; set; }
        public string linkCode { get; set; } = "playlist_defaultsquad";
    }

    public class LinkDataObject
    {
        public string mnemonic { get; set; } = "playlist_defaultsquad";
        public string linkType { get; set; } = "BR:Playlist";
        public bool active { get; set; } = true;
        public bool disabled { get; set; } = false;
        public int version { get; set; } = 1;
        public string moderationStatus { get; set; } = "Unmoderated";
        public string accountId { get; set; } = "epic";
        public string creatorName { get; set; } = "Epic";
        public string[] descriptionTags { get; set; } = new string[0];
        public new Dictionary<string, dynamic> metadata { get; set; } = new Dictionary<string, dynamic>()
        {
            {
                "matchmaking", new Dictionary<string, string>() {
                    {
                        "override_playlist", "playlist_defaultsquad"
                    }
                }
            }
        };
    }
}
