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
using FortLibrary.MongoDB.Module;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class ExchangeGameCurrencyForBattlePassOffer
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, ExchangeGameCurrencyForBattlePassOfferReq Body)
        {
            if (ProfileId == "athena")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                int BaseRev_C = profileCacheEntry.AccountData.commoncore.RVN;
                List<object> ProfileChanges = new();
                List<object> MultiUpdates = new();
                List<NotificationsItemsClassOG> NotificationsItems = new();

                SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons!.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;
                CommonCoreItem CurrentVbucks = profileCacheEntry.AccountData.commoncore.Items["Currency"];
                int OGBucks = CurrentVbucks.quantity;
                if (seasonObject != null)
                {
                    

                    if (BattlepassManager.BattlePassPItems.TryGetValue(Season.Season, out var bpItems) && bpItems.Count > 0)
                    {
                        var bpLookup = bpItems.ToDictionary(e => e.OfferId);

                        var resolvedItems = Body.offerItemIdList
                        .Select(id => bpLookup.TryGetValue(id, out var item) ? item : null)
                        .Where(item => item != null)
                        .OrderBy(item => item!.templateId.Contains("CosmeticVariantToken:", StringComparison.Ordinal));

                        foreach (var bpItm in resolvedItems)
                        {
                            if(bpItm is null) continue;

                            if (bpItm.bRequireBp && !seasonObject.BookPurchased) continue;
                            if (seasonObject.battlestars_currency >= bpItm.Price)
                            {
                                var SeasonOffer = seasonObject.season_offers.Find((e) => e.offerId == bpItm.OfferId);
                                if (SeasonOffer != null) continue; // they own it

                                Console.WriteLine(bpItm.templateId);

                                seasonObject.battlestars_currency -= bpItm.Price;

                                (profileCacheEntry, seasonObject, ProfileChanges, CurrentVbucks.quantity, NotificationsItems) =
                                    await BattlePassRewardsV2.Init(bpItm, profileCacheEntry, seasonObject, ProfileChanges, CurrentVbucks.quantity, NotificationsItems);

                                seasonObject.season_offers.Add(new()
                                {
                                    offerId = bpItm.OfferId,
                                    bIsFreePassReward = bpItm.bRequireBp,
                                    purchaseDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    lootResult = new() { 
                                        new() {
                                            itemType = bpItm.templateId,
                                            itemGuid = bpItm.templateId,
                                            itemProfile = !bpItm.templateId.Contains("HomebaseBannerIcon") ? "athena" : "common_core",
                                            quantity = bpItm.Quantity
                                        } 
                                    },
                                    currencyType = "battlestars",
                                    totalCurrencyPaid = bpItm.Price
                                });
                            }
                        }
                        

                    }//2E4B9D3E411F2CA6F7F7E8BAAD9348FF

                    if(OGBucks != CurrentVbucks.quantity)
                    {
                        MultiUpdates.Add(new
                        {
                            changeType = "itemQuantityChanged",
                            itemId = "Currency",
                            quantity = CurrentVbucks.quantity
                        });
                    }

                    ProfileChanges.Add(new
                    {
                        changeType = "statModified",
                        name = "purchased_bp_offers",
                        value = seasonObject.season_offers
                    });

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

                if (NotificationsItems.Count > 0)
                {
                    // this season doesnt need it?!?! else it doubles
                    //var RandomOfferId = Guid.NewGuid().ToString();
                    //MultiUpdates.Add(new ApplyProfileChangesClassV2
                    //{
                    //    changeType = "itemAdded",
                    //    itemId = RandomOfferId,
                    //    item = new
                    //    {
                    //        templateId = "GiftBox:gb_battlepassoffers",
                    //        attributes = new
                    //        {
                    //            max_level_bonus = 0,
                    //            fromAccountId = "",
                    //            lootList = NotificationsItems
                    //        },
                    //        quantity = 1
                    //    }
                    //});

                    //profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                    //{
                    //    templateId = "GiftBox:gb_battlepassoffers",
                    //    attributes = new GiftCommonCoreItemAttributes
                    //    {
                    //        lootList = NotificationsItems
                    //    },
                    //    quantity = 1
                    //});

                    //await XmppGift.NotifyUser(profileCacheEntry.AccountId);
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (MultiUpdates.Count > 0)
                    profileCacheEntry.AccountData.commoncore.BumpRevisions();

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    multiUpdate = new List<object>()
                    {
                        new
                        {
                            profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                            profileId = "common_core",
                            profileChangesBaseRevision = BaseRev_C,
                            profileChanges = MultiUpdates,
                            profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                        }
                    },
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

            }

            return new Mcp();
        }
    }
}
