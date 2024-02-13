namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items
{

    public class AthenaItem
    {
        public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
        public string templateId { get; set; } = string.Empty;
        public int quantity { get; set; } = 1;

    }
    public class AthenaItemAttributes
    {
        public bool favorite { get; set; }
        public bool item_seen { get; set; }
        public int level { get; set; }
        public int max_level_bonus { get; set; }
        public int rnd_sel_cnt { get; set; }
        public List<object> variants { get; set; } = new List<object>();
        public int xp { get; set; }
    }
}
