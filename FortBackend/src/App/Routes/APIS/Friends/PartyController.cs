using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.APIS.Friends
{
    [ApiController]
    [Route("party")]
    public class PartyController : ControllerBase
    {
        [HttpGet("api/api/v1/Fortnite/user/:accountId")]
        public IActionResult FortnitePartyUser(string accountId)
        {
            //var UserData = await Handlers.FindOne<User>("accountId", accountId);

        }
    }
}
