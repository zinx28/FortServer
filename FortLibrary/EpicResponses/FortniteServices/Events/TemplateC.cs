namespace FortLibrary.EpicResponses.FortniteServices.Events
{
    public class TemplateC
    {
        public string eventTemplateId { get; set; } = string.Empty;
        public string gameId { get; set; } = "Fortnite";
        public int matchCap { get; set; } = 100;
        public string? persistentScoreId { get; set; } = null;
        public string playlistId { get; set; } = string.Empty;
        public List<object> scoringRules { get; set; } = new List<object>();
        public object? tiebreakerFormula { get; set; } = null;
        public List<object>? payoutTable { get; set; } = null;
        public List<object>? liveSessionAttributes { get; set; } = null;
    }
}
