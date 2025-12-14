using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortLibrary.EpicResponses.Profile.Query;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using FortLibrary.EpicResponses.Profile.Query.Items;
using Microsoft.IdentityModel.Tokens;
using FortLibrary.EpicResponses.Profile.Quests;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary.Dynamics;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortBackend.src.App.Utilities.MongoDB.Extentions;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class FortRerollDailyQuest
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, FortRerollDailyQuestReq requestBodyy)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            if (ProfileId == "athena")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;

                List<object> MultiUpdates = new List<object>();
                List<object> NotificationsUpdates = new List<object>();

                // ill need to make quest adding its own file
                if (!string.IsNullOrEmpty(requestBodyy.questId))
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons!.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;
                    if (seasonObject != null)
                    {
                        if(seasonObject.DailyQuests.Rerolls > 0)
                        {
                            seasonObject.DailyQuests.Rerolls -= 1;

                            if(seasonObject.DailyQuests.Daily_Quests.TryGetValue(requestBodyy.questId, out DailyQuestsData value)){
                                seasonObject.DailyQuests.Daily_Quests.Remove(requestBodyy.questId);


                                if (!(seasonObject.DailyQuests.Daily_Quests.Count > 3) || seasonObject.SeasonNumber == 13 && !(seasonObject.DailyQuests.Daily_Quests.Count > 5))
                                {
                                    var DailyCount = seasonObject.SeasonNumber != 13 ? 3 - seasonObject.DailyQuests.Daily_Quests.Count : 5 - seasonObject.DailyQuests.Daily_Quests.Count;

                                    for (int i = 0; i < DailyCount; i++)
                                    {
                                        try
                                        {
                                            DailyQuestsJson dailyQuests = await DailyQuestsManager.GrabRandomQuest(seasonObject);

                                            if (!string.IsNullOrEmpty(dailyQuests.Name))
                                            {
                                                if (dailyQuests.Properties.Objectives.Count > 1)
                                                {
                                                    Logger.Error("FEATURE NOT IMPLEMENTED");
                                                }
                                                else
                                                {
                                                    DailyQuestsData dailyQuestsData = new DailyQuestsData
                                                    {
                                                        templateId = $"Quest:{dailyQuests.Name}",
                                                        attributes = new DailyQuestsDataDB
                                                        {
                                                            sent_new_notification = false,
                                                            ObjectiveState = new List<DailyQuestsObjectiveStates>
                                                            {
                                                                new DailyQuestsObjectiveStates
                                                                {
                                                                    Name = $"completion_{dailyQuests.Properties.Objectives[0].BackendName}",
                                                                    Value = 0
                                                                }
                                                            }
                                                        },
                                                        quantity = 1
                                                    };

                                                    MultiUpdates.Add(new MultiUpdateClass
                                                    {
                                                        changeType = "itemAdded",
                                                        itemId = dailyQuests.Name,
                                                        item = new
                                                        {
                                                            templateId = $"Quest:{dailyQuests.Name}",
                                                            attributes = new Dictionary<string, object>
                                                            {
                                                                { "creation_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                                { "level", -1 },
                                                                { "item_seen", false },
                                                                { "playlists", new List<object>() },
                                                                { "sent_new_notification", true },
                                                                { "challenge_bundle_id", "" },
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
                                                                { $"completion_{dailyQuests.Properties.Objectives[0].BackendName}", 0 }
                                                            },
                                                            quantity = 1
                                                        }
                                                    });

                                                    MultiUpdates.Add(new MultiUpdateClassV2
                                                    {
                                                        changeType = "statModified",
                                                        name = "quest_manager",
                                                        value = new
                                                        {
                                                            dailyLoginInterval = currentDate,
                                                            dailyQuestRerolls = 1
                                                        }
                                                    });

                                                    MultiUpdates.Add(new ApplyProfileChangesClass
                                                    {
                                                        changeType = "itemRemoved",
                                                        itemId = requestBodyy.questId
                                                    });

                                                    NotificationsUpdates.Add(new
                                                    {
                                                        type = "dailyQuestReroll",
                                                        primary = true,
                                                        newQuestId = dailyQuests.Name
                                                    });

                                                    seasonObject.DailyQuests.Daily_Quests.Add(dailyQuests.Name, dailyQuestsData);
                                                }

                                            }
                                            else
                                            {
                                                Logger.Error("GRABBED EMPTY STUFF FOR DAUKY QUESTS?", "DAILY QUESTS!");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error(ex.Message, "DAILY QUESTS!");
                                        }
                                    }
                                    // 
                                } // else how?
                            }
                        }
                    }
                }

                if (MultiUpdates.Count > 0)
                    profileCacheEntry.AccountData.athena.BumpRevisions();

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    MultiUpdates = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = MultiUpdates,
                    notifications = NotificationsUpdates,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }
            return new Mcp();
        }
    }
}
