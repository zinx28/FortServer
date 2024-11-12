using FortBackend.src.App.Utilities.ADMIN;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.Shop;
using System.Text.Json.Serialization;

namespace FortBackend.src.App.Utilities.Saved
{
    public class Saved
    {
        public static CachedAdminData CachedAdminData = new CachedAdminData();
        public static CachedDataClass BackendCachedData = new CachedDataClass();
        public static FortConfig DeserializeConfig = new FortConfig();
        public static FortGameConfig DeserializeGameConfig = new FortGameConfig();
    }

    public class CachedDataClass
    {
        // AUTO CHANGES DURING RUN TIME
        public string DefaultProtocol { get; set; } = "http://";
        public Dictionary<string, AthenaItem> FullLocker_AthenaItems { get; set; } = new Dictionary<string, AthenaItem>();
        public Dictionary<string, CommonCoreItem> DefaultBanners_Items { get; set; } = new Dictionary<string, CommonCoreItem>();

        public Timeline TimelineData { get; set; } = new Timeline();

        public ItemPricing ShopPrices { get; set; } = new ItemPricing();
        public List<ShopBundles> ShopBundles { get; set; } = new List<ShopBundles>();
        public List<ShopBundles> ShopBundlesFiltered { get; set; } = new List<ShopBundles>();

        
        public List<ShopItems> ShopSkinItems { get; set; } = new List<ShopItems>();
        public List<ShopItems> ShopPickaxesItems { get; set; } = new List<ShopItems>();
        public List<ShopItems> ShopEmotesItems { get; set; } = new List<ShopItems>();
        public List<ShopItems> ShopGlidersItems { get; set; } = new List<ShopItems>();
        public List<ShopItems> ShopWrapItems { get; set; } = new List<ShopItems>();
        
      
    }

    /*
     * 
     * Moved Config to the library.. works the same though- i just feel like it's better
     * FortLibrary/ConfigHelpers/FortConfig.cs
     */
}
