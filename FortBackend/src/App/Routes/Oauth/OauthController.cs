using FortBackend.src.App.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using FortLibrary.Encoders;
using System.IdentityModel.Tokens.Jwt;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.EpicResponses.Oauth;
using FortLibrary;
using Microsoft.IdentityModel.Tokens;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using FortLibrary.MongoDB.Module;
using FortLibrary.Encoders.JWTCLASS;


namespace FortBackend.src.App.Routes.Oauth
{
    [ApiController]
    [Route("account/api")]
    public class OauthApiController : ControllerBase
    {

        [HttpGet("oauth/websocket/addnew/token")]
        public async Task<IActionResult> NewToken()
        {
            try
            {
                string accessToken = Request.Headers["Authorization"]!;
                string refreshToken = Request.Headers["RefreshToken"]!;

                Console.WriteLine(accessToken);
                Console.WriteLine(refreshToken);

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var decodedToken = handler.ReadJwtToken(accessToken);
                    string[] tokenParts = decodedToken.ToString().Split('.');

                    if (tokenParts.Length == 2)
                    {
                        var payloadJson = tokenParts[1];
                        dynamic payload = JsonConvert.DeserializeObject(payloadJson)!;
                        if (payload == null)
                        {
                            return BadRequest(new { });
                        }

                        // wrong type of token
                        if (string.IsNullOrEmpty(payload.sub.ToString()))
                        {
                            return BadRequest(new { });
                        }

                        // check the account
                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(payload.sub.ToString());
                        Console.WriteLine(profileCacheEntry.AccountId);
                        if (!string.IsNullOrEmpty(profileCacheEntry.AccountId) && profileCacheEntry.UserData.banned != true)
                        {
                            var FindAccount = GlobalData.AccessToken.FirstOrDefault(e => e.accountId == profileCacheEntry.AccountId);
                            if (FindAccount != null)
                            {
                                GlobalData.AccessToken.Remove(FindAccount);
                                GlobalData.AccessToken.Add(new TokenData
                                {
                                    token = $"eg1~{accessToken}",
                                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                    accountId = FindAccount.accountId,
                                });
                            }
                            else
                            {
                                GlobalData.AccessToken.Add(new TokenData
                                {
                                    token = $"eg1~{accessToken}",
                                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                    accountId = payload.sub,
                                });
                            }

                            var RefreshAccount = GlobalData.RefreshToken.FirstOrDefault(e => e.accountId == profileCacheEntry.AccountId);
                            if (RefreshAccount != null)
                            {
                                GlobalData.RefreshToken.Remove(RefreshAccount);
                                GlobalData.RefreshToken.Add(new TokenData
                                {
                                    token = $"eg1~{accessToken}",
                                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                    accountId = RefreshAccount.accountId,
                                });
                            }
                            else
                            {
                                GlobalData.RefreshToken.Add(new TokenData
                                {
                                    token = $"eg1~{accessToken}",
                                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                                    accountId = payload.sub,
                                });
                            }
                        }

                    }
                }
                return Ok(new { });
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CUSTOM~OAUTH");
            }
            return BadRequest(new { });
        }

        [HttpGet("oauth/verify")]
        [AuthorizeToken]
        public async Task<IActionResult> VerifyToken()
        {
            Response.ContentType = "application/json";
            try
            {
                var profileCacheEntry = HttpContext.Items["ProfileData"] as ProfileCacheEntry;
                if (profileCacheEntry != null)
                {
                    if (profileCacheEntry.UserData.banned != true)
                    {
                        var tokenPayload = HttpContext.Items["Payload"] as TokenPayload;

                        return Ok(new
                        {
                            token = HttpContext.Items["Token"] as string,
                            session_id = tokenPayload?.Jti,
                            token_type = "bearer",
                            client_id = tokenPayload?.Clid,
                            internal_client = true,
                            client_service = "fortnite",
                            account_id = profileCacheEntry.UserData.AccountId,
                            expires_in = 28800,
                            expires_at = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            auth_method = tokenPayload?.Am,
                            display_name = profileCacheEntry.UserData.Username,
                            app = "fortnite",
                            in_app_id = tokenPayload?.Sub,
                            device_id = tokenPayload?.Dvid
                        });
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

        [HttpGet("/test/{token}")]
        public ActionResult Test(string token)
        {
          
             var AccessToken = token.Replace("eg1~", "");
              
             var handler = new JwtSecurityTokenHandler();
                
             var decodedToken = handler.ReadJwtToken(AccessToken);

            var claimsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(decodedToken.Claims.ToDictionary(c => c.Type, c => c.Value)));

            if (decodedToken.ToString() != null)
            {
                //dynamic payload = JsonConvert.DeserializeObject(tokenParts[1].ToString())!;
                if (claimsDictionary == null)
                {
                    return BadRequest(new { });
                }

                if (!string.IsNullOrEmpty(claimsDictionary["dvid"].ToString()))
                {

                //GlobalData.RefreshToken.Add(RefreshTokenClient);

                    return Ok(new OauthToken
                    {
                        access_token = $"{token}",
                        expires_in = 28800,
                        expires_at = DateTimeOffset.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        token_type = "bearer",
                        account_id = "test",
                        client_id = "test",
                        internal_client = true,
                        client_service = "fortnite",
                        refresh_token = $"test",
                        refresh_expires = 115200,
                        refresh_expires_at = DateTimeOffset.UtcNow.AddHours(32).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        displayName = "test",
                        app = "fortnite",
                        in_app_id = "test",
                        device_id = claimsDictionary["dvid"]
                    });
                }

            }
            else
            {
                Console.WriteLine("FAKE TOKEN?"); // never should happen
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

                string token_type = "";
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
                    if (!string.IsNullOrEmpty(GrantType))
                        grant_type = GrantType!;
                }

                //eg1
                if (FormRequest.TryGetValue("token_type", out var TokenType))
                {
                    if (!string.IsNullOrEmpty(TokenType))
                    {
                        token_type = TokenType!;
                        Console.WriteLine(token_type);
                    }

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

                if (FormRequest.TryGetValue("refresh_token", out var Refresh_code))
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

                switch (grant_type)
                {
                    case "exchange_code":
                        if (string.IsNullOrEmpty(exchange_token))
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid",
                                messageVars = new List<string>(),
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid"
                            };
                        }

                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile("", true, exchange_token);
                        if (!string.IsNullOrEmpty(profileCacheEntry.UserData.AccountId))
                        {
                            Logger.Error("FOUND ACCOUNT " + profileCacheEntry.UserData.Username);
                            DisplayName = profileCacheEntry.UserData.Username;
                            AccountId = profileCacheEntry.UserData.AccountId;
                            IsMyFavUserBanned = profileCacheEntry.UserData.banned;
                        }
                        else
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid",
                                messageVars = new List<string>(),
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Sorry the exchange code you supplied was not found. It is possible that it was no longer valid"
                            };
                        }

                        break;

                    case "password":
                      
                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Password is required",
                                messageVars = new List<string>(),
                                numericErrorCode = 1013,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Password is required"
                            };
                        }

                        if(username.ToString().Contains("@"))
                        {
                            ProfileCacheEntry profileCacheEntryData = await GrabData.ProfileEmail(username.ToString());

                            if (!string.IsNullOrEmpty(profileCacheEntryData.AccountId))
                            {
                                if(CryptoGen.VerifyPassword(Password, profileCacheEntryData.UserData.Password))
                                {
                                   // if (profileCacheEntryData.UserData.Password == password) // this auto generated by the bakcend
                                    //{
                                        DisplayName = profileCacheEntryData.UserData.Username;
                                        AccountId = profileCacheEntryData.AccountId;
                                        IsMyFavUserBanned = profileCacheEntryData.UserData.banned;
                                   // }
                                }
                              
                            }
                            else
                            {
                                throw new BaseError
                                {
                                    errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                    errorMessage = "Your e-mail and/or password are incorrect. Please check them and try again.",
                                    messageVars = new List<string>(),
                                    numericErrorCode = 18057,
                                    originatingService = "any",
                                    intent = "prod",
                                    error_description = "Your e-mail and/or password are incorrect. Please check them and try again."
                                };
                            }
                        }
                        else
                        {
                            throw new BaseError
                            {
                                errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                                errorMessage = "Your e-mail and/or password are incorrect. Please check them and try again.",
                                messageVars = new List<string>(),
                                numericErrorCode = 18057,
                                originatingService = "any",
                                intent = "prod",
                                error_description = "Your e-mail and/or password are incorrect. Please check them and try again."
                            };
                        }

                      

                        break;

                    case "client_credentials":

                        if (string.IsNullOrEmpty(clientId))
                        {
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

                        string ClientToken = JWT.GenerateJwtToken(new[]
                        {
                            new Claim("p", Base64.GenerateRandomString(128)),
                            new Claim("clsvc", "fortnite"),
                            new Claim("t", "s"),
                            new Claim("mver", "false"),
                            new Claim("clid", clientId),
                            new Claim("ic", "true"),
                            new Claim("exp", DateTimeOffset.UtcNow.AddMinutes(240).ToUnixTimeSeconds().ToString()),
                            new Claim("am", grant_type),
                            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                            new Claim("jti", Hex.GenerateRandomHexString(32).ToLower()),
                        }, 24, Saved.DeserializeConfig.JWTKEY);



                        if (ClientToken != null) // this is never null BUT fixes a warning
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
                        
                        break;

                    case "refresh_token":
                        Logger.Error("THIS IS UNFINISHED SO YOU MAY HAVE LOGIN ISSUES");
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


                        Logger.Error(refresh_token);

                        // REFRESH TOKEN SHOULD HAVE, AccountId, DeviceId and Secret

                        var refreshTokenData = GlobalData.RefreshToken.FirstOrDefault(x => x.token == refresh_token);
                        if (refreshTokenData != null && !string.IsNullOrEmpty(refreshTokenData.token))
                        {
                            Logger.Log("FOIUND A REFRESH TOKEN!!");
                            var accessToken = refreshTokenData.token.Replace("eg1~", "");
                            var handler = new JwtSecurityTokenHandler();
                            var DecodedToken = handler.ReadJwtToken(accessToken);
                            string[] tokenParts = DecodedToken.ToString().Split(".");

                            if (tokenParts.Length == 2)
                            {
                                var Payload = tokenParts[1];

                                TokenPayload tokenPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenPayload>(Payload)!;

                                if (tokenPayload != null && !string.IsNullOrEmpty(tokenPayload.Sub))
                                {
                                    // verifys the real profile so fake profiles cant do stuff

                                    //TODO verify token + increase time                                     
                                    ProfileCacheEntry profileCacheEntryR = await GrabData.Profile(tokenPayload.Sub);

                                    if (profileCacheEntryR != null)
                                    {
                                        IsMyFavUserBanned = profileCacheEntryR.UserData.banned;
                                        AccountId = profileCacheEntryR.UserData.AccountId;
                                        DisplayName = profileCacheEntryR.UserData.Username;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //todo get real error
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

                        // else failed to find...

                        //    var handler = new JwtSecurityTokenHandler();
                        //    var decodedRefreshToken = handler.ReadJwtToken(GlobalData.RefreshToken[refreshTokenIndex].token.Replace("eg1~", ""));

                        //    if (decodedRefreshToken != null)
                        //    {
                        //        var creationDateClaim = decodedRefreshToken.Claims.FirstOrDefault(claim => claim.Type == "creation_date");

                        //        if (creationDateClaim != null)
                        //        {
                        //            DateTime creationDate = DateTime.Parse(creationDateClaim.Value);
                        //            int hoursToExpire = 1920;

                        //            DateTime expirationDate = creationDate.AddHours(hoursToExpire);

                        //            if (expirationDate <= DateTime.UtcNow)
                        //            {
                        //                return BadRequest(new BaseError
                        //                {
                        //                    errorCode = "errors.com.epicgames.common.oauth.invalid_request",
                        //                    errorMessage = "EXPIED",
                        //                    messageVars = new List<string>(),
                        //                    numericErrorCode = 18057,
                        //                    originatingService = "any",
                        //                    intent = "prod",
                        //                    error_description = "EXPIED."
                        //                });
                        //            }

                        //            GlobalData.RefreshToken[refreshTokenIndex].creation_date = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
                        //        }

                        //        return Ok(new OauthToken
                        //        {
                        //            token_type = "bearer",
                        //            account_id = AccountId,
                        //            client_id = clientId,
                        //            internal_client = true,
                        //            client_service = "fortnite",
                        //            refresh_token = $"{GlobalData.RefreshToken[refreshTokenIndex].token}",
                        //            refresh_expires = 115200,
                        //            refresh_expires_at = DateTimeOffset.UtcNow.AddHours(32).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        //            displayName = DisplayName,
                        //            app = "fortnite",
                        //            in_app_id = AccountId,
                        //            device_id = Hex.GenerateRandomHexString(16)
                        //        });
                        //    }
                        //    else
                        //    {
                        //        Console.WriteLine("FAILED TO DECODE TOEK");
                        //        throw new Exception("Failed to decode refresh token.");
                        //    }
                        //}
                        break;
                }

                if (IsMyFavUserBanned)
                {
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.account.account_not_active",
                        errorMessage = $"You have been permanently banned from {Saved.DeserializeConfig.DiscordBotMessage}.", // why not use this
                        messageVars = new List<string>(),
                        numericErrorCode = -1,
                        originatingService = "any",
                        intent = "prod",
                        error_description = $"You have been permanently banned from {Saved.DeserializeConfig.DiscordBotMessage}."
                    };
                }

                if (string.IsNullOrEmpty(AccountId))
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

                if (string.IsNullOrEmpty(DisplayName))
                    return Ok(new { });

                var DeviceID = Hex.GenerateRandomHexString(16);

                // WE WILL GENERATE A exchange_code TOKEN
                string RefreshToken = JWT.GenerateJwtToken(new[]
                {
                    new Claim("sub", AccountId),
                    new Claim("t", "r"),
                    new Claim("dvid", DeviceID),
                    new Claim("clid", clientId),
                    new Claim("exp", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1920 * 1920).ToString()),
                    new Claim("am", grant_type),
                    new Claim("jti", Hex.GenerateRandomHexString(32)),
                }, 24, Saved.DeserializeConfig.JWTKEY);
        
                string AccessToken = JWT.GenerateJwtToken(new[]
                {
                    new Claim("app", "fortnite"),
                    new Claim("sub",  AccountId),
                    new Claim("dvid", DeviceID),
                    new Claim("mver", "false"),
                    new Claim("clid", clientId),
                    new Claim("dn",  DisplayName!),
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
                
                TokenData RefreshTokenClient = new TokenData
                {
                    token = $"eg1~{RefreshToken}",
                    creation_date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"),
                    accountId = AccountId,
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
            catch (BaseError ex)
            {
                var jsonResult = JsonConvert.SerializeObject(BaseError.FromBaseError(ex));

                StatusCode(500);
                Logger.Error("BaseError -> " + ex.Message, "OauthToken");
                return new ContentResult()
                {
                    Content = jsonResult,
                    ContentType = "application/json",
                    StatusCode = 500
                };
            }
            catch (Exception ex)
            {
                Logger.Error("OauthToken -> " + ex.Message);
            }

            return BadRequest(new
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
