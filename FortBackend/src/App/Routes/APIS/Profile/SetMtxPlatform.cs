using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.APIS.Profile
{
    [ApiController]
    [Route("fortnite/api/game/v2/profile")]
    public class SetMtxPlatformController : ControllerBase
    {
        [HttpPost("{accountId}/{wildcard}/SetMtxPlatform")]
        public IActionResult SetMtxProfile(string accountId, string wildcard)
        {
            try
            {
                Response.ContentType = "application/json";
                var rvn = Request.Query["rvn"].FirstOrDefault() ?? "0";
                var profileId = Request.Query["profileId"];
                return Ok(new
                {
                    profileRevision = int.Parse(rvn.ToString() ?? "0") + 1,
                    profileId = profileId.ToString(),
                    profileChangesBaseRevision = 1,
                    profileChanges = new object[0],
                    profileCommandRevision = 1,
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    responseVersion = 1
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    profileRevision = 1,
                    profileId = "common_core",
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
