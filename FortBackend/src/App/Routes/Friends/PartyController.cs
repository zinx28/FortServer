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

        /*
         * 
         *  
         {
            "config":{
                "discoverability":"INVITED_ONLY",
                "join_confirmation":true,
                "joinability":"INVITE_AND_FORMER",
                "max_size":16
            },
            "join_info":
            {
                "connection":{
                    "id":"644812f9-5e5e-4fd4-a670-b306e5956fd9@prod.ol.epicgames.com/V2:Fortnite:WIN::B9CF5D384BD5B481651C10BBC694F713",
                    "meta":{
                        "urn:epic:conn:platform_s":"WIN",
                        "urn:epic:conn:type_s":"game"
                    },
                    "yield_leadership":false
                },
                "meta":{
                    "urn:epic:member:dn_s":"Femboy Ozf"
                }
            },
            "meta":{
                "urn:epic:cfg:party-type-id_s":"default",
                "urn:epic:cfg:build-id_s":"1:3:15301536",
                "urn:epic:cfg:can-join_b":"true",
                "urn:epic:cfg:join-request-action_s":"Manual",
                "urn:epic:cfg:presence-perm_s":"Noone",
                "urn:epic:cfg:invite-perm_s":"Noone",
                "urn:epic:cfg:chat-enabled_b":"true",
                "urn:epic:cfg:accepting-members_b":"false",
                "urn:epic:cfg:not-accepting-members-reason_i":"0"
            }
        }
         * */

        [HttpPost("api/v1/Fortnite/parties")]
        public async Task<IActionResult> FortniteParty()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    Console.WriteLine("TEST " + requestBody);
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
