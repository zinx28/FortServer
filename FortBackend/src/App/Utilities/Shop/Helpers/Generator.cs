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

        public static string PriceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src/Resources/json/shop/prices.json");
        public static string PricejsonContent = File.ReadAllText(PriceFile);

        public static Dictionary<string, Dictionary<string, int>> PriceValues = new Dictionary<string, Dictionary<string, int>>();

        public static string[] itemTypes1 = {
            "skins",
            "emotes",
            "gliders",
            "pickaxes",
            "wrap"
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
            if (PricejsonContent != null)
            {
                PriceValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(PricejsonContent)!;
                savedData = saveddata;
                Random RandomNumber = new Random();
                WeeklyItems = RandomNumber.Next(1, 2);
                DailyItems = RandomNumber.Next(1, 2);
                // generate 2 rows - daily and weekly row!
                if (WeeklyItems > 3) { WeeklyItems = 3; }
                if (DailyItems > 3) { DailyItems = 3; }

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

                await Generate.RandomItems(DailyItems, savedData.Daily, "Normal");
                await Generate.RandomItems(4, savedData.Daily);
                await Generate.RandomItems(DailyItems, savedData.Weekly, "Normal");
                await Generate.RandomItems(4, savedData.Weekly);

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

                Console.WriteLine(shopGen);
                string updatedJsonContent = JsonConvert.SerializeObject(shopGen, Formatting.Indented);
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/shop.json");
                File.WriteAllText(filePath, updatedJsonContent);
            }
            return savedData;
        }
    }
}
