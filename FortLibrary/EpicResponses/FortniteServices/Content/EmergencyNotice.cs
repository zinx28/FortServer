namespace FortLibrary.EpicResponses.FortniteServices.Content
{
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
}
