

namespace FortBackend.src.App.Utilities.Shop.Helpers.Class
{
    public class ShopBundles
    {
        public List<ShopBundlesItem> Daily { get; set; } 
        public List<ShopBundlesItem> Weekly { get; set; }
        public string LastShownDate { get; set; } = string.Empty;
    }

    public class ShopBundlesItem
    {
        public string item { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<Item> items { get; set; }
        public string rarity { get; set; } = string.Empty;
        public int singleprice { get; set; } = -1;
        public int newprice { get; set; } = -1;
        public string[] categories { get; set; } = new string[0];
    }

    public class Item
    {
        public string name { get; set; }
        public string item { get; set; }
    }
}
