using FortBackend.src.App.Utilities.Quests;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using System.Net;

namespace FortBackend.src.App.Utilities.Helpers.QuestsManagement
{
    public class QuestsDealer
    {
        public static async Task<(List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore)> Init(SeasonClass FoundSeason, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore)
        {
           
            if (WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary.TryGetValue($"Season{FoundSeason.SeasonNumber}", out List<WeeklyQuestsJson> WeeklyQuestsArray))
            {
                if (WeeklyQuestsArray.Count > 0)
                {
                    List<string> ResponseIgIdrk = new List<string>();
                    var ResponseId = "";
                    foreach (var kvp in WeeklyQuestsArray)
                    {
                        ResponseIgIdrk.Add($"ChallengeBundle:{kvp.BundleId}");
                        ResponseId = $"ChallengeBundleSchedule:{kvp.BundleSchedule}";
                        //ResponseId = $"ChallengeBundleSchedule:{kvp.BundleSchedule}";
                        //ResponseIgIdrk.Add($"ChallengeBundle:{kvp.BundleId}");
                        //kvp.BundleId
                        List<string> grantedquestinstanceids = new List<string>();


                        // THIS SHOULD SUPPORT ANY QUESTS.. IF DOESNT SAD!
                        foreach (var AllBundles in kvp.BundlesObject)
                        {
                            bool ShouldGrantItems = true;
                            if (AllBundles.quest_data.RequireBP)
                            {
                                if (!FoundSeason.BookPurchased)
                                {
                                    ShouldGrantItems = false;
                                }
                            }

                            if (ShouldGrantItems)
                            {
                                DailyQuestsData QuestData = FoundSeason.Quests.FirstOrDefault(e => e.Key == AllBundles.templateId).Value;
                                if (QuestData == null)
                                {
                                    if (!AllBundles.quest_data.ExtraQuests)
                                    {

                                        List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();

                                        foreach (WeeklyObjectsObjectives ObjectiveItems in AllBundles.Objectives)
                                        {
                                            QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                            {
                                                Name = $"completion_{ObjectiveItems.BackendName}",
                                                Value = 0,
                                                MaxValue = ObjectiveItems.Count
                                            });
                                        }

                                        FoundSeason.Quests.Add($"{AllBundles.templateId}", new DailyQuestsData
                                        {
                                            templateId = $"{AllBundles.templateId}",
                                            attributes = new DailyQuestsDataDB
                                            {
                                                challenge_bundle_id = $"ChallengeBundle:{kvp.BundleId}",
                                                sent_new_notification = false,
                                                ObjectiveState = QuestObjectStats,
                                            },
                                            quantity = 1
                                        });


                                        var ItemObjectResponse = new
                                        {
                                            templateId = $"{AllBundles.templateId}",
                                            attributes = new Dictionary<string, object>
                                                    {
                                                        { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
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
                                            itemId = $"{AllBundles.templateId}",
                                            item = ItemObjectResponse
                                        });
                                    }
                                }
                                else
                                {
                                    // HANDLE CLAIMING
                                    Logger.Error("TEST");
                                    if (FoundSeason.Quests[AllBundles.templateId].attributes.quest_state != "Claimed")
                                    {
                                        bool ShouldClaim = true;
                                        int Index = 0;
                                        foreach (DailyQuestsObjectiveStates ObjectiveState in FoundSeason.Quests[AllBundles.templateId].attributes.ObjectiveState)
                                        {
                                            if (ObjectiveState.Value != ObjectiveState.MaxValue)
                                            {
                                                ShouldClaim = false;
                                            }
                                            //FoundSeason.Quests[FreeBundles.templateId].attributes.ObjectiveState[Index].Value
                                            Index += 1;
                                        }

                                        if (ShouldClaim)
                                        {
                                            FoundSeason.Quests[AllBundles.templateId].attributes.quest_state = "Claimed";
                                            FoundSeason.BookXP += AllBundles.Rewards.BattleStars;

                                            // TO-DO only call this if theres 0 quests to add
                                            var ItemObjectResponse2 = new
                                            {
                                                templateId = $"{AllBundles.templateId}",
                                                attributes = new Dictionary<string, object>
                                                {
                                                        { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                        { "level", -1 },
                                                        { "item_seen", false },
                                                        { "playlists", new List<object>() },
                                                        { "sent_new_notification", true },
                                                        { "challenge_bundle_id", FoundSeason.Quests[AllBundles.templateId].attributes.challenge_bundle_id },
                                                        { "xp_reward_scalar", 1 },
                                                        { "challenge_linked_quest_given", "" },
                                                        { "quest_pool", "" },
                                                        { "quest_state", "Claimed" },
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

                                            foreach (DailyQuestsObjectiveStates yklist in FoundSeason.Quests[AllBundles.templateId].attributes.ObjectiveState)
                                            {
                                                ItemObjectResponse2.attributes.Add(yklist.Name, yklist.Value);
                                            }

                                            MultiUpdates.Add(new
                                            {
                                                changeType = "itemRemoved",
                                                itemId = AllBundles.templateId,
                                            });

                                            MultiUpdates.Add(new MultiUpdateClass
                                            {
                                                changeType = "itemAdded",
                                                itemId = $"{AllBundles.templateId}",
                                                item = ItemObjectResponse2
                                            });


                                            Logger.Error("TEST2");
                                            //WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary
                                            //  FoundSeason.Quests[Quests.Key].attributes.quest_state

                                            // might be used might not!
                                            if (!AllBundles.quest_data.ExtraQuests && !AllBundles.quest_data.Steps)
                                            {
                                                Logger.Error("TEST3");
                                                foreach (var item in FoundSeason.Quests)
                                                {
                                                    if (item.Value.attributes.quest_state == "Claimed") continue; // dw!
                                                    Logger.Error(item.Key);
                                                    //if (item.Value.attributes.questData.QuestType == "Normal")
                                                    //{
                                                    var Data = item.Value.attributes.ObjectiveState.FindIndex(e => e.Name.ToLower().Contains("weeklychallenges"));
                                                    if (Data != -1)
                                                    {
                                                        FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data].Value += 1;

                                                        // we need to now check if we want to claim this
                                                        var ObjectiveStateQuestData = FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data];

                                                        var FindQuestData = WeeklyQuestsArray.FirstOrDefault(e => e.BundleId.Contains(FoundSeason.Quests[item.Key].attributes.challenge_bundle_id));
                                                        if (FindQuestData != null)
                                                        {
                                                            if (ObjectiveStateQuestData.Value == ObjectiveStateQuestData.MaxValue)
                                                            {
                                                                // sets it to claimed but we still need to find the item to give
                                                                FoundSeason.Quests[item.Key].attributes.quest_state = "Claimed";

                                                                //var FindQuestData = WeeklyQuestsArray.FirstOrDefault(e => e.BundleId.Contains(FoundSeason.Quests[item.Key].attributes.challenge_bundle_id)).BundlesObject;


                                                                foreach (var FindItems in FindQuestData.BundlesObject)
                                                                {
                                                                    //if (!FindItems.templateId.ToLower().Contains("weeklychallenges")) continue;
                                                                    if (!FindItems.quest_data.IsWeekly) continue;

                                                                    foreach (var Item in FindItems.Objectives)
                                                                    {
                                                                        if (Item.BackendName.Contains(FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data].Name.Split("tion_")[1]))
                                                                        {
                                                                            FoundSeason.BookXP += FindItems.Rewards.BattleStars;

                                                                            foreach (var FuckThisLoop in FindItems.Rewards.Quest)
                                                                            {
                                                                                Logger.Error(FuckThisLoop.TemplateId);
                                                                            }

                                                                            foreach (var FuckThisLoop in FindItems.Rewards.GrantedItems)
                                                                            {
                                                                                Logger.Error(FuckThisLoop.TemplateId);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        // We need to remove and add back to update it live!

                                                        var ItemObjectResponse = new
                                                        {
                                                            templateId = $"{item.Key}",
                                                            attributes = new Dictionary<string, object>
                                                            {
                                                                    { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                    { "level", -1 },
                                                                    { "item_seen", false },
                                                                    { "playlists", new List<object>() },
                                                                    { "sent_new_notification", true },
                                                                    { "challenge_bundle_id", item.Value.attributes.challenge_bundle_id },
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

                                                        foreach (DailyQuestsObjectiveStates yklist in FoundSeason.Quests[item.Key].attributes.ObjectiveState)
                                                        {
                                                            ItemObjectResponse.attributes.Add(yklist.Name, yklist.Value);
                                                        }

                                                        MultiUpdates.Add(new
                                                        {
                                                            changeType = "itemRemoved",
                                                            itemId = item.Key,
                                                        });

                                                        MultiUpdates.Add(new MultiUpdateClass
                                                        {
                                                            changeType = "itemAdded",
                                                            itemId = $"{item.Key}",
                                                            item = ItemObjectResponse
                                                        });

                                                    }
                                                    //  }
                                                }
                                            }

                                            foreach (var GrantedItems in AllBundles.Rewards.GrantedItems)
                                            {
                                                Logger.Error("I WANT TO GIVE " + GrantedItems.TemplateId);
                                                //var GiveItenToUser = profileCacheEntry.AccountData.athena.
                                            }

                                            foreach (var Quests in AllBundles.Rewards.Quest)
                                            {
                                                var FoundNewQuest = kvp.BundlesObject.FirstOrDefault(e => e.templateId == Quests.TemplateId);
                                                if (FoundNewQuest != null)
                                                {
                                                    // ADD NEW QUESTS
                                                    Console.WriteLine($"I WANT TO ADD {FoundNewQuest.templateId}");

                                                    List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();

                                                    foreach (WeeklyObjectsObjectives ObjectiveItems in FoundNewQuest.Objectives)
                                                    {
                                                        QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                                        {
                                                            Name = $"completion_{ObjectiveItems.BackendName}",
                                                            Value = 0,
                                                            MaxValue = ObjectiveItems.Count
                                                        });
                                                    }

                                                    FoundSeason.Quests.Add($"{FoundNewQuest.templateId}", new DailyQuestsData
                                                    {
                                                        templateId = $"{FoundNewQuest.templateId}",
                                                        attributes = new DailyQuestsDataDB
                                                        {
                                                            challenge_bundle_id = $"ChallengeBundle:{kvp.BundleId}",
                                                            sent_new_notification = false,
                                                            ObjectiveState = QuestObjectStats,
                                                        },
                                                        quantity = 1
                                                    });


                                                    var ItemObjectResponse = new
                                                    {
                                                        templateId = $"{FoundNewQuest.templateId}",
                                                        attributes = new Dictionary<string, object>
                                                                {
                                                                    { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
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
                                                        itemId = $"{FoundNewQuest.templateId}",
                                                        item = ItemObjectResponse
                                                    });
                                                }
                                                else
                                                {
                                                    Console.WriteLine("I DONT SUPPORT " + Quests.TemplateId);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

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

                    // ProfileChange.Profile.items.Add(ResponseId, AthenaItemDynamicData);


                }
            }

            return (MultiUpdates, MultiUpdatesForCommonCore);
        }
    }
}
