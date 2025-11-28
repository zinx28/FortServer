using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.MongoDB.Module;
using FortLibrary;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class MarkItemSeen
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, MarkNewQuestNotificationSentRequest Body)
        {
            string currentDate = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;

               
                List<object> MultiUpdates = new List<object>();
                List<object> ProfileChanges = new List<object>();
                
                foreach (string item in Body.itemIds)
                {
                    
                    if(profileCacheEntry.AccountData.athena.Items.ContainsKey(item))
                    {
                        profileCacheEntry.AccountData.athena.Items[item].attributes.item_seen = true;
                    }
                    else if (profileCacheEntry.AccountData.commoncore.Items.ContainsKey(item))
                    {
                        profileCacheEntry.AccountData.commoncore.Items[item].attributes.item_seen = true;
                    }
                    else
                    {

                        SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                        // Wow MartItemSeen Works On Quests
                        if (seasonObject != null)
                        {
                            if (seasonObject.Quests.ContainsKey(item))
                            {
                                seasonObject.Quests[item].attributes.item_seen = true;
                            }
                            else if (seasonObject.DailyQuests.Daily_Quests.ContainsKey(item))
                            {
                                seasonObject.DailyQuests.Daily_Quests[item].attributes.item_seen = true;
                            }
                            else if (seasonObject.special_items.ContainsKey(item))
                            {
                                seasonObject.special_items[item].attributes.item_seen = true;
                            }
                        }

                    }

                    MultiUpdates.Add(new
                    {
                        itemId = item,
                        attributeName = "item_seen",
                        attributeValue = true
                    });
                }

                if (MultiUpdates.Count > 0)
                {
                    profileCacheEntry.AccountData.athena.RVN += 1;
                    profileCacheEntry.AccountData.athena.CommandRevision += 1;
                }

                //if (BaseRev != RVN)
                //{
                //    Mcp test = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                //    ProfileChanges = test.profileChanges;
                //}
                //else
                //{
                //    ProfileChanges = MultiUpdates;
                //}

                return new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.athena.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = BaseRev,
                    profileChanges = MultiUpdates,
                    profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                };
            }
            return new Mcp();
        }
    }
}
