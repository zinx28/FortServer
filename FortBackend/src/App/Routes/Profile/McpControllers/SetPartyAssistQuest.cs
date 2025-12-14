using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Profile.Quests;
using Discord;
using System.Xml.Linq;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.MongoDB.Extentions;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetPartyAssistQuest
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetPartyAssistQuestResponse Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;

                SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == Season.Season)!;
                List<object> ProfileChanges = new List<object>();

                if (FoundSeason != null && FoundSeason.SeasonNumber >= 8)
                {
                    DailyQuestsData dailyQuestsData = FoundSeason.DailyQuests.Daily_Quests.FirstOrDefault(e => e.Key == Body.questToPinAsPartyAssist).Value;
               
                    if(dailyQuestsData != null && !string.IsNullOrEmpty(dailyQuestsData.templateId))
                    {
                        FoundSeason.party_assist = Body.questToPinAsPartyAssist;
                    }
                    else
                    {
                        DailyQuestsData weeklyQuestsData = FoundSeason.Quests.FirstOrDefault(e => e.Key == Body.questToPinAsPartyAssist).Value;

                        if (weeklyQuestsData != null && !string.IsNullOrEmpty(weeklyQuestsData.templateId))
                        {
                            FoundSeason.party_assist = Body.questToPinAsPartyAssist;
                        }
                        else
                        {
                            FoundSeason.party_assist = "";
                        }
                           
                    }

                    ProfileChanges.Add(new
                    {
                        changeType = "statModified",
                        name = "party_assist_quest",
                        value = FoundSeason.party_assist
                    });
                }

                if (ProfileChanges.Count > 0)
                {
                    profileCacheEntry.LastUpdated = DateTime.Now;
                    profileCacheEntry.AccountData.athena.BumpRevisions();
                }

                if (BaseRev_G != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = ProfileChanges,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
