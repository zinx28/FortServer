using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using System.Text;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Purchases;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortBackend.src.App.Routes.Profile.McpControllers;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.XMPP.Helpers.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace FortBackend.src.App.Routes.Profile
{
    [ApiController]
    [Route("fortnite/api/game/v2/profile")]
    public class QueryProfileApiController : ControllerBase
    {

        // [HttpPost("{accountId}/{wildcard}/BulkEquipBattleRoyaleCustomization")]
        // [HttpPost("{accountId}/{wildcard}/ClientQuestLogin")] // temp 
        // [HttpPost("{accountId}/{wildcard}/QueryProfile")]
        [HttpPost("{accountId}/{wildcard}/{mcp}")]
        public async Task<ActionResult<Mcp>> McpApi(string accountId, string wildcard, string mcp)
        {
            Response.ContentType = "application/json";
            try
            {
                var RVN = int.Parse(Request.Query["rvn"].FirstOrDefault() ?? "-1");
                var ProfileID = Request.Query["profileId"].ToString() ?? "athena";
                var tokenArray = Request.Headers["Authorization"].ToString().Split("bearer ");
                var token = tokenArray.Length > 1 ? tokenArray[1] : "";

                bool FoundAccount = GlobalData.AccessToken.Any(e => e.token == token) ||
                    GlobalData.ClientToken.Any(e => e.token == token) ||
                    GlobalData.RefreshToken.Any(e => e.token == token);

                if (FoundAccount)
                {
                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);

                    var response = new Mcp();
                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                        {
                            var requestbody = await reader.ReadToEndAsync();
                            Console.WriteLine(requestbody);
                            VersionClass Season = await SeasonUserAgent(Request);
                            if (string.IsNullOrEmpty(requestbody))
                            {
                                throw new BaseError
                                {
                                    errorCode = "errors.com.epicgames.common.iforgot",
                                    errorMessage = $"No Body for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                                    messageVars = new List<string> { $"/api/game/v2/profile/{accountId}/{wildcard}/{mcp}" },
                                    numericErrorCode = 1032,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = $"No Body for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                                };
                            }
                            switch (mcp)
                            {
                                case "QueryProfile":
                                    response = await QueryProfile.Init(accountId, ProfileID, Season, RVN, profileCacheEntry);
                                    break;
                                case "ClientQuestLogin":
                                    response = await ClientQuestLogin.Init(accountId, ProfileID, Season, RVN, profileCacheEntry);
                                    break;
                                // case "SetCosmeticLockerSlot":
                                //response = await SetCosmeticLockerSlot.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<SetCosmeticLockerSlotRequest>(requestbody));
                                // break;
                                case "EquipBattleRoyaleCustomization":
                                    response = await EquipBattleRoyaleCustomization.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<EquipBattleRoyaleCustomizationRequest>(requestbody)!);
                                    break;
                                //case "PurchaseCatalogEntry":
                                //    response = await PurchaseCatalogEntry.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<PurchaseCatalogEntryRequest>(requestbody));
                                //    break;
                                //case "CopyCosmeticLoadout":
                                //    response = await CopyCosmeticLoadout.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<CopyCosmeticLoadoutResponse>(requestbody));
                                //    break;
                                default:

                                    response = new Mcp
                                    {
                                        profileRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.RVN : profileCacheEntry.AccountData.athena.RVN,
                                        profileId = ProfileID,
                                        profileChangesBaseRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.RVN : profileCacheEntry.AccountData.athena.RVN,
                                        profileCommandRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.CommandRevision : profileCacheEntry.AccountData.athena.CommandRevision,
                                        serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                                        responseVersion = 1
                                    };
                                    break;
                            }
                        }

                        return response;
                    }
                }
                else
                {
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                        errorMessage = $"Authentication failed for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                        messageVars = new List<string> { $"/api/game/v2/profile/{accountId}/{wildcard}/{mcp}" },
                        numericErrorCode = 1032,
                        originatingService = "any",
                        intent = "prod",
                        error_description = $"Authentication failed for /api/game/v2/profile/{accountId}/{wildcard}/{mcp}",
                    };
                }

            }
            catch (BaseError ex)
            {
                var jsonResult = JsonConvert.SerializeObject(BaseError.FromBaseError(ex));
                StatusCode(500);
                return new ContentResult()
                {
                    Content = jsonResult,
                    ContentType = "application/json",
                    StatusCode = 500
                };
                // return Ok(errorDetails);
            }
            catch (Exception ex)
            {
                Logger.Error($"McpController: {ex.Message}", "MCP");
            }

            return Ok(new Mcp
            {
                profileRevision = 1,
                profileId = "athena",
                profileChangesBaseRevision = 1,
                //profileChanges = new object[0],
                profileCommandRevision = 1,
                serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                responseVersion = 1
            });
        }
    }
}
