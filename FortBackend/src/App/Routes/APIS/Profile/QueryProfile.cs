using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
            catch (Exception ex) {
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
}
