using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.Shop;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseItem
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "json", "shop", "shop.json");
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
            {
                throw new BaseError()
                {
                    errorCode = "errors.com.epicgames.modules.catalog",
                    errorMessage = "Server Sided Issue",
                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                    numericErrorCode = 12801,
                    originatingService = "any",
                    intent = "prod",
                    error_description = "Server Sided Issue",
                };
            }
            ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(json);

            if (shopData != null)
            {
                string[] SplitOfferId = Body.offerId.Split(":/");
                string SecondSplitOfferId = SplitOfferId[1];
                ItemsSaved ShopContent = new ItemsSaved();
                var NotificationsItems = new List<NotificationsItemsClass>();
                var MultiUpdates = new List<object>();
                var ApplyProfileChanges = new List<object>();
                List<Dictionary<string, object>> itemList = new List<Dictionary<string, object>>();
                int BaseRev = profileCacheEntry.AccountData.commoncore.RVN;
                int BaseRev2 = profileCacheEntry.AccountData.athena.RVN;

                foreach (ItemsSaved storefront in shopData.ShopItems.Daily)
                {
                    if (storefront.id == SecondSplitOfferId)
                    {
                        ShopContent = storefront;
                    }
                }

                foreach (ItemsSaved storefront in shopData.ShopItems.Weekly)
                {
                    if (storefront.id == SecondSplitOfferId)
                    {
                        ShopContent = storefront;
                    }
                }

                if (!string.IsNullOrEmpty(ShopContent.id))
                {
                    bool HasUserHaveItem = profileCacheEntry.AccountData.athena.Items.Any(item => item.Key.Contains(ShopContent.id));

                    if (HasUserHaveItem)
                    {
                        throw new BaseError()
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "You already own the item",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "You already own the item",
                        };
                    }

                    Console.WriteLine(HasUserHaveItem);

                    NotificationsItems.Add(new NotificationsItemsClass
                    {
                        itemType = ShopContent.item,
                        itemGuid = ShopContent.item,
                        itemProfile = "athena"
                    });

                    MultiUpdates.Add(new MultiUpdateClass
                    {
                        changeType = "itemAdded",
                        itemId = ShopContent.item,
                        item = new AthenaItem
                        {
                            templateId = ShopContent.item,
                            attributes = new AthenaItemAttributes
                            {
                                item_seen = false,
                                variants = ShopContent.variants,
                            },
                            quantity = 1
                        }
                    });

                    foreach (Item ItemShopItem in ShopContent.items)
                    {
                        NotificationsItems.Add(new NotificationsItemsClass
                        {
                            itemType = ItemShopItem.item,
                            itemGuid = ItemShopItem.item,
                            itemProfile = "athena"
                        });

                        MultiUpdates.Add(new MultiUpdateClass
                        {
                            changeType = "itemAdded",
                            itemId = ItemShopItem.item,
                            item = new AthenaItem
                            {
                                templateId = ItemShopItem.item,
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = false,
                                    variants = ItemShopItem.variants,
                                },
                                quantity = 1
                            }
                        });
                    }
                    //AthenaItem test = AccountDataParsed.commoncore.Items.FirstOrDefault(e => e.ContainsKey("Currency"))["Currency"] as AthenaItem;

                    // I need to work on this
                    var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"] as dynamic;

                    try
                    {
                        // AthenaItem currencyItem2 = currencyItem as AthenaItem;
                        Console.WriteLine(currencyItem);
                        // Console.WriteLine(currencyItem2.quantity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //Console.WriteLine(AccountDataParsed.commoncore.Items[GrabPlacement]["Currency"]);
                    if (currencyItem.quantity == 0)
                    {
                        throw new BaseError()
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "Your Poor",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Your Poor",
                        };
                    }

                    Console.WriteLine($"Currency Item Quantity: {currencyItem.quantity}");

                    if (ShopContent.price > int.Parse(currencyItem.quantity.ToString()))
                    {
                        throw new BaseError()
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "Item is higher price",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Item is higher price",
                        };
                    }

                    int Price = currencyItem.quantity - ShopContent.price;

                    ApplyProfileChanges.Add(new ApplyProfileChangesClass
                    {
                        changeType = "itemQuantityChanged",
                        itemId = "Currency",
                        quantity = Price
                    });


                    profileCacheEntry.AccountData.athena.Items.Add($"{ShopContent.item}", new AthenaItem
                    {
                        templateId = $"{ShopContent.item}",
                        attributes = new AthenaItemAttributes
                        {
                            variants = ShopContent.variants // variants
                        }
                    });

                    foreach (Item ItemInItems in ShopContent.items)
                    {

                        profileCacheEntry.AccountData.athena.Items.Add($"{ItemInItems.item}", new AthenaItem
                        {
                            templateId = $"{ItemInItems.item}",
                            attributes = new AthenaItemAttributes
                            {
                                variants = ItemInItems.variants // variants
                            }
                        });
                    }

                    profileCacheEntry.AccountData.commoncore.Items["Currency"].quantity = Price;
                    if (MultiUpdates.Count > 0)
                    {
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                    }

                    if (ApplyProfileChanges.Count > 0)
                    {
                        profileCacheEntry.AccountData.commoncore.RVN += 1;
                        profileCacheEntry.AccountData.commoncore.CommandRevision += 1;
                    }

                    if (Season.SeasonFull >= 12.20)
                    {
                        Mcp test = await CommonCoreResponse.Grab(profileCacheEntry.AccountId, ProfileId, Season, profileCacheEntry.AccountData.commoncore.RVN, profileCacheEntry);
                        ApplyProfileChanges = test.profileChanges;
                    }

                    Mcp mcp = new Mcp()
                    {
                        profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev,
                        profileChanges = ApplyProfileChanges,
                        notifications = new List<McpNotifications>()
                        {
                            new McpNotifications
                            {
                                type = "CatalogPurchase",
                                primary =  true,
                                lootResult = new LootResultClass
                                {
                                    items = NotificationsItems
                                }
                            }
                        },
                        profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        multiUpdate = new List<object>()
                        {
                            new
                            {
                                profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                profileId = "athena",
                                profileChangesBaseRevision = BaseRev2,
                                profileChanges = MultiUpdates,
                                profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            }
                        },
                        responseVersion = 1
                    };
                    string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                    Console.WriteLine(mcpJson);
                    return mcp;
                }
                else
                {
                    // This should be season shop
                    string SeasonShopFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Resources", "json", "shop", "special", "SeasonShop.json");
                    string SeasonShopJson = File.ReadAllText(SeasonShopFilePath);

                    if (string.IsNullOrEmpty(SeasonShopJson))
                    {
                        throw new BaseError()
                        {
                            errorCode = "errors.com.epicgames.modules.catalog",
                            errorMessage = "Server Sided Issue",
                            messageVars = new List<string> { "PurchaseCatalogEntry" },
                            numericErrorCode = 12801,
                            originatingService = "any",
                            intent = "prod",
                            error_description = "Server Sided Issue",
                        };
                    }

                    List<ItemsSaved> SeasonShopData = JsonConvert.DeserializeObject<List<ItemsSaved>>(SeasonShopJson)!;

                    foreach (ItemsSaved storefront in SeasonShopData)
                    {
                        if (storefront.id == SecondSplitOfferId)
                        {
                            ShopContent = storefront;
                        }
                    }

                    if (!string.IsNullOrEmpty(ShopContent.id))
                    {
                        bool HasUserHaveItem = profileCacheEntry.AccountData.athena.Items.Any(item => item.Key.Contains(ShopContent.id));

                        if (HasUserHaveItem)
                        {
                            throw new BaseError()
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "You already own the item",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "You already own the item",
                            };
                        }

                        SeasonClass SeasonData = profileCacheEntry.AccountData.commoncore.Seasons.FirstOrDefault(e => e.SeasonNumber == Season.Season);

                        if(SeasonData != null)
                        {
                            if (SeasonData.Level < ShopContent.MinLevel)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "Required To Be Level " + ShopContent.MinLevel,
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "You already own the item",
                                };
                            }

                            var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                            if (currencyItem.quantity == 0)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "Your Poor",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "Your Poor",
                                };
                            }

                            if (ShopContent.price > currencyItem.quantity)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "Item is higher price",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "Item is higher price",
                                };
                            }

                            int Price = currencyItem.quantity - ShopContent.price;

                            ApplyProfileChanges.Add(new ApplyProfileChangesClass
                            {
                                changeType = "itemQuantityChanged",
                                itemId = "Currency",
                                quantity = Price
                            });


                            profileCacheEntry.AccountData.athena.Items.Add($"{ShopContent.item}", new AthenaItem
                            {
                                templateId = $"{ShopContent.item}",
                                attributes = new AthenaItemAttributes
                                {
                                    variants = ShopContent.variants // variants
                                }
                            });


                            profileCacheEntry.AccountData.commoncore.Items["Currency"].quantity = Price;
                            if (MultiUpdates.Count > 0)
                            {
                                profileCacheEntry.AccountData.athena.RVN += 1;
                                profileCacheEntry.AccountData.athena.CommandRevision += 1;
                            }

                            if (ApplyProfileChanges.Count > 0)
                            {
                                profileCacheEntry.AccountData.commoncore.RVN += 1;
                                profileCacheEntry.AccountData.commoncore.CommandRevision += 1;
                            }

                            Mcp mcp = new Mcp()
                            {
                                profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                                profileId = ProfileId,
                                profileChangesBaseRevision = BaseRev,
                                profileChanges = ApplyProfileChanges,
                                notifications = new List<McpNotifications>()
                                {
                                    new McpNotifications
                                    {
                                        type = "CatalogPurchase",
                                        primary =  true,
                                        lootResult = new LootResultClass
                                        {
                                            items = NotificationsItems
                                        }
                                    }
                                },
                                profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                                serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                                multiUpdate = new List<object>()
                                {
                                    new
                                    {
                                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                        profileId = "athena",
                                        profileChangesBaseRevision = BaseRev2,
                                        profileChanges = MultiUpdates,
                                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                                    }
                                },
                                responseVersion = 1
                            };
                            string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                            return mcp;
                        }
                       
                    }
                }
            }

            return new Mcp();
        }
    }
}
