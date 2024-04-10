namespace FortLibrary.EpicResponses.FortniteServices.Events
{
    public class EventC
    {
        public string announcementTime { get; set; } = string.Empty;
        public string appId { get; set; } = string.Empty;
        public string beginTime { get; set; } = string.Empty;
        public string displayDataId { get; set; } = string.Empty;
        public string endTime { get; set; } = string.Empty;

        public string environment { get; set; } = string.Empty;
        public string eventGroup { get; set; } = string.Empty;
        public string eventId { get; set; } = string.Empty;
        public List<EventWindowC> eventWindows { get; set; } = new List<EventWindowC>();
        public string gameId { get; set; } = string.Empty;
        public string link { get; set; } = string.Empty;
        public MetaDataC metadata { get; set; } = new MetaDataC()
        {
            TrackedStats = new string[] {
                "PLACEMENT_STAT_INDEX",
                "TEAM_ELIMS_STAT_INDEX",
                "MATCH_PLAYED_STAT"
            },
            minimumAccountLevel = 0,
        };
        public object platformMappings { get; set; } = new { };
        public string[] platforms { get; set; } = new string[] {
            "PS4",
            "XboxOne",
            "Switch",
            "Android",
            "IOS",
            "Windows"
        };
        public object regionMappings { get; set; } = new { };
        public string[] regions { get; set; } = new string[] {
            "NAE",
            "ME",
            "NAW",
            "OCE",
            "ASIA",
            "EU",
            "BR"
        };
    }

    public class MetaDataC
    {
        public string[] TrackedStats { get; set; } = new string[0];
        public int minimumAccountLevel { get; set; } = 0;
    }

    public class EventWindowC
    {
        public string[] additionalRequirements { get; set; } = new string[0];
        public string beginTime { get; set; } = string.Empty;
        public string[] blackoutPeriods { get; set; } = new string[0];
        public bool canLiveSpectate { get; set; } = false;
        public string countdownBeginTime { get; set; } = string.Empty;
        public string endTime { get; set; } = string.Empty;
        public string eventTemplateId { get; set; } = string.Empty;
        public string eventWindowId { get; set; } = string.Empty;
        public bool isTBD { get; set; } = false;
        public EventWindowMetaDataC metadata { get; set; } = new EventWindowMetaDataC();
        public int payoutDelay { get; set; } = 30;
        public string[] requireAllTokens { get; set; } = new string[0];
        public string[] requireAllTokensCaller { get; set; } = new string[0];
        public string[] requireAnyTokens { get; set; } = new string[0];
        public string[] requireAnyTokensCaller { get; set; } = new string[0];
        public string[] requireNoneTokensCaller { get; set; } = new string[0];
        public int round { get; set; } = 0;
        public List<object> scoreLocations { get; set; } = new List<object>();
        public string teammateEligibility { get; set; } = "any";
        public string visibility { get; set; } = "public";
    }

    public class EventWindowMetaDataC
    {
        public string RoundType { get; set; } = string.Empty;
        public Int64 ThresholdToAdvanceDivision { get; set; } = 0;
        public int divisionRank { get; set; } = 0;
    }
}
