using Discord;
using FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog.BP;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.XMPP.Data;
using FortBackend.src.XMPP.SERVER.Send;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.EpicResponses.Storefront;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;
using FortLibrary.XMPP;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers.PurchaseCatalog
{
    public class PurchaseBattlepass
    {
        public static async Task<Mcp> Init(VersionClass Season, string ProfileId, PurchaseCatalogEntryRequest Body, ProfileCacheEntry profileCacheEntry, StoreBattlepassPages battlepass)
        {
            string OfferId = Body.offerId;
            int Price = 0;

            List<object> MultiUpdates = new List<object>();
            List<object> ApplyProfileChanges = new List<object>();
            var NotificationsItems = new List<NotificationsItemsClass>();

            int BaseRev = Season.Season >= 17 ? profileCacheEntry.AccountData.commoncore.CommandRevision : profileCacheEntry.AccountData.commoncore.RVN;
            int BaseRev2 = Season.Season >= 17 ? profileCacheEntry.AccountData.athena.CommandRevision : profileCacheEntry.AccountData.athena.RVN;
            catalogEntrieStore ShopContent = new catalogEntrieStore();

            foreach (catalogEntrieStore storefront in battlepass.catalogEntries)
            {
                if (storefront.offerId == OfferId)
                {
                    ShopContent = storefront;
                    Price = storefront.prices[0].finalPrice;
                    break; // found it
                }
            }

            if (!string.IsNullOrEmpty(ShopContent.offerId))
            {
                List<SeasonClass> Seasons = profileCacheEntry.AccountData.commoncore.Seasons;

                if (profileCacheEntry.AccountData.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                    if (seasonObject != null)
                    {
                        
                        // I need to work on this
                        var currencyItem = profileCacheEntry.AccountData.commoncore.Items["Currency"];

                        if (ShopContent.devName.ToString().Contains("SingleTier"))
                        {
                            if (Body.purchaseQuantity <= 0) Body.purchaseQuantity = 1;
                            Price = Body.purchaseQuantity * Price;
                           
                        }

                        if (Price > currencyItem.quantity)
                        {
                            throw new BaseError()
                            {
                                errorCode = "errors.com.epicgames.modules.catalog",
                                errorMessage = "Not enough vbucks/ did you bypass",
                                messageVars = new List<string> { "PurchaseCatalogEntry" },
                                numericErrorCode = 12801,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Not enough vbucks",
                            };
                        }

                     

                        if (ShopContent.devName.ToString().Contains("SingleTier"))
                        {
                            if (!seasonObject.BookPurchased)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "Required Battlepass",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "Required Battlepass",
                                };
                            }

                           

                            //Body

                            bool NeedItems1 = true;
                            // THIS IS SO WE DONT GO CRAZY
                            int BookLevelOG = seasonObject.BookLevel;
                            int BattleStarCurrenOG = seasonObject.battlestars_currency;

                            seasonObject.BookXP += (10 * Body.purchaseQuantity);

                            if (Season.Season >= 11 && Season.Season < 18 && Price != 0)
                            {
                                seasonObject.BookLevel += Body.purchaseQuantity;
                                seasonObject.Level = seasonObject.BookLevel;
                                seasonObject.BookXP = seasonObject.BookLevel;
                            }
                            // i cant give this above season 19 yet
                            else if (Season.Season >= 18 && Season.Season < 20 && Price != 0)
                            {
                                seasonObject.battlestars_currency += (5 * Body.purchaseQuantity);
                            }

                            (seasonObject, NeedItems1) = await LevelUpdater.Init(Season.Season, seasonObject, NeedItems1);

                           // Console.WriteLine(NeedItems1);

                            List<Battlepass> FreeTier = BattlepassManager.FreeBattlePassItems.FirstOrDefault(e => e.Key == Season.Season).Value;

                            if (FreeTier != null)
                            {
                                if (FreeTier.Count > 0)
                                {
                                    if (Season.Season > 1)
                                    {
                                        List<Battlepass> PaidTier = BattlepassManager.PaidBattlePassItems.FirstOrDefault(e => e.Key == Season.Season).Value;

                                        if (PaidTier != null)
                                        {
                                            if (PaidTier.Count > 0)
                                            {
                                                currencyItem.quantity -= Price;

                                                //Logger.Warn("Sigma");
                                                // THIS SHOULD JUST BE IN A DIFFERENT FILE TO MAKE THIS CLEANER
                                                if (WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary.TryGetValue($"Season{seasonObject.SeasonNumber}", out List<WeeklyQuestsJson> WeeklyQuestsArray1))
                                                {
                                                    if (WeeklyQuestsArray1.Count > 0)
                                                    {
                                                        bool ShouldAddQuests = false;
                                                        List<string> ResponseIgIdrk = new List<string>();
                                                        var ResponseId = "";
                                                        foreach (var kvp in WeeklyQuestsArray1)
                                                        {
                                                            ResponseIgIdrk.Add($"ChallengeBundle:{kvp.BundleId}");
                                                            ResponseId = $"ChallengeBundleSchedule:{kvp.BundleSchedule}";

                                                            List<string> QuestTestResponse = new List<string>();
                                                            foreach (var Bundle in kvp.BundlesObject)
                                                            {
                                                                if (Bundle.quest_data.ExtraQuests) continue; // sigma!

                                                                QuestTestResponse.Add(Bundle.templateId);

                                                                if (Bundle.quest_data.RequireBP)
                                                                {
                                                                    QuestTestResponse.Add(Bundle.templateId);

                                                                    DailyQuestsData QuestData = seasonObject.Quests.FirstOrDefault(e => e.Key == Bundle.templateId).Value;
                                                                    if (QuestData == null)
                                                                    {
                                                                        ShouldAddQuests = true;

                                                                        List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();

                                                                        foreach (WeeklyObjectsObjectives ObjectiveItems in Bundle.Objectives)
                                                                        {
                                                                            //season_xp_gained

                                                                            QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                                            {
                                                                                Name = $"completion_{ObjectiveItems.BackendName}",
                                                                                Value = 0,
                                                                                MaxValue = ObjectiveItems.Count
                                                                            });
                                                                        }

                                                                        seasonObject.Quests.Add($"{Bundle.templateId}", new DailyQuestsData
                                                                        {
                                                                            templateId = $"{Bundle.templateId}",
                                                                            attributes = new DailyQuestsDataDB
                                                                            {
                                                                                challenge_bundle_id = $"ChallengeBundle:{kvp.BundleId}",
                                                                                sent_new_notification = false,
                                                                                ObjectiveState = QuestObjectStats
                                                                            },
                                                                            quantity = 1
                                                                        });


                                                                        var ItemObjectResponse = new
                                                                        {
                                                                            templateId = $"{Bundle.templateId}",
                                                                            attributes = new Dictionary<string, object>
                                                                            {
                                                                                { "creation_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                                { "level", -1 },
                                                                                { "item_seen", false },
                                                                                { "playlists", new List<object>() },
                                                                                { "sent_new_notification", true },
                                                                                { "challenge_bundle_id", $"ChallengeBundle:{kvp.BundleId}" },
                                                                                { "xp_reward_scalar", 1 },
                                                                                { "challenge_linked_quest_given", "" },
                                                                                { "quest_pool", "" },
                                                                                { "quest_state", "Active" },
                                                                                { "bucket", "" },
                                                                                { "last_state_change_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                                { "challenge_linked_quest_parent", "" },
                                                                                { "max_level_bonus", 0 },
                                                                                { "xp", 0 },
                                                                                { "quest_rarity", "uncommon" },
                                                                                { "favorite", false },
                                                                                // { $"completion_{dailyQuests.Properties.Objectives[0].BackendName}", 0 }
                                                                            },
                                                                            quantity = 1
                                                                        };

                                                                        foreach (DailyQuestsObjectiveStates yklist in QuestObjectStats)
                                                                        {
                                                                            ItemObjectResponse.attributes.Add(yklist.Name, yklist.Value);
                                                                        }

                                                                        ApplyProfileChanges.Add(new MultiUpdateClass
                                                                        {
                                                                            changeType = "itemAdded",
                                                                            itemId = $"{Bundle.templateId}",
                                                                            item = ItemObjectResponse
                                                                        });
                                                                    }


                                                                }
                                                            }

                                                            if (ShouldAddQuests)
                                                            {
                                                                var AthenaItemChallengeBundle = new AthenaItemDynamic
                                                                {
                                                                    templateId = $"ChallengeBundle:{kvp.BundleId}",
                                                                    attributes = new Dictionary<string, object>
                                                                    {
                                                                        { "has_unlock_by_completion", false },
                                                                        { "num_quests_completed", 0 },
                                                                        { "level", 0 },
                                                                        { "grantedquestinstanceids", QuestTestResponse.ToArray() },
                                                                        { "item_seen",  true },
                                                                        { "max_allowed_bundle_level", 0 },
                                                                        { "num_granted_bundle_quests", QuestTestResponse.Count() },
                                                                        { "max_level_bonus", 0 },
                                                                        { "challenge_bundle_schedule_id", ResponseId },
                                                                        { "num_progress_quests_completed", 0 },
                                                                        { "xp", 0 },
                                                                        { "favorite", false }
                                                                    },
                                                                    quantity = 1,
                                                                };

                                                                ApplyProfileChanges.Add(new
                                                                {
                                                                    changeType = "itemRemoved",
                                                                    itemId = $"ChallengeBundle:{kvp.BundleId}"
                                                                });

                                                                ApplyProfileChanges.Add(new MultiUpdateClass
                                                                {
                                                                    changeType = "itemAdded",
                                                                    itemId = $"ChallengeBundle:{kvp.BundleId}",
                                                                    item = AthenaItemChallengeBundle
                                                                });




                                                                var ItemTestObjectResponse = new
                                                                {
                                                                    templateId = ResponseId,
                                                                    attributes = new Dictionary<string, object>
                                                                    {
                                                                        { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                        { "max_level_bonus", 0 },
                                                                        { "level", 0 },
                                                                        { "item_seen", true },
                                                                        { "xp", 0 },
                                                                        { "favorite", false },
                                                                        { "granted_bundles", ResponseIgIdrk.ToArray() }
                                                                    },
                                                                    quantity = 1,
                                                                };

                                                                MultiUpdates.Add(new
                                                                {
                                                                    changeType = "itemRemoved",
                                                                    itemId = ResponseId,
                                                                });


                                                                MultiUpdates.Add(new MultiUpdateClass
                                                                {
                                                                    changeType = "itemAdded",
                                                                    itemId = ResponseId,
                                                                    item = ItemTestObjectResponse
                                                                });
                                                            }
                                                        }
                                                    }
                                                }

                                                // ^^ SHOULD BE IN A DIFFETN FILE



                                                List<NotificationsItemsClassOG> ItemsGivenToUser = new List<NotificationsItemsClassOG>();

                                                foreach (Battlepass BattlePass in FreeTier)
                                                {
                                                    if (!NeedItems1) break;
                                                    //We don't need this check on purchase as we "WANT" the user to get them items
                                                    if (BattlePass.Level <= BookLevelOG) continue;
                                                    if (BattlePass.Level > seasonObject.BookLevel) break;

                                                    (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems1, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems1, ItemsGivenToUser);
                                                }

                                                foreach (Battlepass BattlePass in PaidTier)
                                                {
                                                    if (!NeedItems1) break;
                                                    if (BattlePass.Level <= BookLevelOG) continue;
                                                    if (BattlePass.Level > seasonObject.BookLevel) break;


                                                    (profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems1, ItemsGivenToUser) = await BattlePassRewards.Init(BattlePass.Rewards, profileCacheEntry, seasonObject, ApplyProfileChanges, currencyItem, NeedItems1, ItemsGivenToUser);
                                                }

                                                ApplyProfileChanges.Add(new
                                                {
                                                    changeType = "statModified",
                                                    name = $"book_level",
                                                    value = seasonObject.BookLevel
                                                });

                                                ApplyProfileChanges.Add(new
                                                {
                                                    changeType = "statModified",
                                                    name = $"level",
                                                    value = seasonObject.Level
                                                });

                                                if (BattleStarCurrenOG != seasonObject.battlestars_currency)
                                                {
                                                    ApplyProfileChanges.Add(new
                                                    {
                                                        changeType = "statModified",
                                                        name = "battlestars",
                                                        value = seasonObject.battlestars_currency
                                                    });

                                                    ApplyProfileChanges.Add(new
                                                    {
                                                        changeType = "statModified",
                                                        name = "battlestars_season_total",
                                                        value = seasonObject.battlestars_currency
                                                    });
                                                }

                                                // after to get the correct price
                                                MultiUpdates.Add(new
                                                {
                                                    changeType = "itemQuantityChanged",
                                                    itemId = "Currency",
                                                    quantity = currencyItem.quantity
                                                });
                                                /*
                                                 *   NewItemsGiven.Add(new Dictionary<string, object>
                                                    {
                                                        { "itemType", FreeRewards["TemplateId"].ToString() },
                                                        { "itemGuid", FreeRewards["TemplateId"].ToString() },
                                                        { "quantity", int.Parse(FreeRewards["Quantity"].ToString() ?? "1") }
                                                    });
                                                */
                                                var RandomOfferId = Guid.NewGuid().ToString();
                                                MultiUpdates.Add(new ApplyProfileChangesClassV2
                                                {
                                                    changeType = "itemAdded",
                                                    itemId = RandomOfferId,
                                                    item = new
                                                    {
                                                        templateId = "GiftBox:gb_battlepass",
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
                                                    templateId = "GiftBox:gb_battlepass",
                                                    attributes = new GiftCommonCoreItemAttributes
                                                    {
                                                        lootList = ItemsGivenToUser
                                                    },
                                                    quantity = 1
                                                });

                                                await XmppGift.NotifyUser(profileCacheEntry.AccountId);
                                            }
                                        }

                                    }

                                }
                            }
                        }
                        else
                        {
                            // not proper response
                            if (seasonObject.BookPurchased)
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "You already own the bp",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "You already own the bp",
                                };
                            }

                            if (ShopContent.devName.Contains("Battle"))
                            {

                                bool NeedItems = true;
                                int BookLevelOG = seasonObject.BookLevel;
                                int BattleStarsOG = seasonObject.battlestars_currency;

                                if (ShopContent.devName.Contains("Bundle")) {
                                    // We dont want this to be skunked
                                    if (!(seasonObject.BookLevel >= 25))
                                    {
                                        seasonObject.BookLevel = 25;
                                        if (Season.Season >= 11 && Season.Season < 17)
                                        {
                                            seasonObject.Level = 25;
                                            ApplyProfileChanges.Add(new
                                            {
                                                changeType = "statModified",
                                                name = $"level",
                                                value = seasonObject.BookLevel
                                            });
                                        }
                                    }

                                    (seasonObject, NeedItems) = await LevelUpdater.Init(Season.Season, seasonObject, NeedItems);
                                }

                                (seasonObject, currencyItem, ApplyProfileChanges, MultiUpdates, profileCacheEntry) = await BattlePurchase.Init(seasonObject, Season, currencyItem, Price, ApplyProfileChanges, MultiUpdates, profileCacheEntry, NeedItems);

                                if(BattleStarsOG != seasonObject.battlestars_currency)
                                {
                                    ApplyProfileChanges.Add(new
                                    {
                                        changeType = "statModified",
                                        name = "battlestars",
                                        value = seasonObject.battlestars_currency
                                    });

                                    ApplyProfileChanges.Add(new
                                    {
                                        changeType = "statModified",
                                        name = "battlestars_season_total",
                                        value = seasonObject.battlestars_currency
                                    });
                                }

                                ApplyProfileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = $"book_level",
                                    value = seasonObject.BookLevel
                                });
                            

                            }
                            //else if (ShopContent.devName.ToString().Contains("BattlePass"))
                            //{
                            //    bool NeedItems = true;
                            //    //int BookLevelOG = seasonObject.BookLevel;
                            //    (seasonObject, currencyItem, ApplyProfileChanges, MultiUpdates, profileCacheEntry) = await BattlePurchase.Init(seasonObject, Season, currencyItem, Price, ApplyProfileChanges, MultiUpdates, profileCacheEntry, NeedItems);
                            //}
                            else
                            {
                                throw new BaseError()
                                {
                                    errorCode = "errors.com.epicgames.modules.catalog",
                                    errorMessage = "?????",
                                    messageVars = new List<string> { "PurchaseCatalogEntry" },
                                    numericErrorCode = 12801,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "?????",
                                };
                            }
                        }


                        // Response

                        if (ApplyProfileChanges.Count > 0)
                        {
                            profileCacheEntry.AccountData.athena.RVN += 1;
                            profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        }

                        if (MultiUpdates.Count > 0)
                        {
                            profileCacheEntry.AccountData.commoncore.RVN += 1;
                            profileCacheEntry.AccountData.commoncore.CommandRevision += 1;
                        }


                        Mcp mcp = new Mcp()
                        {
                            profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = BaseRev,
                            profileChanges = MultiUpdates,
                            //notifications = new List<McpNotifications>()
                            //{
                            //    new McpNotifications
                            //    {
                            //        type = "CatalogPurchase",
                            //        primary =  true,
                            //        lootResult = new LootResultClass
                            //        {
                            //            items = NotificationsItems
                            //        }
                            //    }
                            //},
                            profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                            serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            multiUpdate = new List<object>()
                            {
                                new
                                {
                                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                                    profileId = "athena",
                                    profileChangesBaseRevision = BaseRev2,
                                    profileChanges = ApplyProfileChanges,
                                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                                }
                            },
                            responseVersion = 1
                        };

                        string mcpJson = JsonConvert.SerializeObject(mcp, Formatting.Indented);
                        //Console.WriteLine(mcpJson);

                        return mcp;
                    }
                }
            }

            return new Mcp();
        }
    }
}
