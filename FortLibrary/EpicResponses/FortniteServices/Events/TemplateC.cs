namespace FortLibrary.EpicResponses.FortniteServices.Events
{
    public class TemplateC
    {
        public string eventTemplateId { get; set; } = string.Empty;
        public string gameId { get; set; } = "Fortnite";
        public int matchCap { get; set; } = 100;
        public string persistentScoreId { get; set; } = "Hype";
        public string playlistId { get; set; } = string.Empty;
        public List<object> scoringRules { get; set; } = new List<object>();
    }
}
