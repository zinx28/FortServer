using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;
using FortBackend.src.App.Routes.APIS.Profile.McpControllers.QueryResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class ClientQuestLogin
    {
        // This IS TEMP code
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                return response;
            }

            return new Mcp();
        }
    }
}
