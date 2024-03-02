namespace FortBackend.src.App.Utilities.Classes.EpicResponses.FortniteServices.Content
{
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
        public string title { get; set; } = "FortBackend";
        public string body { get; set; } = "Fortnite E-Kittens :3";
        public bool spotlight { get; set; } = true;
    };
}
