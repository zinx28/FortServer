namespace FortLibrary.Dynamics
{
    public class DailyQuestsJson
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public DailyQuestsProperties Properties { get; set; } = new DailyQuestsProperties();
    }

    public class DailyQuestsProperties
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int SeasonXP { get; set; } = 0;
        public List<DailyObjectives> Objectives { get; set; } = new List<DailyObjectives>();
    }

    public class DailyObjectives
    {
        public string BackendName { get; set; } = string.Empty;
        public string ObjectiveState { get; set; } = string.Empty;
        public string ItemEvent { get; set; } = string.Empty;
        public string ItemReference { get; set; } = string.Empty;
        public string ItemTemplateIdOverride { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HudShortDescription { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
        public int Stage { get; set; } = -1;
        public bool bHidden { get; set; } = false;
    }
}
