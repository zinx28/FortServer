using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
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
                Logger.Error("Went pass to many attempts", "ItemShop");
                return false;
            }

            Random random = new Random();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "json", "shop", "bundles.json");
            string jsonContent = File.ReadAllText(filePath);
            if (jsonContent == null)
            {
                Logger.Error("Shop generation will be canceled -> bundles is null");
                return false;
            }


            List<ShopBundles> skinItems = JsonConvert.DeserializeObject<List<ShopBundles>>(jsonContent);
            if (skinItems != null)
            {
                int randomIndex = random.Next(skinItems.Count);
                ShopBundles RandomSkinItem = skinItems[randomIndex];

                if (RandomSkinItem == null)
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
                        Generator.Attempts += 1;
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
                            //foreach (Item item in Item.items)
                            //{
                            //    string itemType = item.item.Split(":")[0];

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

                            if (!string.IsNullOrEmpty(ItemTemplateId))
                            {
                                break;
                            }
                            //}

                            if (string.IsNullOrEmpty(ItemTemplateId))
                            {
                                Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                                return false;
                            }

                            if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            {
                                Price = price;
                            }
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
                    }
                    Generator.DailyItems -= HowManyTurns;
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
                            //foreach (Item item in Item.items)
                            //{
                            //    string itemType = item.item.Split(":")[0];

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

                                if(!string.IsNullOrEmpty(ItemTemplateId))
                                {
                                    break;
                                }
                            //}

                            if (string.IsNullOrEmpty(ItemTemplateId))
                            {
                                Logger.Error("Shop generation will be canceled -> I cannot idenity the item");
                                return false;
                            }

                            if (Generator.PriceValues.TryGetValue(ItemTemplateId, out var categoryPrices) && categoryPrices.TryGetValue(Item.rarity, out var price))
                            {
                                Price = price;
                            }
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
                    }
                    Generator.WeeklyItems -= HowManyTurns;
                }
            }
            else
            {
                Logger.Error("Shop items is null");
            }


            return true;
        }

        public static async Task RandomItems(int Items, List<ItemsSaved> Type, string ItemType = "Small")
        {
            Logger.Log("Generating useless stuff -> " + Items, "ItemShop");
            for (int i = 0; i < Items; i++)
            {
                await SingleItem(Generator.savedData, Type, Generator.itemTypes1, Generator.rarityProb1, ItemType);
            }
        }

        public static async Task SingleItem(SavedData savedData, List<ItemsSaved> ListItemSaved, string[] itemType, double[] rarityProb, string type = "Small")
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
            string jsonContent = File.ReadAllText(filePath);

            List<ShopItems> skinItems = JsonConvert.DeserializeObject<List<ShopItems>>(jsonContent);

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
                        await SingleItem(savedData, ListItemSaved, itemType, rarityProb);
                        return;
                    }
                }

                RandomSkinItem.LastShownDate = DateTime.Now.ToString("yyyy-MM-dd");
                string updatedJsonContent = JsonConvert.SerializeObject(skinItems, Formatting.Indented);
                File.WriteAllText(filePath, updatedJsonContent);

                if (Generator.PriceValues.TryGetValue(ChosenItem, out var categoryPrices) && categoryPrices.TryGetValue(RandomSkinItem.rarity, out var price))
                {
                    Price = price;
                }

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
                Logger.Error($"Failed To Generate Item: {ChosenItem}:Unknown ofc", "ItemShop");
            }
        }
    }
}
