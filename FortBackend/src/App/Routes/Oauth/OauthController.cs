using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Security.Claims;
using System.Text;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Oauth;
using System.IdentityModel.Tokens.Jwt;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.XMPP_Server.Globals;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Module;


namespace FortBackend.src.App.Routes.Oauth
{
    [ApiController]
    [Route("account/api")]
    public class OauthApiController : ControllerBase
    {
        [HttpGet("oauth/verify")]
        public async Task<IActionResult> VerifyToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var tokenArray = Request.Headers["Authorization"].ToString().Split("bearer ");
                var token = tokenArray.Length > 1 ? tokenArray[1] : "";

                bool FoundAccount = false;
                if (GlobalData.AccessToken.Any(e => e.token == token))
                    FoundAccount = true;
              //  else if (GlobalData.ClientToken.Any(e => e.token == token))
                   // FoundAccount = true;
                else if (GlobalData.RefreshToken.Any(e => e.token == token))
                    FoundAccount = true;

                if (FoundAccount)
                {
                    var accessToken = token.Replace("eg1~", "");

                    var handler = new JwtSecurityTokenHandler();
                    var decodedToken = handler.ReadJwtToken(accessToken);

                    Console.WriteLine(decodedToken);
                    string[] tokenParts = decodedToken.ToString().Split('.');

                    if (tokenParts.Length == 2)
                    {
                        var payloadJson = tokenParts[1];
                        dynamic payload = JsonConvert.DeserializeObject(payloadJson)!;
                        if (payload == null)
                        {
                            return BadRequest(new { });
                        }
 
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(payload.sub.ToString());

                        if (profileCacheEntry != null)
                        {
                            if (profileCacheEntry.UserData.banned != true)
                            {
                                return Ok(new
                                {
                                    token = $"{token}",
                                    session_id = payload.jti.ToString(),
                                    token_type = "bearer",
                                    client_id = payload.clid.ToString(),
                                    internal_client = true,
                                    client_service = "fortnite",
                                    account_id = profileCacheEntry.UserData.AccountId,
                                    expires_in = 28800,
                                    expires_at = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    auth_method = payload.am.ToString(),
                                    display_name = profileCacheEntry.UserData.Username,
                                    app = "fortnite",
                                    in_app_id = payload.sub.ToString(),
                                    device_id = payload.dvid.ToString()
                                });
                            }                           
                        }
                    }
                }

                throw new BaseError
                {
                    errorCode = "errors.com.epicgames.common.authentication.authentication_failed",
                    errorMessage = $"Authentication failed for /account/api/oauth/verify",
                    messageVars = new List<string> { $"/account/api/oauth/verify" },
                    numericErrorCode = 1032,
                    originatingService = "any",
                    intent = "prod",
                    error_description = $"Authentication failed for /account/api/oauth/verify",
                };
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
                Logger.Error($"[OauthApi:Verify] -> {ex.Message}");
            }
     
            // shouldn't never call this
            return BadRequest(new { });
        }

        [HttpPost("oauth/token")]
        public async Task<IActionResult> LoginToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var Headers = Request.Headers;
                var FormRequest = HttpContext.Request.Form;

                string grant_type = "";
                string DisplayName = "";
                string Email = "";
                string AccountId = "";
                string Password = "";
                string refresh_token = "";
                string exchange_token = "";
                bool IsMyFavUserBanned = false;

                if (FormRequest.TryGetValue("grant_type", out var GrantType))
                {
                    if(!string.IsNullOrEmpty(GrantType))
                        grant_type = GrantType!;
                }

                if (FormRequest.TryGetValue("username", out var username))
                {
                    if (!string.IsNullOrEmpty(username))
                        Email = username!;
                }

                if (FormRequest.TryGetValue("exchange_code", out var ExchangeCode))
                {
                    if (!string.IsNullOrEmpty(ExchangeCode))
                        exchange_token = ExchangeCode!;
                }

                if (FormRequest.TryGetValue("refresh_code", out var Refresh_code))
                {
                    if (!string.IsNullOrEmpty(Refresh_code))
                        refresh_token = Refresh_code!;
                }

                if (FormRequest.TryGetValue("password", out var password))
                {
                    if (!string.IsNullOrEmpty(password))
                        Password = password!;
                }

                Console.WriteLine(grant_type);

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
                    Logger.Error("OAUTH -> " + ex.Message);
                    Response.StatusCode = 400;
                    return Ok(new BaseError
                    {
                        errorCode = "errors.com.epicgames.account.invalid_client",
                        errorMessage = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                        messageVars = new List<string>(),
                        numericErrorCode = 1011,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                        error = "invalid_client"
                    });
                }
               
                switch (grant_type)
                {
                    case "exchange_code":
                        if (string.IsNullOrEmpty(exchange_token))
                        {
                            return BadRequest(new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid",
                                messageVars = new List<string>(),
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid"
                            });
                        }

                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, exchange_token);
                        if (!string.IsNullOrEmpty(profileCacheEntry.UserData.AccountId))
                        {
                            DisplayName = profileCacheEntry.UserData.Username;
                            AccountId = profileCacheEntry.UserData.AccountId;
                            IsMyFavUserBanned = profileCacheEntry.UserData.banned;
                        }                       
                        
                        break;

                    case "client_credentials":
                        if (string.IsNullOrEmpty(clientId))
                        {
                            return BadRequest(new BaseError
                            {
                                errorCode = "errors.com.epicgames.account.invalid_client",
                                errorMessage = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                                messageVars = new List<string>(),
                                numericErrorCode = 1011,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "It appears that your Authorization header may be invalid or not present, please verify that you are sending the correct headers.",
                                error = "invalid_client"
                            });
                        }

                        string ClientToken = JWT.GenerateJwtToken(new[]
                        {
                            new Claim("p", Base64.GenerateRandomString(128)),
                            new Claim("clsvc", "fortnite"),
                            new Claim("t", "s"),
                            new Claim("mver", false.ToString()),
                            new Claim("clid", clientId),
                            new Claim("ic", "true"),
                            new Claim("exp", DateTimeOffset.UtcNow.AddMinutes(240).ToUnixTimeSeconds().ToString()),
                            new Claim("am", "client_credentials"),
                            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                            new Claim("jti", Hex.GenerateRandomHexString(32).ToLower()),
                        }, 24);

                        if(ClientToken != null) // this is never null BUT fixes a warning
                        {
                            GlobalData.ClientToken.Add(new TokenData
                            {
                                accountId = AccountId,
                                token = $"eg1~{ClientToken}"
                            });

                            return Ok(new
                            {
                                access_token = $"eg1~{ClientToken}",
                                expires_in = 28800,
                                expires_at = DateTimeOffset.UtcNow.AddHours(4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                token_type = "bearer",
                                client_id = clientId,
                                internal_client = true,
                                client_service = "fortnite"
                            });
                        }
                    break;

                    case "refresh_token":
                        Logger.Error("THIS IS UNFINISHED SO YOU MAY HAVE LOGIN ISSUES");
                        if (string.IsNullOrEmpty(refresh_token))
                        {
                            return BadRequest(new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Refresh token is required.",
                                messageVars = new List<string>(),
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Refresh token is required."
                            });
                        }

                        var refreshTokenIndex = GlobalData.RefreshToken.FindIndex(x => x.token == refresh_token);
                        if(refreshTokenIndex != -1)
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var decodedRefreshToken = handler.ReadJwtToken(GlobalData.RefreshToken[refreshTokenIndex].token.Replace("eg1~", ""));
             
                            if (decodedRefreshToken != null)
                            {
                                var creationDateClaim = decodedRefreshToken.Claims.FirstOrDefault(claim => claim.Type == "creation_date");

                                if (creationDateClaim != null)
                                {
                                    DateTime creationDate = DateTime.Parse(creationDateClaim.Value);
                                    int hoursToExpire = 1920; 

                                    DateTime expirationDate = creationDate.AddHours(hoursToExpire);

                                    if (expirationDate <= DateTime.UtcNow)
                                    {
                                        return BadRequest(new BaseError
                                        {
                                            errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                            errorMessage = "EXPIED",
                                            messageVars = new List<string>(),
                                            numericErrorCode = 18057,
                                            originatingService = "any",
                                            intent = "prod",
                                            error_description = "EXPIED."
                                        });
                                    }

                                    GlobalData.RefreshToken[refreshTokenIndex].creation_date = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                                }

                                return Ok(new OauthToken
                                {
                                    token_type = "bearer",
                                    account_id = AccountId,
                                    client_id = clientId,
                                    internal_client = true,
                                    client_service = "fortnite",
                                    refresh_token = $"{GlobalData.RefreshToken[refreshTokenIndex].token}",
                                    refresh_expires = 115200,
                                    refresh_expires_at = DateTimeOffset.UtcNow.AddHours(32).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    displayName = DisplayName,
                                    app = "fortnite",
                                    in_app_id = AccountId,
                                    device_id = Hex.GenerateRandomHexString(16)
                                });
                            }
                            else
                            {
                                Console.WriteLine("FAILED TO DECODE TOEK");
                                throw new Exception("Failed to decode refresh token.");
                            }
                        }
                        break;
                }

                if (IsMyFavUserBanned)
                {
                    return BadRequest(new BaseError
                    {
                        errorCode = "errors.com.epicgames.account.account_not_active",
                        errorMessage = "You have been permanently banned from FortBackend.",
                        messageVars = new List<string>(),
                        numericErrorCode = -1,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "You have been permanently banned from FortBackend."
                    });
                }
                if(string.IsNullOrEmpty(AccountId))
                {
                    return BadRequest(new BaseError
                    {
                        errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                        errorMessage = "Server Issue",
                        messageVars = new List<string>(),
                        numericErrorCode = 18057,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "Server Issue"
                    });
                }
                var DeviceID = Hex.GenerateRandomHexString(16);
                string RefreshToken = JWT.GenerateJwtToken(new[]
                   {
                    new Claim("sub", AccountId),
                    new Claim("t", "r"),
                    new Claim("dvid", DeviceID),
                    new Claim("clid", clientId),
                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1920 * 1920).ToString()),
                    new Claim("am", grant_type),
                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                }, 24);

                string AccessToken = JWT.GenerateJwtToken(new[]
                {
                    new Claim("app", "fortnite"),
                    new Claim("sub", AccountId),
                    new Claim("dvid", DeviceID),
                    new Claim("mver", "false"),
                    new Claim("clid", clientId),
                    new Claim("dn", DisplayName),
                    new Claim("am", grant_type),
                    new Claim("sec", "1"),
                    new Claim("p", Hex.GenerateRandomHexString(256)),
                    new Claim("iai", AccountId),
                    new Claim("clsvc", "fortnite"),
                    new Claim("t", "s"),
                    new Claim("ic", "true"),
                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 480 * 480).ToString()),
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                }, 8);

                TokenData AccessTokenClient = new TokenData
                {
                    token = $"eg1~{AccessToken}",
                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                    accountId = AccountId, // YPP!P
                };
                //new Claim("creation_date", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"))
                TokenData RefreshTokenClient = new TokenData
                {
                    token = $"eg1~{RefreshToken}",
                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                    accountId = AccountId, // YPP!P
                };
                GlobalData.AccessToken.Add(AccessTokenClient);
                GlobalData.RefreshToken.Add(RefreshTokenClient);

                return Ok(new OauthToken
                {
                    access_token = $"eg1~{AccessToken}",
                    expires_in = 28800,
                    expires_at = DateTimeOffset.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    token_type = "bearer",
                    account_id = AccountId,
                    client_id = clientId,
                    internal_client = true,
                    client_service = "fortnite",
                    refresh_token = $"eg1~{RefreshToken}",
                    refresh_expires = 115200,
                    refresh_expires_at = DateTimeOffset.UtcNow.AddHours(32).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    displayName = DisplayName,
                    app = "fortnite",
                    in_app_id = AccountId,
                    device_id = DeviceID
                });
            }
            catch (Exception ex)
            {
                Logger.Error("OauthToken -> " + ex.Message);
            }

            return BadRequest(new BaseError
            {
                errorCode = "errors.com.epicgames.account.invalid_account_credentials",
                errorMessage = "Seems like there has been a error on the backend",
                messageVars = new List<string>(),
                numericErrorCode = 18031,
                originatingService = "any",
                intent = "prod",
                error_description = "Seems like there has been a error on the backend",
                error = "invalid_grant"
            });
        }

    }
}
