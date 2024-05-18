using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

namespace FortBackend.src.App.Utilities.Helpers.QuestsManagement
{
    public class QuestsDealer
    {
        public static async Task<(List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry)> Init(SeasonClass FoundSeason, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry)
        {
            List<NotificationsItemsClassOG> TestIG = new List<NotificationsItemsClassOG>();
            


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
                           // Console.WriteLine(AllBundles.templateId);
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

                                     //   Console.WriteLine(AllBundles.templateId);

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

            if (WeeklyQuestManager.BPSeasonBundleScheduleDictionary.TryGetValue($"Season{FoundSeason.SeasonNumber}", out List<WeeklyQuestsJson> QuestsArray))
            {
                if (QuestsArray.Count > 0)
                {
                    //Console.WriteLine("T");
                    List<string> ResponseIgIdrk = new List<string>();
                    var ResponseId = "";
                    foreach (var kvp in QuestsArray)
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
                            // Console.WriteLine(AllBundles.templateId);
                            //Console.WriteLine(AllBundles.quest_data.RequireBP);
                           // Console.WriteLine(AllBundles.templateId);
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
                                if (QuestData != null)
                                {
                                    // HANDLE CLAIMING
                                    //Logger.Error("TEST");
                                    if (FoundSeason.Quests[AllBundles.templateId].attributes.quest_state != "Claimed")
                                    {
                                        bool ShouldClaim = true;
                                        int Index = 0;
                                       // Console.WriteLine(AllBundles.templateId);
                                        foreach (DailyQuestsObjectiveStates ObjectiveState in FoundSeason.Quests[AllBundles.templateId].attributes.ObjectiveState)
                                        {
                                            
                                          //  Console.WriteLine(ObjectiveState.Value);
                                           // Console.WriteLine(ObjectiveState.MaxValue);
                                            if (ObjectiveState.Value != ObjectiveState.MaxValue)
                                            {
                                                ShouldClaim = false;
                                            }
                                            //FoundSeason.Quests[FreeBundles.templateId].attributes.ObjectiveState[Index].Value
                                            Index += 1;
                                        }

                                        //   Console.WriteLine(AllBundles.templateId);

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


                                          //  Logger.Error("TEST2");
                                            //WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary
                                            //  FoundSeason.Quests[Quests.Key].attributes.quest_state

                                            // might be used might not!
                                            if (!AllBundles.quest_data.ExtraQuests && !AllBundles.quest_data.Steps)
                                            {
                                               // Logger.Error("TEST3");
                                                foreach (var item in FoundSeason.Quests)
                                                {
                                                    if (item.Value.attributes.quest_state == "Claimed") continue; // dw!
                                                   // Logger.Error(item.Key);
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
                                                if (!profileCacheEntry.AccountData.athena.Items.ContainsKey(GrantedItems.TemplateId))
                                                {
                                                    if (!profileCacheEntry.AccountData.commoncore.Items.ContainsKey(GrantedItems.TemplateId))
                                                    {
                                                        Logger.Error(GrantedItems.TemplateId);
                                                        if (GrantedItems.TemplateId.Contains("HomebaseBannerIcon"))
                                                        {
                                                            profileCacheEntry.AccountData.commoncore.Items.Add(GrantedItems.TemplateId, new CommonCoreItem
                                                            {
                                                                templateId = GrantedItems.TemplateId,
                                                                attributes = new CommonCoreItemAttributes
                                                                {
                                                                    item_seen = false
                                                                },
                                                                quantity = GrantedItems.Quantity,
                                                            });

                                                            MultiUpdates.Add(new MultiUpdateClass
                                                            {
                                                                changeType = "itemAdded",
                                                                itemId = GrantedItems.TemplateId,
                                                                item = new AthenaItem
                                                                {
                                                                    templateId = GrantedItems.TemplateId,
                                                                    attributes = new AthenaItemAttributes
                                                                    {
                                                                        item_seen = false,
                                                                    },
                                                                    quantity = 1
                                                                }
                                                            });
                                                        }
                                                        else if (GrantedItems.TemplateId.Contains("Athena"))
                                                        {
                                                            MultiUpdates.Add(new MultiUpdateClass
                                                            {
                                                                changeType = "itemAdded",
                                                                itemId = GrantedItems.TemplateId,
                                                                item = new AthenaItem
                                                                {
                                                                    templateId = GrantedItems.TemplateId,
                                                                    attributes = new AthenaItemAttributes
                                                                    {
                                                                        item_seen = false,
                                                                    },
                                                                    quantity = 1
                                                                }
                                                            });


                                                            profileCacheEntry.AccountData.athena.Items.Add(GrantedItems.TemplateId, new AthenaItem
                                                            {
                                                                templateId = GrantedItems.TemplateId,
                                                                attributes = new AthenaItemAttributes
                                                                {
                                                                    favorite = false,
                                                                    item_seen = false,
                                                                    level = 1,
                                                                    max_level_bonus = 0,
                                                                    rnd_sel_cnt = 0,
                                                                    variants = GrantedItems.variants,
                                                                    xp = 0
                                                                },
                                                                quantity = GrantedItems.Quantity,
                                                            });
                                                        }
                                                        else if (GrantedItems.TemplateId.Contains("CosmeticVariantToken:"))
                                                        {
                                                            Console.WriteLine(GrantedItems.connectedTemplate);
                                                            if (!string.IsNullOrEmpty(GrantedItems.connectedTemplate))
                                                            {
                                                                AthenaItem athenaItem = profileCacheEntry.AccountData.athena.Items.FirstOrDefault(e => e.Key == GrantedItems.connectedTemplate).Value;

                                                                if (athenaItem != null)
                                                                {
                                                                    var AddedVariants = athenaItem.attributes.variants;

                                                                    var NeedToAdd = GrantedItems.new_variants;

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

                                                                    profileCacheEntry.AccountData.athena.Items[GrantedItems.connectedTemplate].attributes.variants = AddedVariants;
                                                                    // athenaItem.attributes.variants.Add()


                                                                    MultiUpdates.Add(new
                                                                    {
                                                                        changeType = "itemAttrChanged",
                                                                        itemId = GrantedItems.connectedTemplate,
                                                                        attributeName = "variants",
                                                                        attributeValue = AddedVariants
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    Logger.Error(GrantedItems.connectedTemplate, "DONT EVEN OWN");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Logger.Error(GrantedItems.TemplateId, "CosmeticVariantToken");
                                                            }
                                                        }
                                                        else if (GrantedItems.TemplateId.Contains("Token:"))
                                                        {
                                                            if (GrantedItems.TemplateId.Contains("athenaseasonfriendxpboost"))
                                                            {
                                                                FoundSeason.season_friend_match_boost += GrantedItems.Quantity;

                                                                MultiUpdates.Add(new
                                                                {
                                                                    changeType = "statModified",
                                                                    name = "season_match_boost",
                                                                    value = FoundSeason.season_friend_match_boost
                                                                });
                                                            }
                                                            else if (GrantedItems.TemplateId.Contains("athenaseasonxpboost"))
                                                            {
                                                                //  season_match_boost
                                                                FoundSeason.season_match_boost += GrantedItems.Quantity;

                                                                MultiUpdates.Add(new
                                                                {
                                                                    changeType = "statModified",
                                                                    name = "season_friend_match_boost",
                                                                    value = FoundSeason.season_match_boost
                                                                });
                                                            }
                                                            else if (GrantedItems.TemplateId.Contains("athenaseasonmergedxpboosts"))
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
                                                                Logger.Log($"{GrantedItems.TemplateId} is not supported", "ClientQuestLogin");
                                                            }
                                                        }
                                                        //else if (FuckThisLoop.TemplateId.Contains("Currency:"))
                                                        //{
                                                        //    currencyItem.quantity += FuckThisLoop.Quantity;
                                                        //}
                                                        //else if (FuckThisLoop.TemplateId.Contains("AccountResource:"))
                                                        //{
                                                        //    FoundSeason.SeasonXP += FuckThisLoop.Quantity;

                                                        //    (FoundSeason, NeedItems) = await LevelUpdater.Init(FoundSeason.SeasonNumber, FoundSeason, NeedItems);
                                                        //    NeedItems = true; // force it as we don't want this to become false here
                                                        //}
                                                        else
                                                        {
                                                            Logger.Log($"{GrantedItems.TemplateId} is not supported!", "QuestsDealer");
                                                        }

                                                        TestIG.Add(new NotificationsItemsClassOG
                                                        {
                                                            itemType = GrantedItems.TemplateId,
                                                            itemGuid = GrantedItems.TemplateId,
                                                            quantity = GrantedItems.Quantity,
                                                        });
                                                    }
                                                }
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

            if (TestIG.Count() > 0)
            {
                var RandomOfferId = Guid.NewGuid().ToString();

                profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                {
                    templateId = "GiftBox:gb_challengecomplete",
                    attributes = new GiftCommonCoreItemAttributes
                    {
                        lootList = TestIG
                    },
                    quantity = 1
                });

                MultiUpdatesForCommonCore.Add(new ApplyProfileChangesClassV2
                {
                    changeType = "itemAdded",
                    itemId = RandomOfferId,
                    item = new
                    {
                        templateId = "GiftBox:gb_challengecomplete",
                        attributes = new
                        {
                            max_level_bonus = 0,
                            fromAccountId = "",
                            lootList = TestIG
                        },
                        quantity = 1
                    }
                });

                if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    Clients Client = GlobalData.Clients.FirstOrDefault(client => client.accountId == profileCacheEntry.AccountId)!;

                    if (Client != null)
                    {
                        string xmlMessage;
                        byte[] buffer;
                        WebSocket webSocket = Client.Game_Client;
                        Console.WriteLine(webSocket.State);
                        if (webSocket != null && webSocket.State == WebSocketState.Open)
                        {
                            XNamespace clientNs = "jabber:client";

                            var message = new XElement(clientNs + "message",
                              new XAttribute("from", $"xmpp-admin@prod.ol.epicgames.com"),
                              new XAttribute("to", profileCacheEntry.AccountId),
                              new XElement(clientNs + "body", Newtonsoft.Json.JsonConvert.SerializeObject(new
                              {
                                  payload = new { },
                                  type = "com.epicgames.gift.received",
                                  timestamp = DateTime.UtcNow.ToString("o")
                              }))
                            );

                            xmlMessage = message.ToString();
                            buffer = Encoding.UTF8.GetBytes(xmlMessage);

                            Console.WriteLine(xmlMessage);

                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }

                    }
                }

            }

            return (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);
        }
    }
}
