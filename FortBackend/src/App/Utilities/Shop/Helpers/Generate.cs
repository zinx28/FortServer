using FortBackend.src.App.Utilities.Constants;
using FortLibrary;
using FortLibrary.Shop;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            List<ShopBundles> bundleItems = new();
            var currentEvent = Generator.GetActiveEvent();
            if (!string.IsNullOrEmpty(currentEvent))
            {
                var FestiveShop = Saved.Saved.BackendCachedData.ShopFestiveItems
                       .FirstOrDefault(e => e.Key == currentEvent);

                if (FestiveShop.Key != null && FestiveShop.Value != null)
                    bundleItems = FestiveShop.Value.Bundles;
            }
            else 
                bundleItems = Saved.Saved.BackendCachedData.ShopBundlesFiltered; 

            if (bundleItems != null)
            {
                int randomIndex = random.Next(bundleItems.Count);
                ShopBundles RandomSkinItem = bundleItems[randomIndex];

                if (RandomSkinItem == null)
                {
                    Logger.Error($"Shop generation will be canceled -> RandomSkinItem is null", "ItemShop");
                    return false;
                }

                DateTime lastShownDate;
                if (DateTime.TryParse(RandomSkinItem.LastShownDate, out lastShownDate))
                {
                    if ((lastShownDate.Month == DateTime.UtcNow.Month && lastShownDate.Year == DateTime.UtcNow.Year) && !RandomSkinItem.AllowAgain)
                    {
                        Logger.Log("Item Already been this month", "ItemShop");
                        Generator.Attempts += 1;
                        return false;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

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
                            HowManyTurns += 1;

                        if (Item.singleprice != -1)
                            Price = Item.singleprice;
                        else
                        {
                            ItemTemplateId = Generator.GetCategoryFromItem(Item.item)!;

                            if (string.IsNullOrEmpty(ItemTemplateId))
                            {
                                Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                                return false;
                            }

                            if (Generator.categoryMap.ContainsKey(ItemTemplateId))
                                Price = Generator.categoryMap[ItemTemplateId](Item.rarity);
                        }

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
                            variants = Item.variants,
                            rarity = Item.rarity,
                            BundlePath = Item.BundlePath,
                            type = "Normal",
                            categories = Item.categories
                        });

                        Logger.Log($"Generated {ItemTemplateId}:{Item.name}", "ItemShop");
                        Generator.DailyItems -= 1;
                    }
                   
                }

                List<ShopBundlesItem> WeeklyArray = RandomSkinItem.Weekly;
                if (WeeklyArray != null)
                {
                    Logger.Log("Generating Weekly Bundles", "ItemShop");
                    HowManyTurns = 0;
                    int Price = -1;
                    string ItemTemplateId = "";


                    foreach (var Item in WeeklyArray)
                    {
                        if (!Item.categories.Any())
                            HowManyTurns += 1;

                        if (Item.singleprice != -1)
                            Price = Item.singleprice;
                        else
                        {
                            ItemTemplateId = Generator.GetCategoryFromItem(Item.item)!;

                            if (string.IsNullOrEmpty(ItemTemplateId))
                            {
                                Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                                return false;
                            }

                            if (Generator.categoryMap.ContainsKey(ItemTemplateId))
                                Price = Generator.categoryMap[ItemTemplateId](Item.rarity);
                        }

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

        public static async Task RandomItems(int Items, List<ItemsSaved> Type, List<object> DiscordFields, List<ShopItems> EventItems, double RandomFest = 0, string ItemType = "Small")
        {
            Logger.Log("Generating useless stuff -> " + Items, "ItemShop");
            Random RandomNumber = new Random();

            for (int i = 0; i < Items; i++)
            {
                if(EventItems != null && EventItems.Count > 0 && RandomNumber.NextDouble() < RandomFest)
                   await FestiveSingleItem(Generator.savedData, Type, Generator.itemTypes, EventItems, DiscordFields, ItemType);
                else
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
                    if (lastShownDate.Month == DateTime.UtcNow.Month && lastShownDate.Year == DateTime.UtcNow.Year)
                    {
                        await SingleItem(savedData, ListItemSaved, itemType, rarityProb, DiscordFields, type);
                        return;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

                if (Generator.categoryMap.ContainsKey(ChosenItemString))
                    Price = Generator.categoryMap[ChosenItemString](RandomSkinItem.rarity);

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
    

        public static async Task FestiveSingleItem(SavedData savedData, List<ItemsSaved> ListItemSaved, List<string> itemType, List<ShopItems> shopItems, List<object> DiscordFields, string type = "Small")
        {
            int Price = -1;
            Random rng = new Random();

            var available = shopItems
                .Where(item =>
                {
                    if (DateTime.TryParse(item.LastShownDate, out var d))
                        return d.Date != DateTime.UtcNow.Date;

                    return true;
                })
                .ToList();

            if (available.Count == 0)
            {
                await SingleItem(savedData, ListItemSaved, itemType, Generator.rarityProb1, DiscordFields, type);
                return;
            }

            var RandomSkinItem = available[rng.Next(available.Count)];
            RandomSkinItem.LastShownDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var ItemType = Generator.GetCategoryFromItem(RandomSkinItem.item);

            if (ItemType != null && Generator.categoryMap.ContainsKey(ItemType))
                Price = Generator.categoryMap[ItemType](RandomSkinItem.rarity);

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

            Logger.Log($"Generated Item: {ItemType}:{RandomSkinItem.name}", "ItemShop:Festive");
        }
    }
}
