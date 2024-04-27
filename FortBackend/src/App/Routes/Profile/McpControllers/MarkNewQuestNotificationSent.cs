using FortLibrary.EpicResponses.Profile;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortLibrary.EpicResponses.Profile.Query;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class MarkNewQuestNotificationSent
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, MarkNewQuestNotificationSentRequest Body)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
           
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;

                SeasonClass FoundSeason = profileCacheEntry.AccountData.commoncore?.Seasons.FirstOrDefault(x => x.SeasonNumber == Season.Season)!;
                List<object> MultiUpdates = new List<object>();
                List<object> ProfileChanges = new List<object>();
                if (FoundSeason != null)
                {
                    foreach(string item in Body.itemIds)
                    {
                        if (FoundSeason.DailyQuests.Daily_Quests.TryGetValue(item, out var dailyQuests))
                        {
                            FoundSeason.DailyQuests.Daily_Quests[item].attributes.sent_new_notification = true;

                            MultiUpdates.Add(new
                            {
                                changeType = "itemAttrChanged",
                                itemId = item,
                                attributeName = "sent_new_notification",
                                attributeValue = true
                            });
                        }
                    }

                    if (MultiUpdates.Count > 0)
                    {
                        profileCacheEntry.AccountData.athena.RVN += 1;
                        profileCacheEntry.AccountData.athena.CommandRevision += 1;
                    }

                    if (BaseRev != RVN)
                    {
                        Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                        ProfileChanges = test.profileChanges;
                    }
                    else
                    {
                        ProfileChanges = MultiUpdates;
                    }

                    return new Mcp()
                    {
                        profileRevision = profileCacheEntry.AccountData.athena.RVN,
                        profileId = ProfileId,
                        profileChangesBaseRevision = BaseRev,
                        profileChanges = ProfileChanges,
                        profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        responseVersion = 1
                    };
                }



              
            }
            return new Mcp();
        }
    }
}
