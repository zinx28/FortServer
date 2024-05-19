using Discord;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace FortBackend.src.App.Utilities.Helpers.BattlepassManagement
{
    public class BattlePassRewards
    {
        public static async Task<(ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges)> Init(List<ItemInfo> Rewards, ProfileCacheEntry profileCacheEntry, SeasonClass FoundSeason, List<object> MultiUpdates, CommonCoreItem currencyItem, bool NeedItems, List<NotificationsItemsClassOG> applyProfileChanges = null)
        {
            List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;
            var beforeLevelXPElement = SeasonXpIg.FirstOrDefault(e => e.Level == FoundSeason.Level);
            //
            int CurrentLevelXP;
            if (beforeLevelXPElement != null && SeasonXpIg.IndexOf(beforeLevelXPElement) == SeasonXpIg.Count - 1)
            {
                FoundSeason.SeasonXP = 0;
            }

            CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (beforeLevelXPElement.XpTotal + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;


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
                                    variants = iteminfo.variants,
                                    xp = 0
                                },
                                quantity = iteminfo.Quantity,
                            });
                        }
                        else if (iteminfo.TemplateId.Contains("CosmeticVariantToken:"))
                        {
                            Console.WriteLine(iteminfo.connectedTemplate);
                            if (!string.IsNullOrEmpty(iteminfo.connectedTemplate))
                            {
                                AthenaItem athenaItem = profileCacheEntry.AccountData.athena.Items.FirstOrDefault(e => e.Key == iteminfo.connectedTemplate).Value;

                                if (athenaItem != null)
                                {
                                    var AddedVariants = athenaItem.attributes.variants;

                                    var NeedToAdd = iteminfo.new_variants;

                                    foreach (var variant in NeedToAdd)
                                    {
                                        var existingVariant = AddedVariants.FirstOrDefault(v => v.channel == variant.channel);
                                        if (existingVariant != null)
                                        {
                                            existingVariant.owned.AddRange(variant.added);
                                        }
                                        else
                                        {
                                            var newVariant = new AthenaItemVariants
                                            {
                                                channel = variant.channel,
                                                active = variant.added.First(),
                                                owned = variant.added
                                            };
                                            AddedVariants.Add(newVariant);
                                        }
                                    }

                                    profileCacheEntry.AccountData.athena.Items[iteminfo.connectedTemplate].attributes.variants = AddedVariants;
                                    // athenaItem.attributes.variants.Add()


                                    MultiUpdates.Add(new
                                    {
                                        changeType = "itemAttrChanged",
                                        itemId = iteminfo.connectedTemplate,
                                        attributeName = "variants",
                                        attributeValue = AddedVariants
                                    });
                                }else
                                {
                                    Logger.Error(iteminfo.connectedTemplate, "DONT EVEN OWN");
                                }
                            }
                            else
                            {
                                Logger.Error(iteminfo.TemplateId, "CosmeticVariantToken");
                            }
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
                                        var ResponseIG = new Dictionary<string, List<string>>();

                                        foreach (var QuestJson in matchingQuestJsons)
                                        {
                                            if (QuestJson == null) continue;

                                            List<string> FindFirstOrDe = ResponseIG.FirstOrDefault(e => e.Key == QuestJson.BundleSchedule).Value;
                                            if (FindFirstOrDe == null || FindFirstOrDe.Count == 0)
                                            {
                                                ResponseIG[QuestJson.BundleSchedule] = new List<string> { $"ChallengeBundle:{QuestJson.BundleId}" };
                                            }
                                            else
                                            {
                                                FindFirstOrDe.Add($"ChallengeBundle:{QuestJson.BundleId}");
                                            }
                                            int CurrentXP = 0;
                                            List<string> OkayIG = new List<string>();

                                            foreach (var Quests in QuestJson.BundlesObject)
                                            {
                                                if (Quests.quest_data.ExtraQuests) continue;


                                                OkayIG.Add(Quests.templateId);

                                     
                                                DailyQuestsData QyestData = FoundSeason.Quests.FirstOrDefault(e => e.Key == Quests.templateId).Value;

                                                if (QyestData == null)
                                                {
                                                    List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();
                                                    //  int AmountCompleted = 0;
                                                    foreach (WeeklyObjectsObjectives ObjectiveItems in Quests.Objectives)
                                                    {
                                                        //CurrentLevelXP
                                                        if (ObjectiveItems.BackendName.ToLower().Contains("_xp_"))
                                                        {
                                                            if (CurrentLevelXP >= ObjectiveItems.Count)
                                                            {
                                                                //  AmountCompleted += 1;
                                                                QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                                {
                                                                    Name = $"completion_{ObjectiveItems.BackendName}",
                                                                    Value = ObjectiveItems.Count,
                                                                    MaxValue = ObjectiveItems.Count
                                                                });
                                                            }
                                                            else
                                                            {
                                                                QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                                {
                                                                    Name = $"completion_{ObjectiveItems.BackendName}",
                                                                    Value = CurrentLevelXP,
                                                                    MaxValue = ObjectiveItems.Count
                                                                });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                            {
                                                                Name = $"completion_{ObjectiveItems.BackendName}",
                                                                Value = 0,
                                                                MaxValue = ObjectiveItems.Count
                                                            });
                                                        }

                                                    }

                                                    FoundSeason.Quests.Add($"{Quests.templateId}", new DailyQuestsData
                                                    {
                                                        templateId = $"{Quests.templateId}",
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
                                                        templateId = $"{Quests.templateId}",
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
                                                        itemId = $"{Quests.templateId}",
                                                        item = ItemObjectResponse
                                                    });
                                                }
                                                

                                            }




                                            //  FoundSeason.Level



                                            var TempValueIG = QuestJson.BundleSchedule;
                                            if (!QuestJson.BundleSchedule.Contains("ChallengeBundleSchedule:"))
                                            {
                                                TempValueIG = $"ChallengeBundleSchedule:{QuestJson.BundleSchedule}";
                                            }

                                            var AthenaItemChallengeBundle = new
                                            {
                                                templateId = $"ChallengeBundle:{QuestJson.BundleId}",
                                                attributes = new Dictionary<string, object>
                                                {
                                                    { "has_unlock_by_completion", false },
                                                    { "num_quests_completed", 0 },
                                                    { "level", 0 },
                                                    { "grantedquestinstanceids", OkayIG.ToArray() },
                                                    { "item_seen",  true },
                                                    { "max_allowed_bundle_level", 0 },
                                                    { "num_granted_bundle_quests", OkayIG.Count() },
                                                    { "max_level_bonus", 0 },
                                                    { "challenge_bundle_schedule_id", TempValueIG },
                                                    { "num_progress_quests_completed", 0 },
                                                    { "xp", 0 },
                                                    { "favorite", false }
                                                },
                                                quantity = 1,
                                            };


                                            MultiUpdates.Add(new
                                            {
                                                changeType = "itemRemoved",
                                                itemId = $"ChallengeBundle:{QuestJson.BundleId}",
                                            });

                                            MultiUpdates.Add(new MultiUpdateClass
                                            {
                                                changeType = "itemAdded",
                                                itemId = $"ChallengeBundle:{QuestJson.BundleId}",
                                                item = AthenaItemChallengeBundle
                                            });
                                        }
                                        foreach (var kvp in ResponseIG)
                                        {
                                            var ItemTestObjectResponse = new
                                            {
                                                templateId = kvp.Key,
                                                attributes = new Dictionary<string, object>
                                                {
                                                    { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                    { "max_level_bonus", 0 },
                                                    { "level", 0 },
                                                    { "item_seen", true },
                                                    { "xp", 0 },
                                                    { "favorite", false },
                                                    { "granted_bundles", kvp.Value.ToArray() }
                                                },
                                                quantity = 1,
                                            };

                                            MultiUpdates.Add(new
                                            {
                                                changeType = "itemRemoved",
                                                itemId = kvp.Key,
                                            });

                                            MultiUpdates.Add(new MultiUpdateClass
                                            {
                                                changeType = "itemAdded",
                                                itemId = kvp.Key,
                                                item = ItemTestObjectResponse
                                            });
                                        };
                                        // WeeklyQuestsJson QuestJson = BPQuestsArray.FirstOrDefault(e => e.BundleSchedule == iteminfo.TemplateId);

                                    }
                                }
                            }
                            else
                            {
                                Logger.Log($"{iteminfo.TemplateId} is not supported", "ClientQuestLogin");
                            }
                           
                           
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
