using FortBackend.src.App.Utilities.Shop.Helpers.Data;

namespace FortBackend.src.App.Utilities.Shop.Helpers.Class
{
    public class ShopJson
    {
        public string expiration { get; set; } = string.Empty;
        public string cacheExpire {  get; set; } = string.Empty;
        public ShopJsonItem ShopItems { get; set; } = new ShopJsonItem();
    }

    public class ShopJsonItem
    {
        public List<ItemsSaved> Weekly { get; set; } = new List<ItemsSaved>();
        public List<ItemsSaved> Daily { get; set; } = new List<ItemsSaved>();
    }
}
