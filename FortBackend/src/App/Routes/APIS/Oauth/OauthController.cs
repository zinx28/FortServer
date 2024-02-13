﻿using FortBackend.src.App.Utilities.Classes.EpicResponses;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;

using System.Security.Claims;
using System.Text;
using FortBackend.src.App.XMPP;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Errors;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Oauth;
using System.IdentityModel.Tokens.Jwt;


namespace FortBackend.src.App.Routes.APIS.Oauth
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
                var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];
                var accessToken = token.Replace("eg1~", "");

                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(accessToken);
                var AccountData = await Handlers.FindOne<Account>("accountId", decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value.ToString());
                var UserData = await Handlers.FindOne<User>("accountId", decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value.ToString());

                if (AccountData != "Error" || UserData != "Error")
                {
                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];
                    User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0];

                    if (AccountDataParsed != null && UserDataParsed != null)
                    {
                        if (AccountData.ToString().Contains(accessToken))
                        {
                            if (UserDataParsed.banned != true)
                            {
                                return Ok(new
                                {
                                    token = $"{token}",
                                    session_id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "jti")?.Value,
                                    token_type = "bearer",
                                    client_id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value,
                                    internal_client = true,
                                    client_service = "fortnite",
                                    account_id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value,
                                    expires_in = 28800,
                                    expires_at = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    auth_method = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "am")?.Value,
                                    display_name = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dn")?.Value,
                                    app = "fortnite",
                                    in_app_id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value,
                                    device_id = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "dvid")?.Value
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[OauthApi:Verify] -> {ex.Message}");
            }
            return Ok(new { });

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
                string exchange_token = "";
                bool IsMyFavUserBanned = false;

                if (FormRequest.TryGetValue("grant_type", out var GrantType))
                {
                    grant_type = GrantType;
                }
    
                if (FormRequest.TryGetValue("username", out var username))
                {
                    Email = username;
                }

                if (FormRequest.TryGetValue("exchange_code", out var ExchangeCode))
                {
                    exchange_token = ExchangeCode;
                }

                if (FormRequest.TryGetValue("password", out var password))
                {
                    Password = password;
                }

                string clientId = "";
                try
                {
                    string AuthorizationToken = Request.Headers["Authorization"];

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
                        messageVars = new string[0],
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
                                messageVars = new string[0],
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid"
                            });
                        }

                        var UserData1 = await Handlers.FindOne<User>("accesstoken", exchange_token);
                        if (UserData1 != "Error")
                        {
                            User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData1)?[0];

                            if (UserDataParsed != null)
                            {
                                DisplayName = UserDataParsed.Username;
                                AccountId = UserDataParsed.AccountId;
                                IsMyFavUserBanned = bool.Parse(UserDataParsed.banned.ToString() ?? "false");
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
                        messageVars = new string[0],
                        numericErrorCode = -1,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "You have been permanently banned from FortBackend."
                    });
                }

                string RefreshToken = JWT.GenerateJwtToken(new[]
                   {
                    new Claim("sub", AccountId),
                    new Claim("t", "r"),
                    new Claim("clid", clientId),
                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (1920 * 1920)).ToString()),
                    new Claim("am", grant_type),
                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                }, 24);

                string AccessToken = JWT.GenerateJwtToken(new[]
                {
                    new Claim("app", "fortnite"),
                    new Claim("sub", AccountId),
                    new Claim("mver", "false"),
                    new Claim("clid", clientId),
                    new Claim("dn", DisplayName),
                    new Claim("am", grant_type),
                    new Claim("p", Hex.GenerateRandomHexString(256)),
                    new Claim("iai", AccountId),
                    new Claim("clsvc", "fortnite"),
                    new Claim("t", "s"),
                    new Claim("ic", "true"),
                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (480 * 480)).ToString()),
                    new Claim("iat", (DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString()),
                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                }, 8);

                AccessToken AccessTokenClient = new AccessToken
                {
                    token = $"eg1~{AccessToken}",
                    accountId = AccountId, // YPP!P
                };
                RefreshToken RefreshTokenClient = new RefreshToken
                {
                    token = $"eg1~{AccessToken}",
                    accountId = AccountId, // YPP!P
                };
                //GlobalData.AccessToken.Add(AccessTokenClient);
                //GlobalData.RefreshToken.Add(RefreshTokenClient);

                await Handlers.UpdateOne<Account>("accountId", AccountId, new Dictionary<string, object>
                {
                    {
                        "refreshToken", new string[] { RefreshToken }
                    },
                    {
                        "accessToken", new string[] { AccessToken }
                    }
                });

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
                    device_id = Hex.GenerateRandomHexString(16)
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
                messageVars = new string[0],
                numericErrorCode = 18031,
                originatingService = "any",
                intent = "prod",
                error_description = "Seems like there has been a error on the backend",
                error = "invalid_grant"
            });
        }

    }
}