using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class SetCosmeticLockerSlot
    {
        public static async Task<Mcp> Init(string AccountId, string ProfileId, int Season, int RVN, Account AccountDataParsed, string Body)
        {
            var requestData = JsonConvert.DeserializeObject<dynamic>(Body);
            Console.WriteLine(requestData);
            if (ProfileId == "athena" || ProfileId == "profile0")
            {
                Mcp response = await AthenaResponse.Grab(AccountId, ProfileId, Season, RVN, AccountDataParsed);
                return response;
            }

            return new Mcp();
        }
    }
}
