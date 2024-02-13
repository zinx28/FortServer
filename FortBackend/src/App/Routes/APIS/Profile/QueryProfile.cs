using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers;

namespace FortBackend.src.App.Routes.APIS.Profile
{
    [ApiController]
    [Route("fortnite/api/game/v2/profile")]
    public class QueryProfileApiController : ControllerBase
    {
        private IMongoDatabase _database;

        public QueryProfileApiController(IMongoDatabase database)
        {
            _database = database;
        }


        [HttpPost("{accountId}/{wildcard}/BulkEquipBattleRoyaleCustomization")]
        [HttpPost("{accountId}/{wildcard}/ClientQuestLogin")] // temp 
        [HttpPost("{accountId}/{wildcard}/QueryProfile")]
        public async Task<IActionResult> QueryProfile(string accountId, string wildcard)
        {
            Response.ContentType = "application/json"; // yips
            try
            {
                var RVN = Request.Query["rvn"].FirstOrDefault() ?? "-1";
                var ProfileID = Request.Query["profileId"].ToString() ?? "athena";
                var AccountData = await Handlers.FindOne<Account>("accountId", accountId);
                if (AccountData != "Error")
                {
                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];

                    if (AccountDataParsed != null)
                    {
                        Console.WriteLine(AccountDataParsed.ToString());
                        var Season = await Grabber.SeasonUserAgent(Request);

                        if (ProfileID == "athena" || ProfileID == "profile0")
                        {
                           var response = await AthenaResponses.Response.AthenaResponse(accountId, ProfileID, Season, RVN, AccountDataParsed);
                           return Ok(response);
                        }
                    }
                }

                return Ok(new
                {
                    profileRevision = RVN,
                    profileId = ProfileID,
                    profileChangesBaseRevision = RVN,
                    profileChanges = new object[0],
                    profileCommandRevision = RVN,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                });
            }
            catch (Exception ex) {
                Logger.Error($"QueryProfile: {ex.Message}");
            }

            return Ok(new
            {
                profileRevision = 1,
                profileId = "athena",
                profileChangesBaseRevision = 1,
                profileChanges = new object[0],
                profileCommandRevision = 1,
                serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                responseVersion = 1
            });
        }
    }
}
