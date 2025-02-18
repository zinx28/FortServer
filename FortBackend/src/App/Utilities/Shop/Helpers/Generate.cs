using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.Shop;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Shop.Helpers
{
    public class Generate
    {
        public static int HowManyTurns = 0;
        public static async Task<bool> Bundles()
        {

            if (Generator.Attempts == 4)
            {
                Logger.Error("Went pass too many attempts", "ItemShop");
                return false;
            }
            Random random = new Random();

            List<ShopBundles> skinItems = Saved.Saved.BackendCachedData.ShopBundlesFiltered;
            if (skinItems != null)
            {
                int randomIndex = random.Next(skinItems.Count);
                Console.WriteLine(randomIndex);
                ShopBundles RandomSkinItem = skinItems[randomIndex];
                Console.WriteLine(RandomSkinItem);
                if (RandomSkinItem == null)
                {
                    Logger.Error($"Shop generation will be canceled -> RandomSkinItem is null", "ItemShop");
                    return false;
                }

                DateTime lastShownDate;
                if (DateTime.TryParse(RandomSkinItem.LastShownDate, out lastShownDate))
                {
                    if (lastShownDate.Month == DateTime.Now.Month && lastShownDate.Year == DateTime.Now.Year)
                    {
                        Logger.Log("Item Already been this month", "ItemShop");
                        Generator.Attempts += 1;
                        return false;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.Now.ToString();

                await UpdateShopBundle.UpdateShopData();

                List<ShopBundlesItem> DailyArray = RandomSkinItem.Daily;
                if (DailyArray != null)
                {
                    Logger.Log("Generating Daily Bundles", "ItemShop");
                    HowManyTurns = 0;
                    int Price = -1; // High As Broken
                    string ItemTemplateId = "";

                    foreach (var Item in DailyArray)
                    {
                        
                        if (!Item.categories.Any())
                        {
                            HowManyTurns += 1;
                        }
                        if (Item.singleprice != -1)
                        {
                            Price = Item.singleprice;
                        }
                        else
                        { 
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

                            int price = 0;
                            if (Generator.categoryMap.ContainsKey(ItemTemplateId))
                            {
                                Console.WriteLine("TEST!!");
                                price = Generator.categoryMap[ItemTemplateId](Item.rarity);

                                if (price != 0)
                                {
                                    Price = price;
                                }
                            }

                            //if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            //{
                            //    Price = price;
                            //}
                        }

                        //if (Item.singleprice == -1)
                        //{
                        //    if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                        //    {
                        //        Price = price;
                        //    }
                        //}
                        //else
                        //{
                        //    if (Item.newprice != -1)
                        //    {
                        //        Price = Item.newprice;
                        //    }
                        //    else
                        //    {
                        //        Price = Item.singleprice;
                        //    }
                        //}

                        Generator.savedData.DailyFields.Add(new
                        {
                            name = Item.name,
                            value = Price
                        });

                        Generator.savedData.Daily.Add(new ItemsSaved
                        {
                            id = Item.id,
                            item = Item.item,
                            name = Item.name,
                            description = Item.description,
                            items = Item.items,
                            price = Price,
                            singleprice = Price, // not done
                            rarity = Item.rarity,
                            BundlePath = Item.BundlePath,
                            type = "Normal",
                            categories = Item.categories
                        });
                        Logger.Log($"Generated {ItemTemplateId}:{Item.name}", "ItemShop");
                    }
                    Generator.DailyItems -= 1;
                }

                List<ShopBundlesItem> WeeklyArray = RandomSkinItem.Weekly;
                if (WeeklyArray != null)
                {
                    Logger.Log("Generating Weekly Bundles", "ItemShop");
                    HowManyTurns = 0;
                    int Price = -1; // High As Broken
                    string ItemTemplateId = "";

                  
                    foreach (var Item in WeeklyArray)
                    {
                       
                        if (!Item.categories.Any())
                        {
                            HowManyTurns += 1;
                        }

                        if(Item.singleprice != -1) {
                            Price = Item.singleprice;
                        }
                        else
                        {
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

                            int price = 0;
                            if (Generator.categoryMap.ContainsKey(ItemTemplateId))
                            {
                                price = Generator.categoryMap[ItemTemplateId](Item.rarity);

                                if (price != 0)
                                {
                                    Price = price;
                                }
                            }

                            //if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            //{
                            //    Price = price;
                            //}
                        }



                        //if (Item.singleprice == -1)
                        //{
                        //    if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                        //    {
                        //        Price = price;
                        //    }
                        //}
                        //else
                        //{
                        //    if (Item.newprice != -1)
                        //    {
                        //        Price = Item.newprice;
                        //    }
                        //    else
                        //    {
                        //        Price = Item.singleprice;
                        //    }
                        //}

                        Generator.savedData.WeeklyFields.Add(new
                        {
                            name = Item.name,
                            value = Price
                        });

                        Generator.savedData.Weekly.Add(new ItemsSaved
                        {
                            id = Item.id,
                            item = Item.item,
                            name = Item.name,
                            description = Item.description,
                            items = Item.items,
                            price = Price,
                            singleprice = Price, // not done
                            rarity = Item.rarity,
                            BundlePath = Item.BundlePath,
                            type = "Normal",
                            categories = Item.categories
                        });
                        Logger.Log($"Generated {ItemTemplateId}:{Item.name}", "ItemShop");
                    }
                    Generator.WeeklyItems -= 1;
                }
            }
            else
            {
                Logger.Error("Shop items is null");
            }


            return true;
        }

        public static async Task RandomItems(int Items, List<ItemsSaved> Type, List<object> DiscordFields, string ItemType = "Small")
        {
            Logger.Log("Generating useless stuff -> " + Items, "ItemShop");
            for (int i = 0; i < Items; i++)
            {
                await SingleItem(Generator.savedData, Type, Generator.itemTypes, Generator.rarityProb1, DiscordFields, ItemType);
            }
        }

        public static async Task SingleItem(SavedData savedData, List<ItemsSaved> ListItemSaved, List<string> itemType, double[] rarityProb, List<object> DiscordFields, string type = "Small")
        {
            Random random = new Random();
            List<ShopItems> ChosenItem = new List<ShopItems>();
            var ChosenItemString = "";
            int Price = -1;
            double y = 0.0;
            double randomValue = random.NextDouble();

            for (int i = 0; i < rarityProb.Length; i++)
            {
                y += rarityProb[i];
                if (randomValue < y)
                {
                    ChosenItemString = itemType[i];
                    if (Generator.itemTypeMap.ContainsKey(ChosenItemString))
                    {
                        ChosenItem = Generator.itemTypeMap[ChosenItemString]();

                        if(ChosenItem.Count != 0)
                        {
                            break;
                        }
                    }
                    //ChosenItem = (itemType[i]);
                    break;
                }
            }

            if (ChosenItem != null && ChosenItem.Count > 0)
            {
                int randomIndex = random.Next(ChosenItem.Count);
                ShopItems RandomSkinItem = ChosenItem[randomIndex];
                Random random1 = new Random();

                DateTime lastShownDate;
                if (DateTime.TryParse(RandomSkinItem.LastShownDate, out lastShownDate))
                {
                    if (lastShownDate.Month == DateTime.Now.Month && lastShownDate.Year == DateTime.Now.Year)
                    {
                        await SingleItem(savedData, ListItemSaved, itemType, rarityProb, DiscordFields, type);
                        return;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.Now.ToString("yyyy-MM-dd");

                // enable if you want doesnt really matter (epic has the same items in the shop 24/7 now)
                
                //string filePath = Path.Combine(PathConstants.BaseDir, $"json/shop/{ChosenItemString}.json");
                //if (!File.Exists(filePath))
                //{
                //    Logger.Error("Chosen Item: " + ChosenItemString + " Path isnt found :(");
                //    return;
                //}
                //string updatedJsonContent = JsonConvert.SerializeObject(ChosenItem, Formatting.Indented);
                //File.WriteAllText(filePath, updatedJsonContent);

                //

                int price = 0;
                Console.WriteLine(ChosenItemString);
                Console.WriteLine(Generator.categoryMap.ContainsKey(ChosenItemString));
                if (Generator.categoryMap.ContainsKey(ChosenItemString))
                {
                    price = Generator.categoryMap[ChosenItemString](RandomSkinItem.rarity);
                    Console.WriteLine(price);
                    if (price != 0)
                    {
                        Price = price;
                    }
                }

                //if (Generator.PriceValues.TryGetValue(ChosenItem, out var categoryPrices) && categoryPrices.TryGetValue(RandomSkinItem.rarity, out var price))
                //{
                //    Price = price;
                //}

                DiscordFields.Add(new
                {
                    name = RandomSkinItem.name,
                    value = Price
                });

                ListItemSaved.Add(new ItemsSaved
                {
                    id = RandomSkinItem.id,
                    item = RandomSkinItem.item,
                    name = RandomSkinItem.name,
                    description = RandomSkinItem.description,
                    items = RandomSkinItem.items,
                    price = Price,
                    singleprice = Price,
                    variants = RandomSkinItem.variants,
                    BundlePath = RandomSkinItem.BundlePath,
                    type = type,
                    rarity = RandomSkinItem.rarity
                });

                Logger.Log($"Generated Item: {ChosenItem}:{RandomSkinItem.name}", "ItemShop");
            }
            else
            {
                Logger.Error($"Failed To Generate Item: {ChosenItemString}:Unknown ofc", "ItemShop");
            }
        }
    }
}
