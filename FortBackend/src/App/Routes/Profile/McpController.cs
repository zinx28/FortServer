using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FortBackend.src.App.Utilities;
using Newtonsoft.Json;
using System.Text;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortBackend.src.App.Routes.Profile.McpControllers;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Purchases;
using FortLibrary;
using FortBackend.src.XMPP.Data;
using FortLibrary.Encoders.JWTCLASS;

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
        [AuthorizeToken]
        public async Task<ActionResult<Mcp>> McpApi(string accountId, string wildcard, string mcp)
        {
            Response.ContentType = "application/json";
            try
            {
                var RVN = int.Parse(Request.Query["rvn"].FirstOrDefault() ?? "-1");
                var ProfileID = Request.Query["profileId"].ToString() ?? "athena";

                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    var requestbody = await reader.ReadToEndAsync();
                    Console.WriteLine(requestbody);
                    Console.WriteLine(mcp);
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
                    var response = new Mcp();
                    if (wildcard == "dedicated_server")
                    {
                        // we check from the cached data... it will no matter try to grab the data if the account id is valid
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);

                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {

                            switch (mcp)
                            {
                                case "QueryProfile":
                                    response = await QueryProfile.Init(accountId, ProfileID, Season, RVN, profileCacheEntry);
                                    break;

                                default:
                                    Logger.Error("MISSING MCP REQUEST FOR DEDICATED SERVER -> " + mcp);
                                    response = new Mcp
                                    {
                                        profileRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.RVN : profileCacheEntry.AccountData.athena.RVN,
                                        profileId = ProfileID,
                                        profileChangesBaseRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.RVN : profileCacheEntry.AccountData.athena.RVN,
                                        profileCommandRevision = ProfileID == "common_core" || ProfileID == "common_public" ? profileCacheEntry.AccountData.commoncore.CommandRevision : profileCacheEntry.AccountData.athena.CommandRevision,
                                        serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        responseVersion = 1
                                    };
                                    break;
                            }

                            return response;
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
                    else
                    {
                        var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;

                        if (string.IsNullOrEmpty(tokenPayload?.Sec))
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

                        var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;

                        if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId) && profileCacheEntry.AccountId == accountId)
                        {


                            switch (mcp)
                            {
                                case "QueryProfile":
                                   response = await QueryProfile.Init(accountId, ProfileID, Season, RVN, profileCacheEntry);
                                break;
                                case "ClientQuestLogin":
                                    response = await ClientQuestLogin.Init(accountId, ProfileID, Season, RVN, profileCacheEntry);
                                    break;
                                case "SetCosmeticLockerSlot":
                                    response = await SetCosmeticLockerSlot.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<SetCosmeticLockerSlotRequest>(requestbody)!);
                                    break;
                                case "MarkNewQuestNotificationSent":
                                    response = await MarkNewQuestNotificationSent.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<MarkNewQuestNotificationSentRequest>(requestbody)!);
                                    break;
                                case "MarkItemSeen":
                                    response = await MarkItemSeen.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<MarkNewQuestNotificationSentRequest>(requestbody)!);
                                    break;
                                case "EquipBattleRoyaleCustomization":
                                    response = await EquipBattleRoyaleCustomization.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<EquipBattleRoyaleCustomizationRequest>(requestbody)!);
                                    break;
                                case "SetPartyAssistQuest":
                                    response = await SetPartyAssistQuest.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<SetPartyAssistQuestResponse>(requestbody)!);
                                    break;
                                case "SetBattleRoyaleBanner":
                                    response = await SetBattleRoyaleBanner.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<SetBattleRoyaleBannerReq>(requestbody)!);
                                    break;
                                case "PurchaseCatalogEntry":
                                    response = await PurchaseCatalogEntry.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<PurchaseCatalogEntryRequest>(requestbody)!);
                                    break;
                                case "RemoveGiftBox":
                                    response = await RemoveGiftBox.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<RemoveGiftBoxReq>(requestbody)!);
                                    break;
                                case "FortRerollDailyQuest":
                                    response = await FortRerollDailyQuest.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<FortRerollDailyQuestReq>(requestbody)!);
                                    break;
                                case "UpdateQuestClientObjectives":
                                    response = await UpdateQuestClientObjectives.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<UpdateQuestClientObjectivesReq>(requestbody)!);
                                    break;
                                case "BulkEquipBattleRoyaleCustomization":
                                    response = await BulkEquipBattleRoyaleCustomization.Init(accountId, ProfileID, Season, RVN, profileCacheEntry, JsonConvert.DeserializeObject<BulkEquipBattleRoyaleCustomizationResponse>(requestbody)!);
                                    break;
                                // not proper
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
                                            serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            responseVersion = 1
                                        };
                                        break;
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

                        return response;
                    }
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
                serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                responseVersion = 1
            });
        }
    }
}
