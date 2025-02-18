using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Shop;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FortBackend.src.App.Utilities.Shop.Helpers
{
    public class Generator
    {
        public static int WeeklyItems;
        public static int DailyItems;
        public static int Attempts = 0;
        public static SavedData savedData = new SavedData();
        public static ItemPricing PriceValues = new ItemPricing();

        public static Dictionary<string, Func<string, int>> categoryMap = new Dictionary<string, Func<string, int>>()
        {
            { "skins", (rarity) => GetPrice(rarity, PriceValues.skins) },
            { "emotes", (rarity) => GetPrice(rarity, PriceValues.emotes) },
            { "pickaxes", (rarity) => GetPrice(rarity, PriceValues.pickaxes) },
            { "gliders", (rarity) => GetPrice(rarity, PriceValues.gliders) },
            { "wrap", (rarity) => GetPrice(rarity, PriceValues.wrap) }
        };

        public static int GetPrice(string rarity, object category)
        {
            var prop = category.GetType().GetProperty(rarity);
            if (prop != null)
            {
                if(int.TryParse(prop.GetValue(category)!.ToString(), out int price)) {
                    return price;
                }
            }
            else
            {
                Logger.Error($"Rarity {rarity} not found.");
           
            }
            return -1;
        }

        public static List<string> itemTypes = new List<string>(); // auto added during startup

        public static Dictionary<string, Func<List<ShopItems>>> itemTypeMap = new Dictionary<string, Func<List<ShopItems>>>()
        {
            { "skins", () => Saved.Saved.BackendCachedData.ShopSkinItems },
            { "emotes", () => Saved.Saved.BackendCachedData.ShopEmotesItems },
            { "pickaxes", () => Saved.Saved.BackendCachedData.ShopPickaxesItems },
            { "gliders", () => Saved.Saved.BackendCachedData.ShopGlidersItems },
            { "wrap", () => Saved.Saved.BackendCachedData.ShopWrapItems }
        };

        public static double[] rarityProb1 = {
            0.30,
            0.30,
            0.30,
            0.30,
            0.15
        };

        public static async Task<SavedData> Start(SavedData saveddata)
        {
            if (Saved.Saved.BackendCachedData.ShopPrices != null)
            {
                PriceValues = Saved.Saved.BackendCachedData.ShopPrices!;
                savedData = saveddata;

                WeeklyItems = savedData.WeeklyItems;
                DailyItems = savedData.DailyItems;

                Random RandomNumber = new Random();

                //WeeklyItems = RandomNumber.Next(1, 2);
                //DailyItems = RandomNumber.Next(1, 2);
                //// generate 2 rows - daily and weekly row!
                //if (WeeklyItems > 3) { WeeklyItems = 3; }
                //if (DailyItems > 3) { DailyItems = 3; }

                //WeeklyItems = RandomNumber.Next(2, 3);
                //DailyItems = RandomNumber.Next(5, 7);
                //if (DailyItems > 7) { DailyItems = 7; };
                //if (WeeklyItems > 4) { WeeklyItems = 4; };

                Logger.Log($"Expecting ~ WeeklyItems [{WeeklyItems}] - DailyItems [{DailyItems}]", "ItemShop");

                while (Attempts != 4)
                {
                    bool waitforme = await Generate.Bundles();
                    if (waitforme)
                    {
                        Attempts = 4;
                    }
                    else
                    {
                        Logger.Error("Attempting again", "ItemShop");
                    }
                }
                Logger.Log("Generating left over items", "ItemShop");



                await Generate.RandomItems(DailyItems, savedData.Daily, saveddata.DailyFields, "Normal");
                await Generate.RandomItems(WeeklyItems, savedData.Weekly, saveddata.WeeklyFields, "Normal");

                if (savedData.Season >= 14)
                {
                    await Generate.RandomItems(4, savedData.Daily, saveddata.DailyFields);
                    await Generate.RandomItems(4, savedData.Weekly, saveddata.WeeklyFields);

                }
              
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                var shopGen = new ShopJson
                {
                    expiration = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    cacheExpire = new DateTime(date.Year, date.Month, date.Day, 16, 57, 14, 490).AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ShopItems = new ShopJsonItem
                    {
                        Weekly = savedData.Weekly,
                        Daily = savedData.Daily,
                    }
                };

                Saved.Saved.BackendCachedData.CurrentShop = shopGen;
                string updatedJsonContent = JsonConvert.SerializeObject(shopGen, Formatting.Indented);
                string filePath = PathConstants.ShopJson.Shop;
                if(File.Exists(filePath))
                {
                    File.WriteAllText(filePath, updatedJsonContent);
                }
                else
                {
                    throw new Exception("FAILED TO FIND SHOP PATH");
                }
             
            }
            return savedData;
        }
    }
}
