using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Routes.APIS.Profile.McpControllers.AthenaResponses;

namespace FortBackend.src.App.Routes.APIS.Profile.McpControllers
{
    public class QueryProfile
    {
        public static async Task<Mcp> QueryProfileHelper(string AccountId, string ProfileId, int Season, int RVN, Account AccountDataParsed)
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
