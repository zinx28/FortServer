
using FortLibrary.EpicResponses.Profile.Query.Items;
namespace FortLibrary.Shop
{
    public class SavedData
    {
        public List<ItemsSaved> Weekly { get; set; } = new List<ItemsSaved>();
        public List<ItemsSaved> Daily { get; set; } = new List<ItemsSaved>();

        // copied it from like a very old backend version
        public List<object> WeeklyFields { get; set; } = new List<object>();
        public List<object> DailyFields { get; set; } = new List<object>();

        public int Season = 15;
        public int WeeklyItems;
        public int DailyItems;
    }

    public class ItemsSaved
    {
        public string devName { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string item { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<Item> items { get; set; } = new List<Item>();
        public int price { get; set; } = -1;
        public int singleprice { get; set; } = -1;
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();

        public List<MetaInfo> metaInfo { get; set; } = new List<MetaInfo>();
        public string BundlePath { get; set; } = string.Empty;

        public string displayAssetPath { get; set; } = string.Empty;
        public string rarity { get; set; } = string.Empty;
        public string type = string.Empty;
        public string[] categories { get; set; } = new string[0];

        public int sortPriority { get; set; } = 0;
        public int catalogGroupPriority { get; set; } = 0;

        public int MinLevel { get; set; } = 0;
    }

    public class MetaInfo
    {
        public string key { get; set; } = string.Empty;
        public dynamic value { get; set; }
    }
}
