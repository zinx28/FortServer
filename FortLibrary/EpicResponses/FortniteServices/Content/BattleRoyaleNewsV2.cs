using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class BattleRoyaleNewsV2
    {
        

        public NewContentV2 news { get; set; } = new NewContentV2();
        public string _title { get; set; } = "battleroyalenewsv2";
        public bool _noIndex { get; set; } = false;
        public string header { get; set; } = "";
        public bool alwaysShow { get; set; } = false;
        public string style { get; set; } = "None";
        public string _activeData { get; set; } = "2023-11-09T18:08:17.347Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.347Z";

        [JsonPropertyName("jcr:isCheckedOut")]
        [JsonProperty("jcr:isCheckedOut")]
        public bool isCheckedOut { get; set; } = false;

        [JsonPropertyName("jcr:baseVersion")]
        [JsonProperty("jcr:baseVersion")]
        public string baseVersion { get; set; } = "";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameMOTD";
       // public string[] _suggestedPrefetch = new string[0];
    };

    public class NewContentV2
    {
        public List<NewContentV2Motds> motds { get; set; } = new List<NewContentV2Motds>();
        public string _type { get; set; } = "Battle Royale News V2";

    };

    public class NewContentV2Motds
    {

        public string image { get; set; } = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
        public string tileImage { get; set; } = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message MOTD";
        public string entrytype { get; set; } = "Website"; // "normal"
        public string messagetype { get; set; } = "normal";
        public string tabTitleOverride { get; set; } = "FortBackend";
        public string title { get; set; } = "FortBackend";
        public string body { get; set; } = "Fortnite E-Kittens :3";
        public int sortingPriority { get; set; } = 0;
        public string id { get; set; } = "femboy69";
        public bool videoStreamingEnabled { get; set; } = false;
        public bool videoLoop { get; set; } = false;
        public bool videoMute { get; set; } = false;
        public bool videoAutoplay { get; set; } = false;
        public bool videoFullscreen { get; set; } = false;
        public bool spotlight { get; set; } = true;
        public string websiteURL { get; set; } = "https://github.com/zinx28/FortServer";
        public string websiteButtonText { get; set; } = "Github!!";
    };
}
