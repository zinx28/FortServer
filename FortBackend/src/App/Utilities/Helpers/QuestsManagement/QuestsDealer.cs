using Discord;
using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortBackend.src.App.Utilities.Quests;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;
using FortLibrary.Shop;
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
            if (WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary.TryGetValue($"Season{FoundSeason.SeasonNumber}", out List<WeeklyQuestsJson>? WeeklyQuestsArray))
            {
                if (WeeklyQuestsArray.Count > 0)
                {
                    (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestGrabber(FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry, WeeklyQuestsArray);
                }
            }

            if (WeeklyQuestManager.BPSeasonBundleScheduleDictionary.TryGetValue($"Season{FoundSeason.SeasonNumber}", out List<WeeklyQuestsJson>? BPQuestsArray))
            {
                if (BPQuestsArray.Count > 0)
                {
                    (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestGrabber(FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry, BPQuestsArray);
                }
            }


            return (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);
        }

        public class GiftClass
        {
            public string GiftBox { get; set; } = "GiftBox:gb_generic";
            public List<NotificationsItemsClassOG> GiftClassList { get; set; } = new List<NotificationsItemsClassOG>();
        }
        public static async Task<(List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry)> QuestGrabber(SeasonClass FoundSeason, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry, List<WeeklyQuestsJson> WeeklyQuestsArray)
        {
            List<GiftClass> GiftClassList = new List<GiftClass>();
            // List<NotificationsItemsClassOG> ItemClassOG = new List<NotificationsItemsClassOG>();

            List<string> AddedBundles = new List<string>();
            var ResponseId = "";

            //   bool bNewQuest = false;
            //  int NewBundleCount = 0;
            //   bool bAddMoreBundles = false;
            foreach (WeeklyQuestsJson WeeklyQuests in WeeklyQuestsArray)
            {
                if (
                    !string.IsNullOrEmpty(ResponseId) &&
                    ResponseId != $"ChallengeBundleSchedule:{WeeklyQuests.BundleSchedule}"
                    && WeeklyQuests.BundleRequired.QuestBundleID
                )
                {
                    var ItemObj = new
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
                            { "granted_bundles", AddedBundles.ToArray() }
                        },
                        quantity = 1,
                    };

                    MultiUpdates.Add(new
                    {
                        changeType = "itemAttrChanged",
                        ItemId = ResponseId,
                        attributeName = $"granted_bundles",
                        attributeValue = AddedBundles.ToArray()
                    });

                    AddedBundles.Clear();
                }


                AddedBundles.Add($"ChallengeBundle:{WeeklyQuests.BundleId}");
                ResponseId = $"ChallengeBundleSchedule:{WeeklyQuests.BundleSchedule}";

                List<string> grantedquestinstanceids = new List<string>();
                foreach (var AllBundles in WeeklyQuests.BundlesObject)
                {
                    if (AllBundles.quest_data.RequireBP)
                        if (!FoundSeason.BookPurchased)
                            continue;

                    grantedquestinstanceids.Add(AllBundles.templateId);

                    DailyQuestsData QuestData = FoundSeason.Quests.FirstOrDefault(e => e.Key == AllBundles.templateId).Value;

                    if (QuestData != null)
                    {
                        bool bShouldGrantItems = false;
                        if (FoundSeason.Quests[AllBundles.templateId].attributes.quest_state != "Claimed")
                        {
                            bool bShouldClaim = true;
                            int Index = 0;

                            foreach (DailyQuestsObjectiveStates ObjectiveState in FoundSeason.Quests[AllBundles.templateId].attributes.ObjectiveState)
                            {
                                //Logger.Warn(AllBundles.templateId + " " + ObjectiveState.Value.ToString());
                                //Logger.Warn(ObjectiveState.MaxValue.ToString());
                                if (ObjectiveState.Value != ObjectiveState.MaxValue)
                                {
                                    bShouldClaim = false;
                                }

                                Index += 1;
                            }

                            if (bShouldClaim)
                            {
                                FoundSeason.Quests[AllBundles.templateId].attributes.quest_state = "Claimed";

                                FoundSeason.BookXP += AllBundles.Rewards.BattleStars; // Should stay 0 above s10

                                MultiUpdates.Add(new
                                {
                                    changeType = "itemAttrChanged",
                                    ItemId = AllBundles.templateId,
                                    attributeName = $"quest_state",
                                    attributeValue = FoundSeason.Quests[AllBundles.templateId].attributes.quest_state
                                });

                                Console.WriteLine(!AllBundles.quest_data.ExtraQuests && !AllBundles.quest_data.Steps);

                                if (!AllBundles.quest_data.ExtraQuests && !AllBundles.quest_data.Steps)
                                {
                                    foreach (var item in FoundSeason.Quests)
                                    {
                                        if (item.Value.attributes.quest_state == "Claimed") continue; // We don't want to loop through claimed quests

                                        var Data = item.Value.attributes.ObjectiveState.FindIndex(e => e.Name.ToLower().Contains("weeklychallenges"));

                                        if (Data != -1)
                                        {
                                            FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data].Value += 1;

                                            var ObjectiveStateQuestData = FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data];

                                            var FindQuestData = WeeklyQuestsArray.FirstOrDefault(e => e.BundleId.Contains(FoundSeason.Quests[item.Key].attributes.challenge_bundle_id));
                                            if (FindQuestData != null)
                                            {
                                                if (ObjectiveStateQuestData.Value == ObjectiveStateQuestData.MaxValue)
                                                {
                                                    // sets it to claimed but we still need to find the item to give
                                                    FoundSeason.Quests[item.Key].attributes.quest_state = "Claimed";

                                                    MultiUpdates.Add(new
                                                    {
                                                        changeType = "itemAttrChanged",
                                                        ItemId = item.Key,
                                                        attributeName = $"quest_state",
                                                        attributeValue = FoundSeason.Quests[item.Key].attributes.quest_state
                                                    });

                                                    foreach (var FindItems in FindQuestData.BundlesObject)
                                                    {
                                                        if (!FindItems.quest_data.IsWeekly) continue;

                                                        foreach (var Item in FindItems.Objectives)
                                                        {
                                                            if (Item.BackendName.Contains(FoundSeason.Quests[item.Key].attributes.ObjectiveState[Data].Name.Split("completion_")[1]))
                                                            {
                                                                FoundSeason.BookXP += FindItems.Rewards.BattleStars;

                                                                foreach (WeeklyRewardsQuest Quest in FindItems.Rewards.Quest)
                                                                {
                                                                    Logger.Error(Quest.TemplateId);
                                                                }

                                                                foreach (WeeklyRewardsQuest GrantedItems in FindItems.Rewards.GrantedItems)
                                                                {
                                                                    Logger.Error(GrantedItems.TemplateId);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                bShouldGrantItems = true;

                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(FoundSeason.Quests[AllBundles.templateId].attributes.questData.SchemeData))
                            {
                                bShouldGrantItems = true;
                            }
                        }

                        if (bShouldGrantItems)
                        {
                            foreach (var GrantedItems in AllBundles.Rewards.GrantedItems)
                            {
                                Logger.Error("I WANT TO GIVE " + GrantedItems.TemplateId);
                                if (!profileCacheEntry.AccountData.athena.Items.ContainsKey(GrantedItems.TemplateId))
                                {
                                    if (!profileCacheEntry.AccountData.commoncore.Items.ContainsKey(GrantedItems.TemplateId))
                                    {
                                        Logger.Error(GrantedItems.TemplateId);
                                        //if (GrantedItems.TemplateId.Contains("RewardEventGraphPurchaseToken")){
                                        //    MultiUpdates.Add(new MultiUpdateClass
                                        //    {
                                        //        changeType = "itemAdded",
                                        //        itemId = GrantedItems.TemplateId,
                                        //        item = new AthenaItem
                                        //        {
                                        //            templateId = GrantedItems.TemplateId,
                                        //            attributes = new AthenaItemA
                                        //            {
                                        //                item_seen = false,
                                        //            },
                                        //            quantity = 1
                                        //        }
                                        //    });

                                        //    profileCacheEntry.AccountData.commoncore.Items.Add(GrantedItems.TemplateId, new CommonCoreItem
                                        //    {
                                        //        templateId = GrantedItems.TemplateId,
                                        //        attributes = new CommonCoreItemAttributes
                                        //        {
                                        //            item_seen = false,
                                        //            level = 1,
                                        //        },
                                        //        quantity = GrantedItems.Quantity,
                                        //    });
                                        //}
                                        //else
                                        if (GrantedItems.TemplateId.Contains("Token:"))
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
                                            else if (GrantedItems.TemplateId.Contains("HeroSelection"))
                                            {
                                                MultiUpdatesForCommonCore.Add(new MultiUpdateClass
                                                {
                                                    changeType = "itemAdded",
                                                    itemId = GrantedItems.TemplateId,
                                                    item = new CommonCoreItem
                                                    {
                                                        templateId = GrantedItems.TemplateId,
                                                        attributes = new CommonCoreItemAttributes
                                                        {
                                                            item_seen = false,
                                                        },
                                                        quantity = 1
                                                    }
                                                });

                                                profileCacheEntry.AccountData.commoncore.Items.Add(GrantedItems.TemplateId, new CommonCoreItem
                                                {
                                                    templateId = GrantedItems.TemplateId,
                                                    attributes = new CommonCoreItemAttributes
                                                    {
                                                        item_seen = false,
                                                        level = 1,
                                                    },
                                                    quantity = GrantedItems.Quantity,
                                                });

                                                GiftClassList.Add(new GiftClass
                                                {
                                                    GiftBox = "GiftBox:gb_paperdoll",
                                                    GiftClassList = new List<NotificationsItemsClassOG> {
                                                        new NotificationsItemsClassOG
                                                        {
                                                            itemType = GrantedItems.TemplateId,
                                                            itemGuid = GrantedItems.TemplateId,
                                                            quantity = 1
                                                        }
                                                    }
                                                });


                                            }
                                            else
                                            {
                                                Logger.Log($"{GrantedItems.TemplateId} is not supported", "ClientQuestLogin");
                                            }
                                        }
                                        else if (GrantedItems.TemplateId.Contains("HomebaseBannerIcon"))
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


                                            GiftClass existingGiftClass = GiftClassList.FirstOrDefault(g => g.GiftBox == "GiftBox:gb_generic")!;
                                            if (existingGiftClass != null)
                                            {
                                                existingGiftClass.GiftClassList.Add(new NotificationsItemsClassOG
                                                {
                                                    itemType = GrantedItems.TemplateId,
                                                    itemGuid = GrantedItems.TemplateId,
                                                    quantity = 1
                                                });
                                            }
                                            else
                                            {
                                                GiftClass newGiftClass = new GiftClass
                                                {
                                                    GiftClassList = new List<NotificationsItemsClassOG> {
                                                        new NotificationsItemsClassOG
                                                        {
                                                            itemType = GrantedItems.TemplateId,
                                                            itemGuid = GrantedItems.TemplateId,
                                                            quantity = 1
                                                        }
                                                    }
                                                };

                                                GiftClassList.Add(newGiftClass);
                                            }
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
                                        else if (GrantedItems.TemplateId.Contains("Quest:"))
                                        {
                                            // Gives User Quest Items
                                            WeeklyObjects QuestJson = WeeklyQuests.BundlesObject.Find(e => e.templateId == GrantedItems.TemplateId);

                                            if (QuestJson != null)
                                            {
                                                //bNewQuest = true;
                                                (FoundSeason, MultiUpdates) = await QuestDataHandler.Add(QuestJson, FoundSeason, WeeklyQuests, MultiUpdates);
                                            }
                                        }
                                        else if (GrantedItems.TemplateId.Contains("ChallengeBundle:"))
                                        {
                                            WeeklyQuestsJson WeeklyQuestsARR = WeeklyQuestsArray.Find(e => $"ChallengeBundle:{e.BundleId}" == GrantedItems.TemplateId)!;
                                            if (WeeklyQuestsARR != null)
                                            {
                                                List<WeeklyObjects> WeeklyQuestsBundle = WeeklyQuestsARR.BundlesObject;
                                                if (WeeklyQuestsBundle.Count > 0)
                                                {
                                                    WeeklyObjects DPData = WeeklyQuestsBundle[0];

                                                    if (DPData != null)
                                                    {
                                                        //  bNewQuest = true;
                                                        (FoundSeason, MultiUpdates) = await QuestDataHandler.Add(DPData, FoundSeason, WeeklyQuestsARR, MultiUpdates);

                                                        FoundSeason.Quests[AllBundles.templateId].attributes.questData.SchemeData = "";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                FoundSeason.Quests[AllBundles.templateId].attributes.questData.SchemeData = GrantedItems.TemplateId;

                                                //NeedScheme
                                            }
                                            // WeeklyObjects QuestJson = WeeklyQuests.BundlesObject.Find(e => e.templateId == GrantedItems.TemplateId);
                                        }

                                        //
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

                                        //TestIG.Add(new NotificationsItemsClassOG
                                        //{
                                        //    itemType = GrantedItems.TemplateId,
                                        //    itemGuid = GrantedItems.TemplateId,
                                        //    quantity = GrantedItems.Quantity,
                                        //});
                                    }
                                }
                                //var GiveItenToUser = profileCacheEntry.AccountData.athena.
                            }

                            foreach (var Quests in AllBundles.Rewards.Quest)
                            {
                                var FoundNewQuest = WeeklyQuests.BundlesObject.FirstOrDefault(e => e.templateId == Quests.TemplateId);
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
                                            challenge_bundle_id = $"ChallengeBundle:{WeeklyQuests.BundleId}",
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
                                            { "challenge_bundle_id", $"ChallengeBundle:{WeeklyQuests.BundleId}" },
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
                                        },
                                        quantity = 1
                                    };

                                    if (!WeeklyQuests.BundleRequired.QuestBundleID)
                                        ItemObjectResponse.attributes["challenge_bundle_id"] = ""; // yeah crazy right

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
                    else
                    {
                        if (AllBundles.quest_data.ExtraQuests) continue;

                        if (AllBundles.quest_data.WeekQuest > Saved.Saved.DeserializeGameConfig.WeeklyQuest) continue;

                        // Don't even own the quest wow!

                        List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();
                        //  int AmountCompleted = 0;
                        foreach (WeeklyObjectsObjectives ObjectiveItems in AllBundles.Objectives)
                        {
                            //CurrentLevelXP
                            if (ObjectiveItems.BackendName.ToLower().Contains("_xp_"))
                            {
                                if (FoundSeason.Level >= ObjectiveItems.Count)
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
                                    List<SeasonXP> SeasonXpIg = BattlepassManager.SeasonBattlePassXPItems.FirstOrDefault(e => e.Key == FoundSeason.SeasonNumber).Value;
                                    var beforeLevelXPElement = SeasonXpIg.FirstOrDefault(e => e.Level == FoundSeason.Level);
                                    if (SeasonXpIg != null && beforeLevelXPElement != null)
                                    {

                                        int CurrentLevelXP;
                                        if (SeasonXpIg.IndexOf(beforeLevelXPElement) == SeasonXpIg.Count - 1)
                                        {
                                            FoundSeason.SeasonXP = 0;
                                        }

                                        var SeasonXPItem = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (beforeLevelXPElement.XpTotal + FoundSeason.SeasonXP));
                                        if (SeasonXPItem != null)
                                        {
                                            CurrentLevelXP = SeasonXPItem.XpTotal + FoundSeason.SeasonXP;
                                            QuestObjectStats.Add(new DailyQuestsObjectiveStates
                                            {
                                                Name = $"completion_{ObjectiveItems.BackendName}",
                                                Value = CurrentLevelXP,
                                                MaxValue = ObjectiveItems.Count
                                            });
                                        }
                                    }
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

                        FoundSeason.Quests.Add($"{AllBundles.templateId}", new DailyQuestsData
                        {
                            templateId = $"{AllBundles.templateId}",
                            attributes = new DailyQuestsDataDB
                            {
                                challenge_bundle_id = $"ChallengeBundle:{WeeklyQuests.BundleId}",
                                sent_new_notification = false,
                                ObjectiveState = QuestObjectStats
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
                                { "challenge_bundle_id", $"ChallengeBundle:{WeeklyQuests.BundleId}" },
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

                        if (!WeeklyQuests.BundleRequired.QuestBundleID)
                            ItemObjectResponse.attributes["challenge_bundle_id"] = ""; // yeah crazy right

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


                //if (!FoundSeason.Quests.TryGetValue(WeeklyQuests.BundleId, out DailyQuestsData value))
                //{
                //    NewBundleCount += 1;

                //    var AthenaItemChallengeBundle = new AthenaItemDynamic
                //    {
                //        templateId = $"ChallengeBundle:{WeeklyQuests.BundleId}",
                //        attributes = new Dictionary<string, object>
                //        {
                //            { "has_unlock_by_completion", false },
                //            { "num_quests_completed", 0 }, // gotta do this as well gulp
                //            { "level", 0 },
                //            { "grantedquestinstanceids", grantedquestinstanceids.ToArray() },
                //            { "item_seen",  true },
                //            { "max_allowed_bundle_level", 0 },
                //            { "num_granted_bundle_quests", grantedquestinstanceids.Count() },
                //            { "max_level_bonus", 0 },
                //            { "challenge_bundle_schedule_id", ResponseId },
                //            { "num_progress_quests_completed", 0 },
                //            { "xp", 0 },
                //            { "favorite", false }
                //        },
                //        quantity = 1,
                //    };

                //    FoundSeason.Quests.Add(WeeklyQuests.BundleId, new DailyQuestsData
                //    {
                //        templateId = $"ChallengeBundle:{WeeklyQuests.BundleId}",
                //        attributes = new DailyQuestsDataDB
                //        {
                //            challenge_bundle_id = $"ChallengeBundle:{WeeklyQuests.BundleId}",
                //            grantedquestinstanceids = grantedquestinstanceids,
                //            item_seen = true,
                //            num_granted_bundle_quests = grantedquestinstanceids.Count(),
                //            challenge_bundle_schedule_id = ResponseId,
                //            sent_new_notification = false,

                //        },
                //        quantity = 1
                //    });

                //    MultiUpdates.Add(new MultiUpdateClass
                //    {
                //        changeType = "itemAdded",
                //        itemId = $"ChallengeBundle:{WeeklyQuests.BundleId}",
                //        item = AthenaItemChallengeBundle
                //    });
                //}
                // else
                //{
                // if(value.attributes.grantedquestinstanceids != grantedquestinstanceids)
                // {
                MultiUpdates.Add(new
                {
                    changeType = "itemAttrChanged",
                    ItemId = ResponseId,
                    attributeName = $"grantedquestinstanceids",
                    attributeValue = grantedquestinstanceids
                });
                // }
                // }
            }

            //if(NewBundleCount == AddedBundles.Count)
            //{
            //    var ItemTestObjectResponse = new
            //    {
            //        templateId = ResponseId,
            //        attributes = new Dictionary<string, object>
            //        {
            //            { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
            //            { "max_level_bonus", 0 },
            //            { "level", 0 },
            //            { "item_seen", true },
            //            { "xp", 0 },
            //            { "favorite", false },
            //            { "granted_bundles", AddedBundles.ToArray() }
            //        },
            //        quantity = 1,
            //    };


            //    MultiUpdates.Add(new MultiUpdateClass
            //    {
            //        changeType = "itemAdded",
            //        itemId = ResponseId,
            //        item = ItemTestObjectResponse
            //    });

            //}
            //else
            //{

            //   if(AddedBundles.Count > NewBundleCount)
            //  {
            MultiUpdates.Add(new
            {
                changeType = "itemAttrChanged",
                ItemId = ResponseId,
                attributeName = $"granted_bundles",
                attributeValue = AddedBundles
            });
            // }
            // }

            if (GiftClassList.Count > 0)
            {
                foreach (var gift in GiftClassList)
                {
                    var RandomOfferId = Guid.NewGuid().ToString();

                    profileCacheEntry.AccountData.commoncore.Gifts.Add(RandomOfferId, new GiftCommonCoreItem
                    {
                        templateId = gift.GiftBox,
                        attributes = new GiftCommonCoreItemAttributes
                        {
                            lootList = gift.GiftClassList
                        },
                        quantity = 1
                    });

                    MultiUpdatesForCommonCore.Add(new ApplyProfileChangesClassV2
                    {
                        changeType = "itemAdded",
                        itemId = RandomOfferId,
                        item = new
                        {
                            templateId = gift.GiftBox,
                            attributes = new
                            {
                                max_level_bonus = 0,
                                fromAccountId = "Server",
                                lootList = gift.GiftClassList
                            },
                            quantity = 1
                        }
                    });
                }


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

                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }

                    }
                }

            }



            return (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);
        }
    }
}