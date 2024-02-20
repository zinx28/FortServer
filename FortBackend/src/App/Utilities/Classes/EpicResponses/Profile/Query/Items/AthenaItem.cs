using Microsoft.Extensions.Primitives;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items
{

    public class AthenaItem
    {
        public string templateId { get; set; } = string.Empty;
        public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
        public int quantity { get; set; } = 1;

    }
    public class AthenaItemAttributes
    {
        public bool favorite { get; set; } = false;
        public bool item_seen { get; set; } = false;
        public int level { get; set; } = 1;
        public int max_level_bonus { get; set; } = 0;
        public int rnd_sel_cnt { get; set; } = 0;
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
        public int xp { get; set; } = 0;
    }

    public class AthenaItemVariants
    {
        public string channel { get; set; } = string.Empty;
        public string active { get; set; } = string.Empty;
        public string[] owned { get; set; } = { };
    }
}
