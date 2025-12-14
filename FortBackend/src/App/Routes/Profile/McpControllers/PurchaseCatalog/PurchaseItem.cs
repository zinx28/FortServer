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
using FortLibrary;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.MongoDB.Extentions;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseItem
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry, int RVN)
        {
           
            ShopJson shopData = Saved.BackendCachedData.CurrentShop;

            if (shopData != null)
            {
                string[] SplitOfferId = Body.offerId.Split(":/");
                string SecondSplitOfferId = SplitOfferId[1];
                ItemsSaved ShopContent = new ItemsSaved();
                var NotificationsItems = new List<NotificationsItemsClass>();
                var MultiUpdates = new List<object>();
                var ApplyProfileChanges = new List<object>();
                List<Dictionary<string, object>> itemList = new List<Dictionary<string, object>>();
                int BaseRev_G = profileCacheEntry.AccountData.commoncore.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.commoncore.RVN;
                int BaseRev_A = profileCacheEntry.AccountData.athena.RVN;

                foreach (ItemsSaved storefront in shopData.ShopItems.Daily)
                {
                    if (storefront.id == SecondSplitOfferId)
                    {
                        ShopContent = storefront;
                        break;
                    }
                }

                foreach (ItemsSaved storefront in shopData.ShopItems.Weekly)
                {
                    if (storefront.id == SecondSplitOfferId)
                    {
                        ShopContent = storefront;
                        break;
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

                    Logger.PlainLog(HasUserHaveItem);

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

                    CommonCoreItem CurrentVbucks = profileCacheEntry.AccountData.commoncore.Items["Currency"];
       
                    if (CurrentVbucks.quantity == 0)
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

                    if (ShopContent.price > int.Parse(CurrentVbucks.quantity.ToString()))
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

                    int Price = CurrentVbucks.quantity - ShopContent.price;

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
                        profileCacheEntry.AccountData.athena.BumpRevisions();

                    if (ApplyProfileChanges.Count > 0)
                        profileCacheEntry.AccountData.commoncore.BumpRevisions();

                    if (BaseRev_G != RVN)
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
                        notifications = new List<object>()
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
                        serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        multiUpdate = new List<object>()
                        {
                            new
                            {
                                profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                profileId = "athena",
                                profileChangesBaseRevision = BaseRev_A,
                                profileChanges = MultiUpdates,
                                profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            }
                        },
                        responseVersion = 1
                    };
                    string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                    return mcp;
                }
                else
                {
                    // This should be season shop


                    List<ItemsSaved> SeasonShopData = Saved.BackendCachedData.OGShop;

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

                        SeasonClass SeasonData = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(e => e.SeasonNumber == Season.Season)!;

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

                            CommonCoreItem CurrentVbucks = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                            if (CurrentVbucks.quantity == 0)
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

                            if (ShopContent.price > CurrentVbucks.quantity)
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

                            int Price = CurrentVbucks.quantity - ShopContent.price;

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
                                profileCacheEntry.AccountData.athena.BumpRevisions();

                            if (ApplyProfileChanges.Count > 0)
                                profileCacheEntry.AccountData.commoncore.BumpRevisions();

                            Mcp mcp = new Mcp()
                            {
                                profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                                profileId = ProfileId,
                                profileChangesBaseRevision = BaseRev,
                                profileChanges = ApplyProfileChanges,
                                notifications = new List<object>()
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
                                serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                multiUpdate = new List<object>()
                                {
                                    new
                                    {
                                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                        profileId = "athena",
                                        profileChangesBaseRevision = BaseRev_A,
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
