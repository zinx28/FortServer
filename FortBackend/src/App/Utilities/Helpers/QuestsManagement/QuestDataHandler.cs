using FortBackend.src.App.Utilities.Helpers.BattlepassManagement;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary.MongoDB.Module;

namespace FortBackend.src.App.Utilities.Helpers.QuestsManagement
{
    public class QuestDataHandler
    {
        public static async Task<(SeasonClass FoundSeason, List<object> MultiUpdates)> Add(WeeklyObjects QuestJson, SeasonClass FoundSeason, WeeklyQuestsJson WeeklyQuests, List<object> MultiUpdates)
        {
            List<DailyQuestsObjectiveStates> QuestObjectStats = new List<DailyQuestsObjectiveStates>();
            //  int AmountCompleted = 0;
            foreach (WeeklyObjectsObjectives ObjectiveItems in QuestJson.Objectives)
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

                        int CurrentLevelXP;
                        if (beforeLevelXPElement != null && SeasonXpIg.IndexOf(beforeLevelXPElement) == SeasonXpIg.Count - 1)
                        {
                            FoundSeason.SeasonXP = 0;
                        }

                        CurrentLevelXP = SeasonXpIg.FirstOrDefault(e => e.XpTotal >= (beforeLevelXPElement.XpTotal + FoundSeason.SeasonXP)).XpTotal + FoundSeason.SeasonXP;
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

            FoundSeason.Quests.Add($"{QuestJson.templateId}", new DailyQuestsData
            {
                templateId = $"{QuestJson.templateId}",
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
                templateId = $"{QuestJson.templateId}",
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

            foreach (DailyQuestsObjectiveStates ObjectState in QuestObjectStats)
            {
                ItemObjectResponse.attributes.Add(ObjectState.Name, ObjectState.Value);
            }

            MultiUpdates.Add(new MultiUpdateClass
            {
                changeType = "itemAdded",
                itemId = $"{QuestJson.templateId}",
                item = ItemObjectResponse
            });

            return (FoundSeason, MultiUpdates);
        }
    }
}
