using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Shop.Helpers;
using FortLibrary;
using FortLibrary.Shop;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Shop
{
    public class LoadShopCache
    {
        public static void LoadAndFilterShopBundles(int currentSeason)
        {
            List<ShopBundles> bundlesToRemove = new List<ShopBundles>();

            foreach (var bundle in Saved.Saved.BackendCachedData.ShopBundlesFiltered)
            {
                bundle.Daily = bundle.Daily
                    .Where(item => item.season <= currentSeason)
                    .ToList();

                bundle.Weekly = bundle.Weekly
                    .Where(item => item.season <= currentSeason)
                    .ToList();

                if (!bundle.Daily.Any() && !bundle.Weekly.Any())
                {
                    bundlesToRemove.Add(bundle);
                }
            }

            foreach (var bundle in bundlesToRemove)
            {
                Saved.Saved.BackendCachedData.ShopBundlesFiltered.Remove(bundle);
            }

            Console.WriteLine($"ShopBundles Loaded: {Saved.Saved.BackendCachedData.ShopBundlesFiltered.Count()} / {Saved.Saved.BackendCachedData.ShopBundles.Count()}");
        }

        // yaps
        public static List<ShopItems> LoadAndFilter(string filePath, int currentSeason, string ItemLoader = "skins")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return new List<ShopItems>();
            }

            var jsonContent = File.ReadAllText(filePath);
            var items = JsonConvert.DeserializeObject<List<ShopItems>>(jsonContent) ?? new List<ShopItems>();
            var filteredSkins = items.Where(item => item.season <= currentSeason).ToList();
         
            if(filteredSkins.Count > 0)
                Generator.itemTypes.Add(ItemLoader);

            Logger.Log($"Loaded {ItemLoader} ~ {filteredSkins.Count}/{items.Count}", "ITEMSHOP");
            return filteredSkins;
        }

        public static void LoadItems(int currentSeason)
        {
            Saved.Saved.BackendCachedData.ShopSkinItems = LoadAndFilter(PathConstants.ShopJson.ShopSkins, currentSeason);
            Saved.Saved.BackendCachedData.ShopEmotesItems = LoadAndFilter(PathConstants.ShopJson.ShopEmotes, currentSeason, "emotes");
            Saved.Saved.BackendCachedData.ShopGlidersItems = LoadAndFilter(PathConstants.ShopJson.ShopGliders, currentSeason, "gliders");
            Saved.Saved.BackendCachedData.ShopPickaxesItems = LoadAndFilter(PathConstants.ShopJson.ShopPickaxe, currentSeason, "pickaxes");
            Saved.Saved.BackendCachedData.ShopWrapItems = LoadAndFilter(PathConstants.ShopJson.ShopWrap, currentSeason, "wrap");
        }


    }
}
