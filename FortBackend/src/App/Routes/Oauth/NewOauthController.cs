using FortBackend.src.App.SERVER;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.XMPP.Data;
using FortLibrary;
using FortLibrary.Encoders;
using FortLibrary.Encoders.JWTCLASS;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.MongoDB.Module;
using FortLibrary.XMPP;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Misc;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

namespace FortBackend.src.App.Routes.Oauth
{
    //auth/v1/oauth/token

    [ApiController]
    [Route("auth/v1/oauth/token")]
    public class NewOauthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> NewToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var Headers = Request.Headers;
                var FormRequest = HttpContext.Request.Form;

                string grant_type = "";
                string nonce = "";
                string external_auth_token = "";
                if (FormRequest.TryGetValue("grant_type", out var GrantType))
                {
                    if (!string.IsNullOrEmpty(GrantType))
                        grant_type = GrantType!;
                }

                if (FormRequest.TryGetValue("nonce", out var Nonce))
                {
                    if (!string.IsNullOrEmpty(Nonce))
                        nonce = Nonce!;
                }


                if (FormRequest.TryGetValue("external_auth_token", out var External_auth_tokenexternal_auth_token))
                {
                    if (!string.IsNullOrEmpty(External_auth_tokenexternal_auth_token))
                        external_auth_token = External_auth_tokenexternal_auth_token!;
                }


                // ngl i dont even think fortnite calls this, from client_creds
                if (grant_type == "external_auth")
                {
                    var access_token_d = GlobalData.AccessToken.Find((e) => e.token == external_auth_token);
                    if(access_token_d != null)
                    {
                        ProfileCacheEntry profileCacheEntryData = await GrabData.Profile(access_token_d.accountId);

                        if (!string.IsNullOrEmpty(profileCacheEntryData.AccountId))
                        {
                            var e = JWT.GenerateJwtToken(new[]
                            {
                                new Claim("clientId", "ec684b8c687f479fadea3cb2ad83f5c6"),
                                new Claim("role", "GameClient"),
                                new Claim("productId", "prod-fn"),
                                new Claim("iss", "eos"),
                                new Claim("env", "prod"),
                                new Claim("nonce", nonce),
                                new Claim("organizationId", "o-aa83a0a9bc45e98c80c1b1c9d92e9e"),
                                new Claim("features", System.Text.Json.JsonSerializer.Serialize(new string[] {
                                    "AntiCheat",
                                    "CommerceService",
                                    "Connect",
                                    "ContentService",
                                    "Ecom",
                                    "EpicConnect",
                                    "Inventories",
                                    "LockerService",
                                    "MagpieService",
                                    "Matchmaking Service",
                                    "PCBService",
                                    "QuestService",
                                    "Stats"
                                }), JsonClaimValueTypes.JsonArray),
                                new Claim("product_user_id", access_token_d.accountId),
                                new Claim("organizationUserId", "FortServer"),
                                new Claim("deploymentId", "62a9473a2dca46b29ccf17577fcf42d7"),
                                new Claim("sandboxId", "fn"),
                                new Claim("tokenType", "userToken"),
                                new Claim("exp", "1714598878"),
                                new Claim("iat", "1714595278"),
                                new Claim("account", System.Text.Json.JsonSerializer.Serialize(new
                                {
                                    idp = "epicgames",
                                    displayName = profileCacheEntryData.UserData.Username,
                                    id = access_token_d.accountId,
                                    plf = "other"
                                }), JsonClaimValueTypes.Json),
                                new Claim("jti", "ecbe3f222a8b4f679084d2a0e6476cf5"),
                            }, 24, Saved.DeserializeConfig.JWTKEY);

                            var response = new
                            {
                                access_token = e,
                                token_type = "bearer",
                                expires_at = DateTimeOffset.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                nonce,
                                features = new string[] {
                                    "AntiCheat",
                                    "CommerceService",
                                    "Connect",
                                    "ContentService",
                                    "Ecom",
                                    "EpicConnect",
                                    "Inventories",
                                    "LockerService",
                                    "MagpieService",
                                    "Matchmaking Service",
                                    "PCBService",
                                    "QuestService",
                                    "Stats"
                                },
                                organization_id = "o-aa83a0a9bc45e98c80c1b1c9d92e9e",
                                product_id = "prod-fn",
                                sandbox_id = "fn",
                                deployment_id = "62a9473a2dca46b29ccf17577fcf42d7",
                                organization_user_id = "FortServer",
                                product_user_id = profileCacheEntryData.AccountId,
                                product_user_id_created = false,
                                id_token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY),
                                expires_in = 3599
                            };

                            return Ok(response);
                        }
                    }
                }
                else if(grant_type == "client_credentials")
                {
                    var e = JWT.GenerateJwtToken(new[]
                    {
                        new Claim("clientId", "ec684b8c687f479fadea3cb2ad83f5c6"),
                        new Claim("role", "GameClient"),
                        new Claim("productId", "prod-fn"),
                        new Claim("iss", "eos"),
                        new Claim("env", "prod"),
                        new Claim("organizationId", "o-aa83a0a9bc45e98c80c1b1c9d92e9e"),
                        new Claim("features", System.Text.Json.JsonSerializer.Serialize(new string[] {
                            "AntiCheat",
                            "CommerceService",
                            "Connect",
                            "ContentService",
                            "Ecom",
                            "EpicConnect",
                            "Inventories",
                            "LockerService",
                            "MagpieService",
                            "Matchmaking Service",
                            "PCBService",
                            "QuestService",
                            "Stats"
                        }), JsonClaimValueTypes.JsonArray),
                        new Claim("deploymentId", "62a9473a2dca46b29ccf17577fcf42d7"),
                        new Claim("sandboxId", "fn"),
                        new Claim("tokenType", "clientToken"),
                        new Claim("exp", "1714598878"),
                        new Claim("iat", "1714595278"),
                        new Claim("jti", "ecbe3f222a8b4f679084d2a0e6476cf5"),
                    }, 24, Saved.DeserializeConfig.JWTKEY);

                    var response = new
                    {
                        access_token = e,
                        token_type = "bearer",
                        expires_at = DateTimeOffset.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        features = new string[] {
                            "AntiCheat",
                            "CommerceService",
                            "Connect",
                            "ContentService",
                            "Ecom",
                            "EpicConnect",
                            "Inventories",
                            "LockerService",
                            "MagpieService",
                            "Matchmaking Service",
                            "PCBService",
                            "QuestService",
                            "Stats"
                        },
                        organization_id = "o-aa83a0a9bc45e98c80c1b1c9d92e9e",
                        product_id = "prod-fn",
                        sandbox_id = "fn",
                        deployment_id = "62a9473a2dca46b29ccf17577fcf42d7",
                        expires_in = 3599
                    };

                    return Ok(response);
                }

                return BadRequest(new
                {
                    errorCode = "errors.com.epicgames.account.invalid_account_credentials",
                    errorMessage = "Shrug",
                    messageVars = new List<string>(),
                    numericErrorCode = 18031,
                    originatingService = "any",
                    intent = "prod",
                    error_description = "Shrug",
                    error = "invalid_grant"
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { });
        }

        /**
         * [GET]: /epic/id/v2/sdk/accounts?Microsoft.AspNetCore.Http.QueryCollectionInternal
           [POST]: /epic/oauth/v2/revoke?Microsoft.AspNetCore.Http.QueryCollection

         */

        [HttpGet("/epic/id/v2/sdk/accounts")]
        public async Task<IActionResult> EpicSDKAccounts()
        {
            Response.ContentType = "application/json";
            try
            {
                string RequestQuery = Request.Query["accountId"]!;

                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(RequestQuery);
                if (!string.IsNullOrEmpty(profileCacheEntry.UserData.AccountId))
                {

                    return Ok(new List<object>()
                    {
                        new
                        {
                            accountId = RequestQuery,
                            displayName = profileCacheEntry.UserData.Username,
                            preferredLanguage = "en",
                            cabinedMode = false,
                            empty = false
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { });
        }

        [HttpPost("/epic/oauth/v2/token")]
        public async Task<IActionResult> OauthToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var Headers = Request.Headers;
                var FormRequest = HttpContext.Request.Form;

                //grant_type
                //scope
                //refresh_token
                //deployment_id

                Logger.PlainLog(FormRequest);
                string refresh_token = "";
                string grant_type = "";
                string scope = "";

                if (FormRequest.TryGetValue("grant_type", out var Grant_type))
                {
                    if (!string.IsNullOrEmpty(Grant_type))
                        grant_type = Grant_type!;
                }

                if (FormRequest.TryGetValue("refresh_token", out var Refresh_code))
                {
                    if (!string.IsNullOrEmpty(Refresh_code))
                        refresh_token = Refresh_code!;
                }

                if (FormRequest.TryGetValue("scope", out var Scope))
                {
                    if (!string.IsNullOrEmpty(Scope))
                        scope = Scope!;
                }


                string AccountId = "";
                string access_token = "";

                string clientId = "";
                try
                {
                    string AuthorizationToken = Request.Headers["Authorization"]!;

                    if (string.IsNullOrEmpty(AuthorizationToken))
                    {
                        throw new Exception("Authorization header is missing");
                    }

                    string base64String = AuthorizationToken.Substring(6);
                    byte[] base64Bytes = Convert.FromBase64String(base64String);
                    string decodedString = Encoding.UTF8.GetString(base64Bytes);
                    string[] credentials = decodedString.Split(':');

                    if (credentials.Length < 2)
                    {
                        throw new Exception("Invalid credentials format");
                    }

                    clientId = credentials[0];

                    if (string.IsNullOrEmpty(clientId))
                    {
                        throw new Exception("Invalid Client ID");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("OAUTH~NewOAtuh -> " + ex.Message);
                    Response.StatusCode = 400;
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.account.invalid_client",
                        errorMessage = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                        messageVars = new List<string>(),
                        numericErrorCode = 1011,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                        error = "invalid_client"
                    };
                }

                if (string.IsNullOrEmpty(refresh_token))
                {
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                        errorMessage = "Refresh token is required.",
                        messageVars = new List<string>(),
                        numericErrorCode = 18057,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "Refresh token is required."
                    };
                }

                var refreshTokenData = GlobalData.RefreshToken.FirstOrDefault(x => x.token == refresh_token);
                if (refreshTokenData != null && !string.IsNullOrEmpty(refreshTokenData.token))
                {
                    Logger.PlainLog("FOIUND A REFRESH TOKEN!!");
                    var accessToken = refreshTokenData.token.Replace("eg1~", "");
                    var handler = new JwtSecurityTokenHandler();
                    var DecodedToken = handler.ReadJwtToken(accessToken);
                    string[] tokenParts = DecodedToken.ToString().Split(".");

                    Logger.PlainLog(accessToken);

                    if (tokenParts.Length == 2)
                    {
                        var Payload = tokenParts[1];

                        TokenPayload tokenPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenPayload>(Payload)!;

                        if (tokenPayload != null && !string.IsNullOrEmpty(tokenPayload.Sub))
                        {
                            var expTime = DateTimeOffset.FromUnixTimeSeconds(tokenPayload.Exp.Value);
                            if (DateTimeOffset.UtcNow >= expTime)
                            {
                                throw new BaseError
                                {
                                    errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                    errorMessage = "EXPIRED.",
                                    messageVars = new List<string>(),
                                    numericErrorCode = 18057,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "EXPIRED."
                                };
                                // need to remove it from tje uiser
                            }

                            // just to verify
                            ProfileCacheEntry profileCacheEntryR = await GrabData.Profile(tokenPayload.Sub);

                            if (profileCacheEntryR != null)
                            {
                                AccountId = profileCacheEntryR.AccountId;

                                // verify if other tokens are expired <3

                                var access_token_d = GlobalData.AccessToken.Find((e) => e.accountId == AccountId);
                                if (access_token_d != null)
                                {
                                    if (JWT.VerifyTokenExpired(access_token_d.token))
                                    {
                                        var DeviceID = Hex.GenerateRandomHexString(16);

                                        string AccessToken = JWT.GenerateJwtToken(new[]
                                        {
                                            new Claim("app", "fortnite"),
                                            new Claim("sub",  AccountId),
                                            new Claim("dvid", DeviceID),
                                            new Claim("mver", "false"),
                                            new Claim("clid", clientId),
                                            new Claim("dn",  profileCacheEntryR.UserData.Username!),
                                            new Claim("am", grant_type),
                                            new Claim("sec", "1"),
                                            new Claim("p", Hex.GenerateRandomHexString(256)),
                                            new Claim("iai",  AccountId),
                                            new Claim("clsvc", "fortnite"),
                                            new Claim("t", "s"),
                                            new Claim("ic", "true"),
                                            new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 480 * 480).ToString()),
                                            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                                            new Claim("jti", Hex.GenerateRandomHexString(32)),
                                        }, 8, Saved.DeserializeConfig.JWTKEY);

                                        TokenData AccessTokenClient = new TokenData
                                        {
                                            token = $"eg1~{AccessToken}",
                                            creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                            accountId = AccountId,
                                        };

                                        GlobalData.AccessToken.Remove(access_token_d);
                                        GlobalData.AccessToken.Add(AccessTokenClient);

                                        access_token = AccessTokenClient.token;
                                    }
                                    else
                                        access_token = access_token_d.token;
                                }
                                
                            }
                        }

                        var response = new
                        {
                            scope = scope,
                            token_type = "bearer",
                            access_token = access_token,
                            refresh_token = refresh_token,
                            // i have no ieda what this is, and i dont want to implement it
                            id_token = JWT.GenerateRandomJwtToken(24, Saved.DeserializeConfig.JWTKEY),
                            expires_in = 7200,
                            expires_at = DateTimeOffset.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            refresh_expires_in = 28800,
                            refresh_expires_at = DateTimeOffset.FromUnixTimeSeconds(tokenPayload.Exp.Value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            account_id = AccountId,
                            client_id = clientId,
                            application_id = "auraaura",
                            selected_account_id = AccountId,
                            merged_accounts = new string[0]
                        };

                        return Ok(response);
                    }
                }

            }
            catch (BaseError ex)
            {
                var jsonResult = JsonConvert.SerializeObject(BaseError.FromBaseError(ex));

                StatusCode(500);
                Logger.Error("BaseError -> " + ex.Message, "NewOauthController");
                return new ContentResult()
                {
                    Content = jsonResult,
                    ContentType = "application/json",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                Logger.Error("NewOauthController -> " + ex.Message);
            }

            return Ok(new { });
        }
    }
}
