using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FortBackend.src.App.Routes.APIS.Friends
{
    [ApiController]
    [Route("party")]
    public class PartyController : ControllerBase
    {
        [HttpGet("api/v1/Fortnite/user/{accountId}")]
        public async Task<IActionResult> FortnitePartyUser(string accountId)
        {
            //var UserData = await Handlers.FindOne<User>("accountId", accountId);
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var requestbody = await reader.ReadToEndAsync();
                Console.WriteLine("Party " + requestbody);

            }

            return Ok(new { });
        }
    }
}
