using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Content
{
    public class ContentJson
    {
        [JsonPropertyName("jcr:isCheckedOut")]
        public bool isCheckedOut { get; set; } = false;

        public string _title { get; set; } = "Fortnite Game";

        [JsonPropertyName("jcr:baseVersion")]
        public string baseVersion { get; set; } = "a7ca237317f1e7883b3279";

        public string _activeDate { get; set; } = "2017-08-30T03:20:48.050Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.334Z";
        public string _locale { get; set; } = "en_US";
        public BattleRoyaleNews battleroyalenews { get; set; } = new BattleRoyaleNews();
        public BattleRoyaleNewsV2 battleroyalnewsv2 { get; set; } = new BattleRoyaleNewsV2();
        public EmergencyNotice emergencynotice { get; set; } = new EmergencyNotice();
        public EmergencyNoticeV2 emergencynoticev2 { get; set; } = new EmergencyNoticeV2();
        public DynamicBackground dynamicbackgrounds { get; set; } = new DynamicBackground();
    }

    public class BattleRoyaleNews
    {
        public NewContent news { get; set; } = new NewContent();
    };

    public class NewContent
    {
        public string _type { get; set; } = "Battle Royale News";
        public List<NewContentMotds> motds { get; set; } = new List<NewContentMotds>();
        public List<NewContentMessages> messages { get; set; } = new List<NewContentMessages>();
        public string _title { get; set; } = "battleroyalenews";
        public string header { get; set; } = "";
        public bool _noIndex { get; set; } = false;
        public bool alwaysShow { get; set; } = false;
        public string style { get; set; } = "SpecialEvent";
        public string _activeData { get; set; } = "2023-11-09T18:08:17.347Z";
        public string _locale { get; set; } = "en-US";
    };

    public class NewContentMotds
    {
        public string image { get; set; } = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string entrytype { get; set; } = "normal";
        public string messagetype { get; set; } = "normal";
        public string title { get; set; } = "Fortnite CHapter 2 seaosn";
        public string body { get; set; } = "temp";
        public bool spotlight { get; set; } = false;
    }

    public class NewContentMessages
    {
        public string image { get; set; } = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string entrytype { get; set; } = "normal";
        public string messagetype { get; set; } = "normal";
        public string title { get; set; } = "Fortnite CHapter 2 seaosn";
        public string body { get; set; } = "temp";
        public bool spotlight { get; set; } = false;
    }
    public class BattleRoyaleNewsV2
    {
        public NewContentV2 news { get; set; } = new NewContentV2();

        public string _title { get; set; } = "battleroyalenewsv2";
        public bool _noIndex { get; set; } = false;
        public bool alwaysShow { get; set; } = true;
        public string _activeData { get; set; } = "2023-11-09T18:08:17.347Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.347Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameMOTD";
        public string[] _suggestedPrefetch = new string[0];
    };

    public class NewContentV2
    {
        public List<NewContentV2Motds> motds { get; set; } = new List<NewContentV2Motds>();
        public string _type { get; set; } = "Battle Royale News V2";
    };

    public class NewContentV2Motds
    {
        public string image { get; set; } = "https://cdn2.unrealengine.com/ch4s2-lobbyupdate-4-20-2022-lifted-copy-3840x2160-d3a138f5f9e7.jpg";
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string entrytype { get; set; } = "normal";
        public string messagetype { get; set; } = "normal";
        public string title { get; set; } = "Fortnite CHapter 2 seaosn";
        public string body { get; set; } = "temp";
        public bool spotlight { get; set; } = true;
    };

    public class EmergencyNotice
    {
        public EmergencyNoticeNews news { get; set; } = new EmergencyNoticeNews();
    };

    public class EmergencyNoticeNews
    {
        public List<object> platform_messages { get; set; } = new List<object>();
        public string _type { get; set; } = "Battle Royale News";
        public List<EmergencyNoticeNewsMessages> messages { get; set; } = new List<EmergencyNoticeNewsMessages>();
        public string _title { get; set; } = "emergencynotice";
        public string _activeDate { get; set; } = "2023-11-09T18:08:17.347Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.347Z";
        public string _locale { get; set; } = "en-US";
    };

    public class EmergencyNoticeNewsMessages
    {
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string subgame { get; set; } = "br";
        public string title { get; set; } = "Fortnite CHapter 2 seaosn";
        public string body { get; set; } = "temp";
        public bool spotlight { get; set; } = true;
    }

    public class EmergencyNoticeV2
    {
        public string _title { get; set; } = "emergencynoticev2";

        public bool _noIndex { get; set; } = false;
        public EmergencyNoticeNewsV2 emergencynotices { get; set; } = new EmergencyNoticeNewsV2();
        public string _activeDate { get; set; } = "2023-11-09T18:08:17.347Z";
        public string lastModified { get; set; } = "2024-02-10T18:38:12.907Z";
        public string _locale { get; set; } = "en-US";

        public string _templateName = "FortniteGameMOTD";
        public string[] _suggestedPrefetch = new string[0];
    };

    public class EmergencyNoticeNewsV2
    {
        public string _type { get; set; } = "Battle Royale News";
        public List<EmergencyNoticeNewsV2Messages> emergencynotices { get; set; } = new List<EmergencyNoticeNewsV2Messages>();
    };

    public class EmergencyNoticeNewsV2Messages
    {
        public bool hidden { get; set; } = false;
        public string _type { get; set; } = "CommonUI Simple Message Base";
        public string title { get; set; } = "Fortnite CHapter 2 seaosn";
        public string body { get; set; } = "temp";
    }

    public class DynamicBackground
    {
        public DynamicBackgrounds backgrounds { get; set; } = new DynamicBackgrounds();
        public string _title { get; set; } = "dynamicbackgrounds";
        public bool _noIndex { get; set; } = false;
        public string _activeDate { get; set; } = "2023-11-09T18:08:17.347Z";
        public string lastModified { get; set; } = "2023-11-09T18:08:17.347Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameDynamicBackgrounds";
        public List<object> _suggestedPrefetch { get; set; } = new List<object>();
    };

    public class DynamicBackgrounds
    {
        public List<DynamicBackgroundList> backgrounds { get; set; } = new List<DynamicBackgroundList>();
        public string _type { get; set; } = "DynamicBackgroundList";
    };

    public class DynamicBackgroundList
    {
        public string stage { get; set; } = "season2";
        public string _type { get; set; } = "DynamicBackground";
        public string key { get; set; } = "lobby";
    };

}
