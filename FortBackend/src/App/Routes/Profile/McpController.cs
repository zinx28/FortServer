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
                var AccountData = await Handlers.FindOne<Account>("accountId", accountId);
                if (AccountData != "Error")
                {
                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];

                    var response = new Mcp();
                    if (AccountDataParsed != null)
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
                                    response = await QueryProfile.Init(accountId, ProfileID, Season, RVN, AccountDataParsed);
                                    break;
                                case "ClientQuestLogin":
                                    response = await ClientQuestLogin.Init(accountId, ProfileID, Season, RVN, AccountDataParsed);
                                    break;
                                case "SetCosmeticLockerSlot":
                                    response = await SetCosmeticLockerSlot.Init(accountId, ProfileID, Season, RVN, AccountDataParsed, JsonConvert.DeserializeObject<SetCosmeticLockerSlotRequest>(requestbody));
                                    break;
                                case "EquipBattleRoyaleCustomization":
                                    response = await EquipBattleRoyaleCustomization.Init(accountId, ProfileID, Season, RVN, AccountDataParsed, JsonConvert.DeserializeObject<EquipBattleRoyaleCustomizationRequest>(requestbody));
                                    break;
                                case "PurchaseCatalogEntry":
                                    response = await PurchaseCatalogEntry.Init(accountId, ProfileID, Season, RVN, AccountDataParsed, JsonConvert.DeserializeObject<PurchaseCatalogEntryRequest>(requestbody));
                                    break;
                                case "CopyCosmeticLoadout":
                                    response = await CopyCosmeticLoadout.Init(accountId, ProfileID, Season, RVN, AccountDataParsed, JsonConvert.DeserializeObject<CopyCosmeticLoadoutResponse>(requestbody));
                                    break;
                                default:

                                    response = new Mcp
                                    {
                                        profileRevision = ProfileID == "common_core" || ProfileID == "common_public" ? AccountDataParsed.commoncore.RVN : AccountDataParsed.athena.RVN,
                                        profileId = ProfileID,
                                        profileChangesBaseRevision = ProfileID == "common_core" || ProfileID == "common_public" ? AccountDataParsed.commoncore.RVN : AccountDataParsed.athena.RVN,
                                        //profileChanges = /,
                                        profileCommandRevision = ProfileID == "common_core" || ProfileID == "common_public" ? AccountDataParsed.commoncore.CommandRevision : AccountDataParsed.athena.CommandRevision,
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

                //return Ok(new Mcp
                //{
                //    profileRevision = RVN,
                //    profileId = ProfileID,
                //    profileChangesBaseRevision = RVN,
                //    //profileChanges = new object[0],
                //    profileCommandRevision = RVN,
                //    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                //    responseVersion = 1
                //});
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
                Logger.Error($"McpController: {ex.Message}");
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
