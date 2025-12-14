using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.MongoDB.Extentions;
using FortLibrary;
using FortLibrary.EpicResponses.Profile;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    // on everything i wouldnt add this if fortnite didnt try to force it ;(
    public class SetRandomCosmeticLoadoutFlag
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry, SetRandomCosmeticLoadoutFlagReq Body)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                int BaseRev_G = profileCacheEntry.AccountData.athena.GetBaseRevision(Season.Season);
                int BaseRev = profileCacheEntry.AccountData.athena.RVN;
                List<object> ProfileChanges = new List<object>();
                //"random" : false
                // this makes no sense, use_random_loadout isnt used for season 2
                // videos that show it is legit from 12.20 that didnt have the random preset option

                // enabled might break it anyways
                profileCacheEntry.AccountData.athena.random_loadout = Body.random;

                ProfileChanges.Add(new
                {
                    changeType = "statModified",
                    name = "use_random_loadout",
                    value = Body.random
                });

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
