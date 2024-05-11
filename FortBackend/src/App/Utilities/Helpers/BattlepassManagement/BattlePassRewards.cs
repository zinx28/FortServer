using Discord;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class BattlePassRewards
    {
        public static async Task<(ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges)> Init(List<ItemInfo> Rewards, ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges = null)
        {
            foreach (ItemInfo iteminfo in Rewards)
            {
                if (!profileCacheEntry.AccountData.athena.Items.ContainsKey(iteminfo.TemplateId))
                {
                    if (!profileCacheEntry.AccountData.commoncore.Items.ContainsKey(iteminfo.TemplateId))
                    {
                        if (iteminfo.TemplateId.Contains("HomebaseBannerIcon"))
                        {
                            profileCacheEntry.AccountData.commoncore.Items.Add(iteminfo.TemplateId, new CommonCoreItem
                            {
                                templateId = iteminfo.TemplateId,
                                attributes = new CommonCoreItemAttributes
                                {
                                    item_seen = false
                                },
                                quantity = iteminfo.Quantity,
                            });
                        }
                        else if (iteminfo.TemplateId.Contains("Athena"))
                        {
                            /*
                             *  {
                                  "changeType": "itemAdded",
                                  "itemId": "AthenaPickaxe:Pickaxe_ID_082_SushiChef",
                                  "item": {
                                    "templateId": "AthenaPickaxe:Pickaxe_ID_082_SushiChef",
                                    "attributes": {
                                      "favorite": false,
                                      "item_seen": false,
                                      "level": 1,
                                      "max_level_bonus": 0,
                                      "rnd_sel_cnt": 0,
                                      "variants": [],
                                      "xp": 0
                                    },
                                    "quantity": 1
                                  }
                                }*/
                            MultiUpdates.Add(new MultiUpdateClass
                            {
                                changeType = "itemAdded",
                                itemId = iteminfo.TemplateId,
                                item = new AthenaItem
                                {
                                    templateId = iteminfo.TemplateId,
                                    attributes = new AthenaItemAttributes
                                    {
                                        item_seen = false,
                                    },
                                    quantity = 1
                                }
                            });
                            //MultiUpdates.Add(new
                            //{
                            //    changeType = "itemAttrChanged",
                            //    itemId = iteminfo.TemplateId,
                            //    attributeName = "item_seen",
                            //    attributeValue = false
                            //});

                            profileCacheEntry.AccountData.athena.Items.Add(iteminfo.TemplateId, new AthenaItem
                            {
                                templateId = iteminfo.TemplateId,
                                attributes = new AthenaItemAttributes
                                {
                                    favorite = false,
                                    item_seen = false,
                                    level = 1,
                                    max_level_bonus = 0,
                                    rnd_sel_cnt = 0,
                                    variants = new List<AthenaItemVariants>(),
                                    xp = 0
                                },
                                quantity = iteminfo.Quantity,
                            });
                        }
                        else if (iteminfo.TemplateId.Contains("Token:"))
                        {
                            if (iteminfo.TemplateId.Contains("athenaseasonfriendxpboost"))
                            {
                                FoundSeason.season_friend_match_boost += iteminfo.Quantity;

                                MultiUpdates.Add(new
                                {
                                    changeType = "statModified",
                                    name = "season_match_boost",
                                    value = FoundSeason.season_friend_match_boost
                                });
                            }
                            else if (iteminfo.TemplateId.Contains("athenaseasonxpboost"))
                            {
                                //  season_match_boost
                                FoundSeason.season_match_boost += iteminfo.Quantity;

                                MultiUpdates.Add(new
                                {
                                    changeType = "statModified",
                                    name = "season_friend_match_boost",
                                    value = FoundSeason.season_match_boost
                                });
                            }
                            else if (iteminfo.TemplateId.Contains("athenaseasonmergedxpboosts"))
                            {
                                //FoundSeason.season_merged_boosts += iteminfo.Quantity;

                                //MultiUpdates.Add(new
                                //{
                                //    changeType = "statModified",
                                //    name = ""
                                //})
                            }
                            else
                            {
                                Logger.Log($"{iteminfo.TemplateId} is not supported", "ClientQuestLogin");
                            }
                        }
                        else if (iteminfo.TemplateId.Contains("Currency:"))
                        {
                            currencyItem.quantity += iteminfo.Quantity;
                        }
                        else if (iteminfo.TemplateId.Contains("AccountResource:"))
                        {
                            FoundSeason.SeasonXP += iteminfo.Quantity;

                            (FoundSeason, NeedItems) = await LevelUpdater.Init(FoundSeason.SeasonNumber, FoundSeason, NeedItems);
                            NeedItems = true; // force it as we don't want this to become false here
                        }
                        else
                        {
                            if (iteminfo.TemplateId.Contains("ChallengeBundleSchedule"))
                            {
                                if (WeeklyQuestManager.BPSeasonBundleScheduleDictionary.TryGetValue($"Season{FoundSeason.SeasonNumber}", out List<WeeklyQuestsJson> BPQuestsArray))
                                {
                                    if (BPQuestsArray.Count > 0)
                                    {
                                        var matchingQuestJsons = BPQuestsArray.Where(e => e.BundleSchedule == iteminfo.TemplateId).ToList();

                                        foreach (var QuestJson in matchingQuestJsons)
                                        {
                                            if (QuestJson == null) continue;

                                            List<string> TEST2FRFR = new List<string>();
                                            foreach (var test in QuestJson.PaidBundleObject)
                                            {
                                                DailyQuestsData QyestData = FoundSeason.Quests.FirstOrDefault(e => e.Key == test.templateId).Value;

                                                if (QyestData == null)
                                                {
                                                    List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();

                                                    foreach (WeeklyObjectsObjectives ObjectiveItems in test.Objectives)
                                                    {
                                                        QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                        {
                                                            Name = $"completion_{ObjectiveItems.BackendName}",
                                                            Value = 0
                                                        });
                                                    }

                                                    FoundSeason.Quests.Add($"Quest:{test.templateId}", new DailyQuestsData
                                                    {
                                                        templateId = $"Quest:{test.templateId}",
                                                        attributes = new DailyQuestsDataDB
                                                        {
                                                            challenge_bundle_id = $"ChallengeBundle:{QuestJson.BundleId}",
                                                            sent_new_notification = false,
                                                            ObjectiveState = QuestObjectStats
                                                        },
                                                        quantity = 1
                                                    });

                                                    var ItemObjectResponse = new
                                                    {
                                                        templateId = $"Quest:{test.templateId}",
                                                        attributes = new Dictionary<string, object>
                                                        {
                                                            { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                            { "level", -1 },
                                                            { "item_seen", false },
                                                            { "playlists", new List<object>() },
                                                            { "sent_new_notification", true },
                                                            { "challenge_bundle_id", $"ChallengeBundle:{QuestJson.BundleId}" },
                                                            { "xp_reward_scalar", 1 },
                                                            { "challenge_linked_quest_given", "" },
                                                            { "quest_pool", "" },
                                                            { "quest_state", "Active" },
                                                            { "bucket", "" },
                                                            { "last_state_change_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
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

                                                    MultiUpdates.Add(new MultiUpdateClass
                                                    {
                                                        changeType = "itemAdded",
                                                        itemId = $"Quest:{test.templateId}",
                                                        item = ItemObjectResponse
                                                    });
                                                }
                                            }
                                        }
                                            // WeeklyQuestsJson QuestJson = BPQuestsArray.FirstOrDefault(e => e.BundleSchedule == iteminfo.TemplateId);
                          
                                    }
                                }
                            }
                           
                            Logger.Log($"{iteminfo.TemplateId} is not supported", "ClientQuestLogin");
                        }
                        
                        /*
                         * [ClientQuestLogin]: ChallengeBundleSchedule:season8_paid_schedule is not supported
                            [ClientQuestLogin]: ChallengeBundleSchedule:season8_cumulative_schedule is not supported
                            [ClientQuestLogin]: Token:athenaseasonmergedxpboosts is not supported
                        */

                        applyProfileChanges.Add(new NotificationsItemsClassOG
                        {
                            itemType = iteminfo.TemplateId,
                            itemGuid = iteminfo.TemplateId,
                            quantity = iteminfo.Quantity,
                        });
                    }
                }
            }

            return (profileCacheEntry, FoundSeason, MultiUpdates, currencyItem, NeedItems, applyProfileChanges);
        }
    }
}
