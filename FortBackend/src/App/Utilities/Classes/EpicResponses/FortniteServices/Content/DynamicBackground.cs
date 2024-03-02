namespace FortBackend.src.App.Utilities.Classes.EpicResponses.FortniteServices.Content
{
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
