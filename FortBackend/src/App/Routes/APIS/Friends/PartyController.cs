using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
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
            var UserData = await Handlers.FindOne<User>("accountId", accountId);

            if (UserData != "Error")
            {


                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var requestbody = await reader.ReadToEndAsync();
                    Console.WriteLine("Party " + requestbody);



                    return Ok(new
                    {

                    });
                }
            }
            /*
             * 
             *   throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                        errorMessage = $"Authentication failed for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                        messageVars = new List<string> { $"/api/game/v2/profile/{accountId}/{wildcard}/{mcp}" },
                        numericErrorCode = 1032,
                        originatingService = "any",
                        intent = "prod",
                        error_description = $"Authentication failed for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                    };
            */


            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                errorMessage = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
                messageVars = new List<string> { $"/api/v1/Fortnite/user/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
            });

        }
    }
}
