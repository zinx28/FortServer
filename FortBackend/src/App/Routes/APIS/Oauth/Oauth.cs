using FortBackend.src.App.Utilities.Classes.EpicResponses;
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


namespace FortBackend.src.App.Routes.APIS.Oauth
{
    [ApiController]
    [Route("account/api")]
    public class OauthApiController : ControllerBase
    {
        private IMongoDatabase _database;

        public OauthApiController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpPost("oauth/token")]
        public async Task<IActionResult> LoginToken()
        {
            Response.ContentType = "application/json";
            try
            {

                string grant_type = "";
                string DisplayName = "";
                string Email = "";
                string AccountId = "";
                string Password = "";
                string exchange_token = "";
                bool IsMyFavUserBanned = false;

                var Headers = Request.Headers;
                var FormRequest = HttpContext.Request.Form;

                try
                {
                    if (FormRequest.TryGetValue("grant_type", out var emailToken))
                    {
                        grant_type = emailToken;
                    }
                    //if (FormRequest.TryGetValue("Syphon", out var SyphonToken))
                    //{
                    //    Console.WriteLine(SyphonToken);
                    //}
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
                }
                catch { } // who cares about errors

                string clientId = "";
                try
                {
                    string AuthorizationToken = Request.Headers["Authorization"];
                    string base64String = AuthorizationToken.Substring(6);
                    byte[] base64Bytes = Convert.FromBase64String(base64String);
                    string decodedString = Encoding.UTF8.GetString(base64Bytes);
                    string[] credentials = decodedString.Split(':');

                    clientId = credentials[0];

                    if (credentials[1] == null)
                    {
                        throw new Exception("Invaild Client ID");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("OAUTH -> " + ex.Message);
                    Response.StatusCode = 400;
                    return Ok(new MainResponse
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
                            return BadRequest(new MainResponse
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

                        Console.WriteLine("Exchange Token!");
                        break;
                }

                if (IsMyFavUserBanned)
                {
                    return BadRequest(new MainResponse
                    {
                        errorCode = "errors.com.epicgames.account.account_not_active",
                        errorMessage = "You have been permanently banned from Cosmos.",
                        messageVars = new string[0],
                        numericErrorCode = -1,
                        originatingService = "any",
                        intent = "prod",
                        error_description = "You have been permanently banned from Cosmos."
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

                await Handlers.UpdateOne<FortBackend.src.App.Utilities.MongoDB.Module.Account>("accountId", AccountId, new Dictionary<string, object>
                {
                    {
                        "refreshToken", new string[] { RefreshToken }
                    },
                    {
                        "accessToken", new string[] { AccessToken }
                    }
                });

                return Ok(new OauthLong
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

            var response = new MainResponse
            {
                errorCode = "errors.com.epicgames.account.invalid_account_credentials",
                errorMessage = "Seems like there has been a error on the backend",
                messageVars = new string[0],
                numericErrorCode = 18031,
                originatingService = "any",
                intent = "prod",
                error_description = "Seems like there has been a error on the backend",
                error = "invalid_grant"
            };

            return BadRequest(response);
        }

    }
}
