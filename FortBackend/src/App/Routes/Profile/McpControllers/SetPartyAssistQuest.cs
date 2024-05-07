using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.Profile.Quests;
using Discord;
using System.Xml.Linq;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class SetPartyAssistQuest
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetPartyAssistQuestResponse Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
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
                        FoundSeason.party_assist = "";
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
                    profileCacheEntry.AccountData.athena.RVN += 1;
                    profileCacheEntry.AccountData.athena.CommandRevision += 1;
                }

                if (BaseRev != RVN)
                {
                    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                    ProfileChanges = test.profileChanges;
                }

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN + 1,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev + 1,
                    profileChanges = ProfileChanges,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision + 1,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }

            return new Mcp();
        }
    }
}
