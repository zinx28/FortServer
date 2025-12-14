using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Shop.Helpers;
using FortLibrary;
using FortLibrary.Shop;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Shop
{
    public class LoadShopCache
    {
        public static void LoadAndFilterShopBundles(float currentSeason)
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

            Logger.Log($"ShopBundles Loaded: {Saved.Saved.BackendCachedData.ShopBundlesFiltered.Count()} / {Saved.Saved.BackendCachedData.ShopBundlesFiltered.Count()}");


            // Filter festive items, or smth like that

            var filteredFes = new Dictionary<string, FestiveShopItems>();

            foreach (var (eventName, festive) in Saved.Saved.BackendCachedData.ShopFestiveItems)
            {
                foreach (var bundle in festive.Bundles)
                {
                    bundle.Daily = bundle.Daily
                        .Where(i => i.season <= currentSeason)
                        .ToList();

                    bundle.Weekly = bundle.Weekly
                        .Where(i => i.season <= currentSeason)
                        .ToList();
                }

                var BundleCount = festive.Bundles.Count;
                var NormalCount = festive.Normal.Count;

                festive.Bundles = festive.Bundles
                     .Where(b => b.Daily.Any() || b.Weekly.Any())
                     .ToList();

                festive.Normal = festive.Normal
                    .Where(i => i.season <= currentSeason)
                    .ToList();

                if (festive.Bundles.Any() || festive.Normal.Any())
                    filteredFes[eventName] = festive;

                Logger.Log($"ShopFestive {eventName} Bundle Loaded: {festive.Bundles.Count()} / {BundleCount}");
                Logger.Log($"ShopFestive {eventName} Items Loaded: {festive.Normal.Count()} / {NormalCount}");
            }

            Saved.Saved.BackendCachedData.ShopFestiveItems = filteredFes;
        }

        // yaps
        public static List<ShopItems> LoadAndFilter(string filePath, float currentSeason, string ItemLoader = "skins")
        {
            if (!File.Exists(filePath))
            {
                Logger.Error($"File not found: {filePath}");
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

        public static void LoadItems(float currentSeason)
        {
            Saved.Saved.BackendCachedData.ShopSkinItems = LoadAndFilter(PathConstants.ShopJson.ShopSkins, currentSeason);
            Saved.Saved.BackendCachedData.ShopEmotesItems = LoadAndFilter(PathConstants.ShopJson.ShopEmotes, currentSeason, "emotes");
            Saved.Saved.BackendCachedData.ShopGlidersItems = LoadAndFilter(PathConstants.ShopJson.ShopGliders, currentSeason, "gliders");
            Saved.Saved.BackendCachedData.ShopPickaxesItems = LoadAndFilter(PathConstants.ShopJson.ShopPickaxe, currentSeason, "pickaxes");
            Saved.Saved.BackendCachedData.ShopWrapItems = LoadAndFilter(PathConstants.ShopJson.ShopWrap, currentSeason, "wrap");
        }


    }
}
