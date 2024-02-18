

namespace FortBackend.src.App.Utilities.Shop.Helpers.Class
{
    public class ShopBundles
    {
        public List<ShopBundlesItem> Daily { get; set; } 
        public List<ShopBundlesItem> Weekly { get; set; }
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

        public List<Variants> variants { get; set; } = new List<Variants>();
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
        public int singleprice { get; set; } = -1;
       // public int newprice { get; set; } = -1;
        public string[] categories { get; set; } = new string[0];
    }

    public class Item
    {
        public string name { get; set; } = string.Empty;
        public string item { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public List<Variants> variants { get; set; } = new List<Variants>();
    }

    public class Variants
    {
        public string channel { get; set; }
        public string type { get; set; }
        public List<VariantsOptions> options { get; set; }
    }

    public class VariantsOptions
    {
        public string tag { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }
}
