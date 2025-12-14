using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.XMPP.SERVER.Send;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.EpicResponses.Storefront;
using FortLibrary.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class PurchaseMultipleCatalogEntries
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, PurchaseMultipleCatalogEntriesReq Body)
        {
            if (ProfileId == "common_core")
            {
                int BaseRev_G = profileCacheEntry.AccountData.commoncore.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                int BaseRev_C = profileCacheEntry.AccountData.commoncore.RVN;

                List<object> ProfileChanges = new List<object>();
                List<object> MultiUpdates = new List<object>();

                SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                if (seasonObject != null)
                {
                    StoreBattlepassPages battlepass = BattlepassManager.BattlePasses.FirstOrDefault(e => e.Key == Season.Season).Value;
                    if (Body.purchaseInfoList.Count > 0)
                    {
                        foreach (PurchaseInfoList InfoList in Body.purchaseInfoList)
                        {
                            if (InfoList.purchaseQuantity <= 0) continue;
                            //if(InfoList.expectedTotalPrice) we wont use this price so idc


                            catalogEntrieStore ShopContent = new catalogEntrieStore();

                            foreach (catalogEntrieStore storefront in battlepass.catalogEntries)
                            {
                                if (storefront.offerId == InfoList.offerId)
                                {
                                    var test = InfoList.purchaseQuantity * storefront.prices[0].finalPrice;

                                    if (test > 0)
                                    {
                                        var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                                        if (test > currencyItem.quantity) continue;

                                        currencyItem.quantity -= test;

                                       //seasonObject.battlestars_currency += (5 * InfoList.purchaseQuantity);
                                        seasonObject.Level += InfoList.purchaseQuantity;
                                        bool NeedItems = true;
                                        var OgBattleStars = seasonObject.battlestars_currency;
                                        // todo update level
                                        (seasonObject, NeedItems) = await LevelUpdater.Init(seasonObject.SeasonNumber, seasonObject, NeedItems);

                                        ProfileChanges.Add(new
                                        {
                                            changeType = "statModified",
                                            name = "level",
                                            value = seasonObject.Level
                                        });

                                        if (OgBattleStars != seasonObject.battlestars_currency)
                                        {
                                            ProfileChanges.Add(new
                                            {
                                                changeType = "statModified",
                                                name = "battlestars",
                                                value = seasonObject.battlestars_currency
                                            });

                                            ProfileChanges.Add(new
                                            {
                                                changeType = "statModified",
                                                name = "battlestars_season_total",
                                                value = seasonObject.battlestars_currency
                                            });

                                        }

                                        MultiUpdates.Add(new
                                        {
                                            changeType = "itemQuantityChanged",
                                            itemId = "Currency",
                                            quantity = currencyItem.quantity
                                        });

                                        List<NotificationsItemsClassOG> ItemsGivenToUser = new()
                                        {
                                            new()
                                            {
                                                itemGuid = "AccountResource:AthenaBattleStar",
                                                itemType = "AccountResource:AthenaBattleStar",
                                                quantity = (5 * InfoList.purchaseQuantity)
                                            }
                                        };

                                        var RandomOfferId = Guid.NewGuid().ToString();
                                        MultiUpdates.Add(new ApplyProfileChangesClassV2
                                        {
                                            changeType = "itemAdded",
                                            itemId = RandomOfferId,
                                            item = new
                                            {
                                                templateId = "GiftBox:gb_battlepassoffers",
                                                attributes = new
                                                {
                                                    max_level_bonus = 0,
                                                    fromAccountId = "",
                                                    lootList = ItemsGivenToUser
                                                },
                                                quantity = 1
                                            }
                                        });

                                        profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                                        {
                                            templateId = "GiftBox:gb_battlepassoffers",
                                            attributes = new GiftCommonCoreItemAttributes
                                            {
                                                lootList = ItemsGivenToUser
                                            },
                                            quantity = 1
                                        });

                                        await XmppGift.NotifyUser(profileCacheEntry.AccountId);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (MultiUpdates.Count > 0)
                    profileCacheEntry.AccountData.commoncore.BumpRevisions();

                List<dynamic> ProfileChangesV2= new List<dynamic>();
                if (BaseRev_G != RVN)
                {
                    Mcp test = await CommonCoreResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChangesV2 = test.profileChanges;
                }
                else
                {
                    ProfileChangesV2 = MultiUpdates;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev_C,
                    profileChanges = ProfileChangesV2,
                    multiUpdate = new List<object>()
                    {
                        new
                        {
                            profileRevision = profileCacheEntry.AccountData.athena.RVN,
                            profileId = "athena",
                            profileChangesBaseRevision = BaseRev,
                            profileChanges = ProfileChanges,
                            profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                        }
                    },
                    profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

            }

            return new Mcp();
        }
    }
}
