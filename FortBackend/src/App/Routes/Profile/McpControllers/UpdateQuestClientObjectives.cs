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
using FortLibrary.Shop;
using Discord;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using static System.Net.Mime.MediaTypeNames;
using FortBackend.src.App.Utilities.Helpers.QuestsManagement;
using System.Text.RegularExpressions;
using FortBackend.src.App.Utilities.MongoDB.Extentions;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class UpdateQuestClientObjectives
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, UpdateQuestClientObjectivesReq requestBodyy)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                int BaseRev_C = profileCacheEntry.AccountData.commoncore.RVN;
                List<object> MultiUpdates = new();
                List<object> MultiUpdatesForCommonCore = new();

                SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore.Seasons.FirstOrDefault(x => x.SeasonNumber == Season.Season)!;
                
                if (FoundSeason != null)
                {
                    if (requestBodyy.advance.Count > 0)
                    {
                        foreach(AdvancedCon Advanced in requestBodyy.advance)
                        {
                            if (Advanced.statName.Contains("quest_"))
                            {
                                var match = Regex.Match(Advanced.statName, @"^(.*?)(_\d+)?$"); ;
                                string QuestStatName = match.Groups[1].Value;

    
                                if (FoundSeason.Quests.TryGetValue($"Quest:{QuestStatName}", out DailyQuestsData AvgQuest))
                                {
                                    // cant work without being 1 only as why??!?!?!?!

                                    //if(AvgQuest.attributes.ObjectiveState.Count == 1)
                                    //{

                                    string numberPart = match.Groups[2].Value; 

                                    int OBStateNumber = 0;
                                    if (!string.IsNullOrEmpty(numberPart))
                                    {
                                        OBStateNumber = int.Parse(numberPart.Substring(1)); 
                                    }

                                    if (Advanced.count <= AvgQuest.attributes.ObjectiveState[OBStateNumber].MaxValue)
                                    {
                                        FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState[OBStateNumber].Value = Advanced.count;
                                     

                                        MultiUpdates.Add(new
                                        {
                                            changeType = "itemAttrChanged",
                                            ItemId = FoundSeason.Quests[$"Quest:{QuestStatName}"].templateId,
                                            attributeName = FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState[OBStateNumber].Name,
                                            attributeValue = Advanced.count
                                        });

                                        Logger.PlainLog(FoundSeason.Quests[$"Quest:{QuestStatName}"].templateId);
                                        Logger.PlainLog(FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState[OBStateNumber].Name);

                                        if (FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState.All(os => os.Value == os.MaxValue))
                                        {
                                            foreach(var e in FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState)
                                            {
                                                Logger.Error($"{e.Name} {e.Value.ToString()} {e.MaxValue.ToString()}");
                                            }
                                            //AvgQuest.attributes.quest_state = "Claimed";

                                            //MultiUpdates.Add(new
                                            //{
                                            //    changeType = "itemAttrChanged",
                                            //    ItemId = AvgQuest.templateId,
                                            //    attributeName = $"quest_state",
                                            //    attributeValue = AvgQuest.attributes.quest_state
                                            //});

                                            (MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry) = await QuestsDealer.Init(FoundSeason, MultiUpdates, MultiUpdatesForCommonCore, profileCacheEntry);
                                        }
                                        else
                                        {
                                            //var t = FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.ObjectiveState[OBStateNumber];
                                            //if (t.Value == t.MaxValue)
                                            //{
                                            //    FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.num_quests_completed += 1;

                                            //    MultiUpdates.Add(new
                                            //    {
                                            //        changeType = "itemAttrChanged",
                                            //        ItemId = FoundSeason.Quests[$"Quest:{QuestStatName}"].templateId,
                                            //        attributeName = "num_quests_completed",
                                            //        attributeValue = FoundSeason.Quests[$"Quest:{QuestStatName}"].attributes.num_quests_completed
                                            //    });
                                            //}
                                  
                                        }
                                    }

                                    //}
                                   // if(Advanced.count <= AvgQuest.attributes.ObjectiveState[])
                                    //FoundSeason.Quests[$"Quest:{Advanced.statName}"].attributes.
                             
                                }
                            }
                            else
                            {
                                // NOT ADDED YET
                                Logger.Error($"UpdateQuestClientObject doesnt have {Advanced.statName}");
                            }
                        }
                        
                    }
                }       

                if (MultiUpdates.Count > 0)
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                
                List<object> MultiCommonCoreUpdate = new List<object>();
                if (MultiUpdatesForCommonCore.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.commoncore.BumpRevisions();

                    MultiCommonCoreUpdate.Add(new
                    {
                        profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                        profileId = "common_core",
                        profileChangesBaseRevision = BaseRev_C,
                        profileChanges = MultiUpdatesForCommonCore,
                        profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                    });
                }

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
                    multiUpdate = MultiCommonCoreUpdate,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };

            }
            return new Mcp();
        }
    }
}
