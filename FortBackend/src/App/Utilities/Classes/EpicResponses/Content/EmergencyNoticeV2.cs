namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Content
{
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

}
