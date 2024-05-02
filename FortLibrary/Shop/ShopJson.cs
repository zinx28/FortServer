namespace FortLibrary.Shop
{
    public class ShopJson
    {
        public string expiration { get; set; } = "9999-12-31T23:59:59.9999999";
        public string cacheExpire { get; set; } = "9999-12-31T23:59:59.9999999";
        public ShopJsonItem ShopItems { get; set; } = new ShopJsonItem();
    }

    public class ShopJsonItem
    {
        public List<ItemsSaved> Weekly { get; set; } = new List<ItemsSaved>();
        public List<ItemsSaved> Daily { get; set; } = new List<ItemsSaved>();
    }
}
