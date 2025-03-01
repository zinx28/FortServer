using FortLibrary;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;

namespace FortBackend.src.App.Utilities.Helpers.QuestsManagement
{
    public class QuestClaimer
    {
        public static async Task<(List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry)> Init(List<SeasonXP> SeasonXpIg, SeasonClass FoundSeason, List<object> MultiUpdates, List<object> MultiUpdatesForCommonCore, ProfileCacheEntry profileCacheEntry)
        {
            var beforeLevelXPElement = SeasonXpIg.FirstOrDefault(e => e.Level == FoundSeason.Level);
            //
            int CurrentLevelXP;
            if (beforeLevelXPElement != null && SeasonXpIg.IndexOf(beforeLevelXPElement) == SeasonXpIg.Count - 1)
            {
                FoundSeason.SeasonXP = 0;
            }

            CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (beforeLevelXPElement.XpTotal + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;

            bool testerfnmp = false;
            foreach (var Quests in FoundSeason.Quests)
            {
                int TempIntNum = 0;

                // NEEDS TO SOME WHAT BE DYANMIC !?!?! BUT IT'S HARD WITHOUT HAVING SOME FORCED STUFF
                foreach (var objectiveState in Quests.Value.attributes.ObjectiveState)
                {
                    if (FoundSeason.Quests[Quests.Key].attributes.quest_state == "Claimed") continue; // prevents it updating/!!???!!???

                    if (objectiveState.Name.Contains("_xp_"))
                    {
                        DailyQuestsObjectiveStates ValueIsSoProper = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum];
                        if (CurrentLevelXP >= objectiveState.MaxValue)
                        {
                            FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value = ValueIsSoProper.MaxValue;
                        }
                        else
                        {
                            if (FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value != CurrentLevelXP)
                            {
                                FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value = CurrentLevelXP;
                            }

                        }

                        FoundSeason.Quests[Quests.Key].attributes.creation_time = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                        testerfnmp = true;
                        MultiUpdates.Add(new
                        {
                            changeType = "itemAttrChanged",
                            ItemId = Quests.Key,
                            attributeName = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Name,
                            attributeValue = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value
                        });
                        //DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

                    }
                    else if (objectiveState.Name.Contains("reach_bp_tier"))
                    {
                        DailyQuestsObjectiveStates ValueIsSoProper = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum];
                       
                        if (FoundSeason.BookLevel >= objectiveState.MaxValue)
                        {
                            FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value = ValueIsSoProper.MaxValue;
                        }
                        else
                        {
                            if (FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value != FoundSeason.BookLevel)
                            {
                                FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value = FoundSeason.BookLevel;
                            }

                        }

                        FoundSeason.Quests[Quests.Key].attributes.creation_time = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                        testerfnmp = true;
                        MultiUpdates.Add(new
                        {
                            changeType = "itemAttrChanged",
                            ItemId = Quests.Key,
                            attributeName = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Name,
                            attributeValue = FoundSeason.Quests[Quests.Key].attributes.ObjectiveState[TempIntNum].Value
                        });
                    }
                        
                    //else if(objectiveState.Name.Contains("weeklychallenges"))
                        //{
                        //    //  if (FoundSeason.Quests[Quests.Key].attributes.quest_state != "Claimed") continue;
                        //    WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary
                        //    FoundSeason.Quests.Where(e => e.Value.)

                        //}   

                        TempIntNum += 1;
                }

                // IF THERE IS A BETTER METHOD PLEASE PULL REQUEST OR TELL ME (or ill just look in the future)
                //if (NeedUpdating)
                //{
                //    testerfnmp = true;
                //    var AthenaItemChallengeBundle = new
                //    {
                //        templateId = Quests.Value.templateId,
                //        attributes = new Dictionary<string, object>
                //        {
                //            { "creation_time", Quests.Value.attributes.creation_time },
                //            { "level", -1 },
                //            { "item_seen", false },
                //            { "playlists", new List<object>() },
                //            { "sent_new_notification", true },
                //            { "challenge_bundle_id", Quests.Value.attributes.challenge_bundle_id },
                //            { "xp_reward_scalar", 1 },
                //            { "challenge_linked_quest_given", "" },
                //            { "quest_pool", "" },
                //            { "quest_state", "Active" },
                //            { "bucket", "" },
                //            { "last_state_change_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                //            { "challenge_linked_quest_parent", "" },
                //            { "max_level_bonus", 0 },
                //            { "xp", 0 },
                //            { "quest_rarity", "uncommon" },
                //            { "favorite", false },
                //        },
                //        quantity = 1,
                //    };

                //    foreach (DailyQuestsObjectiveStates yklist in FoundSeason.Quests[Quests.Key].attributes.ObjectiveState)
                //    {
                //        AthenaItemChallengeBundle.attributes.Add(yklist.Name, yklist.Value);
                //    }

                //    MultiUpdates.Add(new
                //    {
                //        changeType = "itemRemoved",
                //        itemId = Quests.Value.templateId,
                //    });

                //    MultiUpdates.Add(new MultiUpdateClass
                //    {
                //        changeType = "itemAdded",
                //        itemId = Quests.Value.templateId,
                //        item = AthenaItemChallengeBundle
                //    });



                //}


            }

            if (testerfnmp)
            {
                (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestsDealer.Init(FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);

            }

            return (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);
        }
    }
}
