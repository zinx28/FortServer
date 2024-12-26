using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.Metrics;
using FortLibrary.EpicResponses.Errors;
using FortLibrary.MongoDB.Module;
using FortLibrary;
using System.Collections.Generic;

namespace FortBackend.src.App.Routes.APIS.Accounts
{
    [ApiController]
    [Route("account/api")]
    public class AccountApiController : ControllerBase
    {
        [HttpGet("public/account/{accountId}")]
        public async Task<IActionResult> AccountAcc(string accountId)
        {
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId); 
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    User UserDataParsed = profileCacheEntry.UserData;
                    Account AccountDataParsed = profileCacheEntry.AccountData;

                    return Ok(new
                    {
                        id = UserDataParsed.AccountId,
                        displayName = UserDataParsed.Username,
                        name = UserDataParsed.Username,
                        lastName = UserDataParsed.Username,
                        email = UserDataParsed.Email,
                        failedLoginAttempts = 0,
                        lastLogin = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        numberOfDisplayNameChanges = 0,
                        ageGroup = "UNKNOWN",
                        headless = false,
                        country = "US",
                        canUpdateDisplayName = false,
                        tfaEnabled = AccountDataParsed.commoncore.mfa_enabled,
                        emailVerified = true,
                        minorVerified = false,
                        minorExpected = false,
                        minorStatus = "UNKOWN"
                    });
                } 
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        // THIS IS WIP
        //persona/api/public/account/lookup
        [HttpGet("/persona/api/public/account/lookup")]
        public async Task<IActionResult> PersonaAPI([FromQuery(Name = "q")] string query)
        {
            try
            {
                if (query != null && !query.Contains(","))
                {
                    ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(query, "Username");;
                    if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        User UserDataParsed = profileCacheEntry.UserData;

                        if (UserDataParsed != null)
                        {

                            return Ok(new
                            {
                                id = UserDataParsed.AccountId,
                                displayName = UserDataParsed.Username,
                            });
                        }
                    }

                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.account.account_not_found",
                        errorMessage = $"Sorry, we couldn't find an account for {query}",
                        messageVars = new List<string> { $"/persona/api/public/account/lookup" },
                        numericErrorCode = 18007,
                        originatingService = "any",
                        intent = "prod",
                        error_description = $"Sorry, we couldn't find an account for {query}",
                    };
                }
                else
                {
                    throw new BaseError
                    {
                        errorCode = "errors.com.epicgames.iforgot",
                        errorMessage = $"Sorry, we couldn't find an account for {query}",
                        messageVars = new List<string> { $"/persona/api/public/account/lookup" },
                        numericErrorCode = 18007,
                        originatingService = "any",
                        intent = "prod",
                        error_description = $"Sorry, we couldn't find an account for {query}",
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
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
          
            return Ok(new { });
        }

        // this works
        [HttpGet("public/account/displayName/{displayName}")]
        public async Task<IActionResult> DisplayNameSearch(string displayName)
        {       
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.ProfileDiscord(displayName, "Username");
                //var UserData = await Handlers.FindOne<User>("Username", displayName);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    User UserDataParsed = profileCacheEntry.UserData;
                    Account AccountDataParsed = profileCacheEntry.AccountData;

                    if(UserDataParsed == null || AccountDataParsed == null)
                        throw new BaseError
                        {
                            errorCode = "errors.com.epicgames.account.account_not_found",
                            errorMessage = $"Sorry, we couldn't find an account for {displayName}",
                            messageVars = new List<string> { $"/account/api/public/account/displayName/{displayName}" },
                            numericErrorCode = 18007,
                            originatingService = "any",
                            intent = "prod",
                            error_description = $"Sorry, we couldn't find an account for {displayName}",
                        };

                    return Ok(new
                    {
                        id = UserDataParsed.AccountId,
                        displayName = UserDataParsed.Username,
                        name = UserDataParsed.Username,
                        lastName = UserDataParsed.Username,
                        email = UserDataParsed.Email,
                        failedLoginAttempts = 0,
                        lastLogin = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        numberOfDisplayNameChanges = 0,
                        ageGroup = "UNKNOWN",
                        headless = false,
                        country = "US",
                        canUpdateDisplayName = false,
                        tfaEnabled = AccountDataParsed.commoncore.mfa_enabled,
                        emailVerified = true,
                        minorVerified = false,
                        minorExpected = false,
                        minorStatus = "UNKOWN"
                    });
                }

                throw new BaseError
                {
                    errorCode = "errors.com.epicgames.account.account_not_found",
                    errorMessage = $"Sorry, we couldn't find an account for {displayName}",
                    messageVars = new List<string> { $"/account/api/public/account/displayName/{displayName}" },
                    numericErrorCode = 18007,
                    originatingService = "any",
                    intent = "prod",
                    error_description = $"Sorry, we couldn't find an account for {displayName}",
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
                Logger.Error(ex.Message);
            }

            return Ok(new { });
        }

        [HttpGet("public/account/{accountId}/externalAuths")]
        public IActionResult ExternalAccountAcc(string accountId)
        {
            return Ok(Array.Empty<string>());
        }

        [HttpGet("epicdomains/ssodomains")]
        public IActionResult SSODomains()
        {
            return Ok(new string[]
            {
                "unrealengine.com",
                "unrealtournament.com",
                "fortnite.com",
                "epicgames.com"
            });
        }


       [HttpGet("public/account")]
        public async Task<IActionResult> PublicAccount()
        {
            try
            {
                string RequestQuery = Request.Query["accountId"]!;
                ArrayList ResponseList = new ArrayList();

                if (RequestQuery == null)
                {
                    return Ok(Array.Empty<string>());
                }

                if (RequestQuery.Contains(","))
                {
                    string[] accountIds = RequestQuery.Split(',');

                    foreach (var AccountId in accountIds)
                    {
                        var UserData = await Handlers.FindOne<User>("accountId", AccountId);

                        if (UserData != "Error")
                        {
                            User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0]!;
                            if (UserDataParsed != null)
                            {
                                if (UserDataParsed.AccountId.ToString() == AccountId)
                                {
                                    ResponseList.Add(new
                                    {
                                        id = AccountId,
                                        displayName = UserDataParsed.Username.ToString(),
                                        externalAuts = new { }
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var UserData = await Handlers.FindOne<User>("accountId", RequestQuery);

                    if (UserData != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)?[0]!;
                        if (UserDataParsed != null && UserDataParsed.AccountId == RequestQuery)
                        {
                            ResponseList.Add(new
                            {
                                id = UserDataParsed.AccountId,
                                displayName = UserDataParsed.Username,
                                externalAuths = new { }
                            });
                        }
                    }
                }

                return Ok(ResponseList);
            }
            catch (Exception ex)
            {
                Logger.Error($"public/account {ex.Message}");
            }

            return Ok(Array.Empty<string>());
        }      
    }
}
