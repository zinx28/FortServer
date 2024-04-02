using Amazon.Runtime.Internal.Transform;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.Dynamics;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Purchases;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Quests;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Quests;
using Newtonsoft.Json;
using System.Collections.Generic;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class ClientQuestLogin
    {
        // This IS TEMP code
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {

                var jsonData = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\default.json"));
                if (!string.IsNullOrEmpty(jsonData))
                {
                    // adds stats shocked
                    List<string> StatsGive = new List<string>()
                    {
                        "br_placetop1_pc_m0_p2",
                        "br_placetop10_pc_m0_p2",
                        "br_placetop25_pc_m0_p2",
                        "br_placetop1_pc_m0_p10",
                        "br_placetop5_pc_m0_p10",
                        "br_placetop12_pc_m0_p10",
                        "br_placetop1_pc_m0_p9",
                        "br_placetop3_pc_m0_p9",
                        "br_placetop6_pc_m0_p9"
                    };
                    //pc_m0_p2 ~ solos
                    //pc_m0_p10 ~ duos
                    //pc_m0_p9 ~ squads
                    foreach (var item in UpdateLeaderBoard.GetStatNames())
                    {
                        if (!profileCacheEntry.StatsData.stats.Keys.Contains(item))
                        {
                            profileCacheEntry.StatsData.stats.Add(item, 0);
                        }
                    }


                    // Daily Quests WIP
                  
                    if (profileCacheEntry.AccountData.commoncore.Seasons.Any(x => x.SeasonNumber == Season.Season))
                    {
                        int BaseRev = profileCacheEntry.AccountData.commoncore.RVN;
                        int BaseRev2 = profileCacheEntry.AccountData.athena.RVN;
                        SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == Season.Season)!;
                        var MultiUpdates = new List<object>();
                        string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        if(FoundSeason.DailyQuests.Interval != currentDate)
                        {
                            FoundSeason.DailyQuests.Interval = currentDate;
                            FoundSeason.DailyQuests.Rerolls = 1; // give 1 reroll everyday
                             
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

                            if (!(FoundSeason.DailyQuests.Daily_Quests.Count > 3))
                            {
                                var DailyCount = 3 - FoundSeason.DailyQuests.Daily_Quests.Count;
                               
                                for (int i = 0; i < DailyCount; i++)
                                {
                                    try
                                    {
                                        DailyQuestsJson dailyQuests = await DailyQuestsManager.GrabRandomQuest(FoundSeason);

                                        if (!string.IsNullOrEmpty(dailyQuests.Name))
                                        {
                                            //foreach(var item in dailyQuests.Properties.Objectives.)
                                            if(dailyQuests.Properties.Objectives.Count > 1)
                                            {
                                                Logger.Error("FEATURE NOT IMPLEMENTED");
                                            }else
                                            {
                                                DailyQuestsData dailyQuestsData = new DailyQuestsData
                                                {
                                                    templateId = $"Quest:{dailyQuests.Name}",
                                                    attributes = new DailyQuestsDataDB
                                                    {

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

                                                // so skunked but should wokr
                                                MultiUpdates.Add(new MultiUpdateClass
                                                {
                                                    changeType = "itemAdded",
                                                    itemId = dailyQuests.Name,
                                                    item = new
                                                    {
                                                        templateId = $"Quest:{dailyQuests.Name}",
                                                        attributes = new Dictionary<string, object>
                                                        {
                                                            { "creation_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                            { "level", -1 },
                                                            { "item_seen", false },
                                                            { "playlists", new List<object>() },
                                                            { "sent_new_notification", false },
                                                            { "challenge_bundle_id", "" },
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
                                                            { $"completion_{dailyQuests.Properties.Objectives[0].BackendName}", 0 }
                                                        },
                                                        quantity = 1
                                                    }
                                                });

                                                FoundSeason.DailyQuests.Daily_Quests.Add(dailyQuests.Name, dailyQuestsData);
                                            }
                                            
                                        }
                                        else
                                        {
                                            Logger.Error("GRABBED EMPTY STUFF FOR DAUKY QUESTS?", "DAILY QUESTS!");
                                        }
                                    }catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message, "DAILY QUESTS!");
                                    }
                                }
                                // 
                            } // else how?


                        }

                        if(MultiUpdates.Count > 0)
                        {
                            profileCacheEntry.AccountData.athena.RVN += 1;
                            profileCacheEntry.AccountData.athena.CommandRevision += 1;
                        }

                        return new Mcp
                        {
                            profileRevision = profileCacheEntry.AccountData.athena.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = BaseRev,
                            profileChanges = MultiUpdates,
                            profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                            responseVersion = 1
                        };
                    }
                    //var DailyQuests =
                    Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);

                    return response;
                    ////Console.WriteLine("fas");
                    //List<AthenaItem> contentconfig = JsonConvert.DeserializeObject<List<AthenaItem>>(jsonData); //dynamicbackgrounds.news
                    ////Console.WriteLine("GR");

                    //ProfileChange test1 = response.profileChanges[0] as ProfileChange;
                    //foreach (AthenaItem test in contentconfig)
                    //{
                    //    //Console.WriteLine("TET");
                    //    test1.Profile.items.Add(test.templateId, test);
                    //}
                    //return response;
                }
                else
                {
                    Logger.Error("ClientQuestLogin might not function well");
                }
            }

            return new Mcp();
        }
    }
}
