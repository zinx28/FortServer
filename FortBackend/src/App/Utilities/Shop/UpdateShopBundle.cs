using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.Shop;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Shop
{
    // couldve been better :3
    public class UpdateShopBundle
    {
        public static async Task UpdateShopData()
        {
            var ShopBundlesPath = PathConstants.ShopJson.ShopBundles;

            try
            {
                if (Saved.Saved.BackendCachedData.ShopBundlesFiltered.Any())
                {
                    if (Saved.Saved.BackendCachedData.ShopBundles.Count > 0)
                    {
                        if (File.Exists(ShopBundlesPath))
                        {
                            // changes the filtered stuff to shopbundles
                            foreach (var filteredBundle in Saved.Saved.BackendCachedData.ShopBundlesFiltered)
                            {
                              
                                var originalBundle = Saved.Saved.BackendCachedData.ShopBundles
                                .FirstOrDefault(bundle => bundle.BundleID == filteredBundle.BundleID && (filteredBundle.Weekly.Count > 1 || filteredBundle.Daily.Count > 1));

                                if (originalBundle != null && originalBundle.LastShownDate != filteredBundle.LastShownDate)
                                {
                                    originalBundle.LastShownDate = filteredBundle.LastShownDate;
                                    Logger.Log($"Bundle: {filteredBundle.BundleID} date been updated to {filteredBundle.LastShownDate}");
                                }
                                Logger.PlainLog(Saved.Saved.BackendCachedData.ShopBundles.Count);
                            }

                            // Then update the json file!!
                            string updatedJsonContent = JsonConvert.SerializeObject(Saved.Saved.BackendCachedData.ShopBundles, Formatting.Indented);

                            File.WriteAllText(ShopBundlesPath, updatedJsonContent);
                            Logger.Log("Writing to file", "ItemShop");
                        }
                        else
                        {
                            Logger.Error("ShopBundle file is null");
                        }
                    }
                    else
                    {
                        Logger.Error("ShopBundles is null");
                    }
                }
              
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}
