using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.XMPP.Helpers.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FortBackend.src.App.Routes.Friends
{
    [ApiController]
    [Route("party")]
    public class PartyController : ControllerBase
    {
        [HttpGet("api/v1/Fortnite/user/{accountId}")]
        public async Task<IActionResult> FortnitePartyUser(string accountId)
        {
            try
            {
                var UserData = await Handlers.FindOne<User>("accountId", accountId);

                if (UserData != "Error")
                {

                    var CurrentParty = GlobalData.parties.Find(e => e.members.Any(a => a == accountId));

                    return Ok(new
                    {
                        current = CurrentParty != null ? new List<Parties> { CurrentParty } : new List<Parties>(),
                        pending = Array.Empty<object>(),
                        invites = Array.Empty<object>(),
                        pings = Array.Empty<object>()
                    });
                }else
                {
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
            catch (Exception ex)
            {
                Logger.Error("PartyUserController: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/user/{accountId}",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/user/{accountId}" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"Authentication failed for /api/v1/Fortnite/user/{accountId}",
            });
        }

        [HttpPost("api/v1/Fortnite/parties")]
        public async Task<IActionResult> FortniteParty()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    Console.WriteLine(requestBody);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("FortniteParty: " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.iforgot",
                errorMessage = $"NGL for /party/api/v1/Fortnite/parties",
                messageVars = new List<string> { $"/party/api/v1/Fortnite/parties" },
                numericErrorCode = 1032,
                originatingService = "party",
                intent = "prod",
                error_description = $"NGL for /party/api/v1/Fortnite/parties",
            });
        }
    }
}
