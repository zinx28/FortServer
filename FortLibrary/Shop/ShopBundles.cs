using FortLibrary.EpicResponses.Profile.Query.Items;

namespace FortLibrary.Shop
{
    public class ShopBundles
    {
        public List<ShopBundlesItem> Daily { get; set; } = new List<ShopBundlesItem>();
        public List<ShopBundlesItem> Weekly { get; set; } = new List<ShopBundlesItem>();
        public string LastShownDate { get; set; } = string.Empty;
    }

    public class ShopItems
    {
        public string id { get; set; } = string.Empty;
        public string item { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<Item> items { get; set; } = new List<Item>();

        public string BundlePath { get; set; } = string.Empty;

        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
        public string rarity { get; set; } = string.Empty;
        public string LastShownDate { get; set; } = string.Empty;
    }

    public class ShopBundlesItem
    {
        public string id { get; set; } = string.Empty;

        public string item { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<Item> items { get; set; }
        public string rarity { get; set; } = string.Empty;

        public string BundlePath { get; set; } = string.Empty;

        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
        public int singleprice { get; set; } = -1;
        public int price { get; set; } = -1;
        public string[] categories { get; set; } = new string[0];
    }

    public class Item
    {
        public string name { get; set; } = string.Empty;
        public string item { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
    }
}
