using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class CopyCosmeticLoadout
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed)
        {
            if (ProfileId == "athena" || ProfileId == "profile0")
            {

            }

            return new Mcp();
        }
    }
}
