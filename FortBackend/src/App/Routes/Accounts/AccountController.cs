using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.Metrics;
using FortLibrary.EpicResponses.Errors;

namespace FortBackend.src.App.Routes.APIS.Accounts
{
    [ApiController]
    [Route("account/api")]
    public class AccountApiController : ControllerBase
    {
        [HttpGet("public/account/{accountId}")]
        public async Task<IActionResult> AccountAcc(string accountId)
        {
            var UserData1 = await Handlers.FindOne<User>("accountId", accountId);
            if (UserData1 != "Error")
            {
                User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData1)![0];

                if(UserDataParsed != null)
                {

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
                        tfaEnabled = false,
                        emailVerified = true,
                        minorVerified = false,
                        minorExpected = false,
                        minorStatus = "UNKOWN"
                    });
                }
            }

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
                    var UserData1 = await Handlers.FindOne<User>("Username", query);
                    if (UserData1 != "Error")
                    {
                        User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData1)?[0]!;

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
            var UserData1 = await Handlers.FindOne<User>("Username", displayName);
            if (UserData1 != "Error")
            {
                User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData1)?[0]!;

                if (UserDataParsed != null)
                {

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
                        tfaEnabled = false,
                        emailVerified = true,
                        minorVerified = false,
                        minorExpected = false,
                        minorStatus = "UNKOWN"
                    });
                }
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
                    Console.WriteLine(RequestQuery);
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
                Console.WriteLine("Errlr " + ex.Message);
                return Ok(Array.Empty<string>());
            }
        }      
    }
}
