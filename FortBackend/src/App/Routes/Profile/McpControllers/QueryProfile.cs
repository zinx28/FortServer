using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary.EpicResponses.Profile;
using FortLibrary;

namespace FortBackend.src.App.Routes.Profile.McpControllers
{
    public class QueryProfile
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                return response;
            }

            if (ProfileId == "common_core" || ProfileId == "common_public")
            {
                Mcp response = await CommonCoreResponse.Grab(AccountId, ProfileId, Season, RVN, profileCacheEntry);
                return response;
            }

            return new Mcp();
        }
    }
}
