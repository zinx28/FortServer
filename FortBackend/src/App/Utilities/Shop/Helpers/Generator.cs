using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FortBackend.src.App.Utilities.Shop.Helpers
{
    public class Generator
    {
        public static int WeeklyItems;
        public static int DailyItems;
        public static int Attempts = 0;
        public static int HowManyTurns = 0;
        public static SavedData savedData = new SavedData();

        public static string PriceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src/Resources/json/shop/prices.json");
        public static string PricejsonContent = File.ReadAllText(PriceFile);
        public static Dictionary<string, Dictionary<string, int>> PriceValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(PricejsonContent);

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

        public static async Task<bool> GenerateBundles()
        {
            
            if (Attempts == 4)
            {
                Logger.Error("Went pass to many attempts", "ItemShop");
                return false;
            }

            Random random = new Random();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/shop/bundles.json");
            string jsonContent = File.ReadAllText(filePath);
            if(jsonContent == null)
            {
                Logger.Error("Shop generation will be canceled -> bundles is null");
                return false;
            }
           

            List<ShopBundles> skinItems = JsonConvert.DeserializeObject<List<ShopBundles>>(jsonContent);
            if(skinItems != null)
            {
                int randomIndex = random.Next(skinItems.Count);
                ShopBundles RandomSkinItem = skinItems[randomIndex];

                if(RandomSkinItem == null)
                {
                    Logger.Error($"Shop generation will be canceled -> RandomSkinItem is null");
                    return false;
                }

                DateTime lastShownDate;
                if (DateTime.TryParse(RandomSkinItem.LastShownDate, out lastShownDate))
                {
                    if (lastShownDate.Month == DateTime.Now.Month && lastShownDate.Year == DateTime.Now.Year)
                    {
                        Logger.Log("Item Already been this month", "ItemShop");
                        Attempts += 1;
                        return false;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.Now.ToString();

                string updatedJsonContent = JsonConvert.SerializeObject(skinItems, Formatting.Indented);
                File.WriteAllText(filePath, updatedJsonContent);


                List<ShopBundlesItem> DailyArray = RandomSkinItem.Daily;
                if (DailyArray != null)
                {
                    Logger.Log("Generating Daily Bundles", "ItemShop");
                    HowManyTurns = 0;
                    HowManyTurns = 0;
                    int Price = 93439340; // High As Broken
                    string ItemTemplateId = "";

                    foreach (var Item in DailyArray)
                    {
                        if (!Item.categories.Any())
                        {
                            HowManyTurns += 1;
                        }
                        switch (Item.item.Split(":")[0])
                        {
                            case "AthenaCharacter":
                                ItemTemplateId = "skins";
                            break;
                            case "AthenaDance":
                                ItemTemplateId = "emotes";
                            break;
                            case "AthenaPickaxe":
                                ItemTemplateId = "pickaxes";
                                break;
                            case "AthenaGlider":
                                ItemTemplateId = "gliders";
                                break;
                            case "AthenaItemWrap":
                                ItemTemplateId = "wrap";
                                break;
                            default:
                                break;
                        }

                        if (string.IsNullOrEmpty(ItemTemplateId))
                        {
                            Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                            return false;
                        }

                        if (Item.singleprice == -1)
                        {
                            if (PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            {
                                Price = price;
                            }
                        }
                        else
                        {
                            if (Item.newprice != -1)
                            {
                                Price = Item.newprice;
                            }
                            else
                            {
                                Price = Item.singleprice;
                            }
                        }

                        savedData.Daily.Add(new ItemsSaved
                        {
                            id = Guid.NewGuid().ToString().Replace("-", ""),
                            item = Item.item,
                            name = Item.name,
                            items = Item.items,
                            price = Price,
                            normalprice = Price, // not done
                            rarity = Item.rarity,
                            type = "Normal",
                            categories = Item.categories
                        });
                    }
                    DailyItems -= HowManyTurns;
                }

                List<ShopBundlesItem> WeeklyArray = RandomSkinItem.Weekly;
                if (WeeklyArray != null)
                {
                    Logger.Log("Generating Weekly Bundles", "ItemShop");
                    HowManyTurns = 0;
                    int Price = 93439340; // High As Broken
                    string ItemTemplateId = "";

                    

                    foreach (var Item in WeeklyArray)
                    {
                        if (!Item.categories.Any())
                        {
                            HowManyTurns += 1;
                        }

                        switch (Item.item.Split(":")[0])
                        {
                            case "AthenaCharacter":
                                ItemTemplateId = "skins";
                                break;
                            case "AthenaDance":
                                ItemTemplateId = "emotes";
                                break;
                            case "AthenaPickaxe":
                                ItemTemplateId = "pickaxes";
                                break;
                            case "AthenaGlider":
                                ItemTemplateId = "gliders";
                                break;
                            case "AthenaItemWrap":
                                ItemTemplateId = "wrap";
                                break;
                            default:
                                break;
                        }

                        if (string.IsNullOrEmpty(ItemTemplateId))
                        {
                            Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                            return false;
                        }

                        if(Item.singleprice == -1)
                        {
                            if (PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            {
                                Price = price;
                            }
                        }else
                        {
                            if (Item.newprice != -1)
                            {
                                Price = Item.newprice;
                            }
                            else
                            {
                                Price = Item.singleprice;
                            }
                        }

                        savedData.Weekly.Add(new ItemsSaved
                        {
                            id = Guid.NewGuid().ToString().Replace("-", ""),
                            item = Item.item,
                            name = Item.name,
                            items = Item.items,
                            price = Price,
                            normalprice = Price, // not done
                            rarity = Item.rarity,
                            type = "large",
                            categories = Item.categories
                        });
                    }
                    WeeklyItems -= HowManyTurns;
                }
            }
            else
            {
                Logger.Error("Shop items is null");
            }


            return true;
        }

        public static async Task GenerateDailyItems(int Items, List<ItemsSaved> Type, string ItemType = "Small")
        {
            Logger.Log("Generating useless stuff -> " + Items, "ItemShop");
            for (int i = 0; i < Items; i++)
            {
                await GenerateSingleItem(savedData, Type, itemTypes1, rarityProb1, ItemType);
            }
        }

        public static async Task GenerateSingleItem(SavedData savedData, List<ItemsSaved> ListItemSaved, string[] itemType, double[] rarityProb, string type = "Small")
        {
            Random random = new Random();
            string ChosenItem = string.Empty;
            int Price = -1;
            double y = 0.0;
            double randomValue = random.NextDouble();

            for (int i = 0; i < rarityProb.Length; i++)
            {
                y += rarityProb[i];
                if (randomValue < y)
                {
                    ChosenItem = itemType[i];
                    break;
                }
            }

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src/Resources/json/shop/{ChosenItem}.json");
            //Console.WriteLine(filePath);
            string jsonContent = File.ReadAllText(filePath);
     
            List<ShopItems> skinItems = JsonConvert.DeserializeObject<List<ShopItems>>(jsonContent);


            ////Console.WriteLine(jsonContent);

            if (skinItems != null)
            {
                int randomIndex = random.Next(skinItems.Count);
                ShopItems RandomSkinItem = skinItems[randomIndex];
                Random random1 = new Random();

                DateTime lastShownDate;
                if (DateTime.TryParse(RandomSkinItem.LastShownDate, out lastShownDate))
                {
                    if (lastShownDate.Month == DateTime.Now.Month && lastShownDate.Year == DateTime.Now.Year)
                    {
                        await GenerateSingleItem(savedData, ListItemSaved, itemType, rarityProb);
                        return;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.Now.ToString("yyyy-MM-dd");
                string updatedJsonContent = JsonConvert.SerializeObject(skinItems, Formatting.Indented);
                File.WriteAllText(filePath, updatedJsonContent);

                if (PriceValues.TryGetValue(ChosenItem, out var categoryPrices) && categoryPrices.TryGetValue(RandomSkinItem.rarity, out var price))
                {
                    Price = price;
                }

                ListItemSaved.Add(new ItemsSaved
                {
                    id = Guid.NewGuid().ToString().Replace("-", ""),
                    item = RandomSkinItem.item,
                    name = RandomSkinItem.name,
                    items = RandomSkinItem.items,
                    price = Price,
                    normalprice = Price,
                    variants = RandomSkinItem.variants,
                    type = type,
                    rarity = RandomSkinItem.rarity
                });

                Logger.Log($"Generated Item: {ChosenItem}:{RandomSkinItem.name}", "ItemShop");
            }
            else
            {
                Logger.Error($"Failed To Generate Item: {ChosenItem}:Unknown ofc", "ItemShop");

            }

        }

        public static async Task<SavedData> Start(SavedData saveddata)
        {
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
                bool waitforme = await GenerateBundles();
                if (waitforme)
                {
                    Attempts = 4;
                }
                else
                {
                    Logger.Error("Attempting again", "ItemShop");
                }
            }
            Console.WriteLine(DailyItems);
            Logger.Log("Generating left over items", "ItemShop");
            //ItemType
            await GenerateDailyItems(DailyItems, savedData.Daily, "Normal");
            await GenerateDailyItems(4, savedData.Daily);
            await GenerateDailyItems(DailyItems, savedData.Weekly, "Normal");
            await GenerateDailyItems(4, savedData.Weekly);

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
            return savedData;
        }
    }
}
