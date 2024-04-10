using Newtonsoft.Json;

namespace FortLibrary.EpicResponses.Profile.Query.Items
{

    public class AthenaItem
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
        [JsonProperty("quantity")]
        public int quantity { get; set; } = 1;

    }

    // for dynamic, and i dicti
    public class AthenaItemDynamic
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public Dictionary<string, object> attributes { get; set; } = new Dictionary<string, object>();
        [JsonProperty("quantity")]
        public int quantity { get; set; } = 1;

    }
    public class AthenaItemAttributes
    {
        [JsonProperty("favorite")]
        public bool favorite { get; set; } = false;
        [JsonProperty("item_seen")]
        public bool item_seen { get; set; } = false;
        [JsonProperty("level")]
        public int level { get; set; } = 1;
        [JsonProperty("max_level_bonus")]
        public int max_level_bonus { get; set; } = 0;
        [JsonProperty("rnd_sel_cnt")]
        public int rnd_sel_cnt { get; set; } = 0;
        [JsonProperty("variants")]
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
        [JsonProperty("xp")]
        public int xp { get; set; } = 0;
    }

    public class AthenaItemVariants
    {
        [JsonProperty("channel")]
        public string channel { get; set; } = string.Empty;
        [JsonProperty("active")]
        public string active { get; set; } = string.Empty;
        [JsonProperty("owned")]
        public string[] owned { get; set; } = { };
    }
}
