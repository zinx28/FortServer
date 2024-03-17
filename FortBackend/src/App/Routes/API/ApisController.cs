using Amazon.Runtime;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.FortniteServices.Events;
using FortBackend.src.App.Utilities.Discord.Helpers.command;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("api")]
    public class ApisController : ControllerBase
    {
        [HttpPost("/datarouter/api/v1/public/data")]
        public async Task<IActionResult> DataRouter()
        {
            //var queryParameters = HttpContext.Request.Query;
            //Console.WriteLine("Query Parameters:");
            //foreach (var (key, value) in queryParameters)
            //{
            //    Console.WriteLine($"{key}: {value}");
            //}
            //var headers = HttpContext.Request.Headers;
            //Console.WriteLine("\nHeaders:");
            //foreach (var (key, value) in headers)
            //{
            //    Console.WriteLine($"{key}: {value}");
            //}

            //using (var reader = new StreamReader(HttpContext.Request.Body))
            //{
            //    try
            //    {
            //        var requestBody = await reader.ReadToEndAsync();
            //        Console.WriteLine($"Request Body: {requestBody}");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error reading request body: {ex.Message}");
            //    }
            //}
            return Ok();
        }

        //statsproxy/api/statsv2/account/
        [HttpGet("/statsproxy/api/statsv2/account/{accountId}")]
        public IActionResult StatsProxy(string accountId)
        {
            return Ok(new
            {
                startTime = 0,
                endTime = 9223372036854776000,
                accountId,
                stats = new { } // stats like "smth": number
            });
        }

        [HttpGet("v1/Fortnite/get")]
        public IActionResult FortniteGet()
        {
            return Ok(new
            {
                interactions = new List<object>()
            });
        }

        ///api/v1/events/Fortnite/download/644812f9-5e5e-4fd4-a670-b306e5956fd9
        [HttpGet("v1/events/Fortnite/download/{accountId}")]
        public async Task<IActionResult> DownloadEndpoint(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                var AccountData = await Handlers.FindOne<Account>("accountId", accountId);
                if (AccountData != "Error")
                {
                    var userAgent = Request.Headers["User-Agent"].ToString();
                    int Season;
                    Season = (await Grabber.SeasonUserAgent(Request)).Season;


                    string filePath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/templates/Events.json");
                    string json1 = System.IO.File.ReadAllText(filePath1);
                    var jsonResponse = JsonConvert.DeserializeObject<List<EventC>>(json1);

                    string filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/json/templates/Arena.json");
                    string json2 = System.IO.File.ReadAllText(filePath2);
                    var jsonResponse2 = JsonConvert.DeserializeObject<List<TemplateC>>(json2);

                    foreach (var templateC in jsonResponse2)
                    {
                        if (templateC.eventTemplateId.Contains("S15"))
                        {
                            templateC.eventTemplateId = templateC.eventTemplateId.Replace("S15", $"S{Season}");
                        }
                    }

                    foreach (EventC templateC in jsonResponse)
                    {
                        if (templateC.eventId.Contains("S15"))
                        {
                            templateC.eventId = templateC.eventId.Replace("S15", $"S{Season}");
                        }

                        foreach (EventWindowC EventWindowOb in templateC.eventWindows)
                        {
                            if (EventWindowOb.eventTemplateId.Contains("S15"))
                            {
                                EventWindowOb.eventTemplateId = EventWindowOb.eventTemplateId.Replace("S15", $"S{Season}");
                            }
                            if (EventWindowOb.eventWindowId.Contains("S15"))
                            {
                                EventWindowOb.eventWindowId = EventWindowOb.eventWindowId.Replace("S15", $"S{Season}");
                            }
                            EventWindowOb.requireAllTokens = EventWindowOb.requireAllTokens.Select(s => s.Contains("S15") ? s.Replace("S15", $"S{Season}") : s).ToArray();
                            EventWindowOb.requireNoneTokensCaller = EventWindowOb.requireNoneTokensCaller.Select(s => s.Contains("S15") ? s.Replace("S15", $"S{Season}") : s).ToArray();

                        }
                    }



                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];
                        
                    if (AccountDataParsed != null)
                    {
                        bool FoundSeasonDataInProfile = AccountDataParsed.commoncore.Seasons.Any(season => season.SeasonNumber == Season);
                        string[] tokens = new string[0];
                        if (Season >= 8 && Season < 23)
                        {
                            tokens = new string[] {
                                $"ARENA_S{Season}_Division1"
                            };
                        }
                        if (!FoundSeasonDataInProfile)
                        {
                            string seasonJson = JsonConvert.SerializeObject(new SeasonClass
                            {
                                SeasonNumber = Season,
                                BookLevel = 1,
                                BookXP = 0,
                                BookPurchased = false,
                                Quests = new List<Dictionary<string, object>>(),
                                BattleStars = 0,
                                DailyQuests = new DailyQuests
                                {
                                    Interval = "0001-01-01T00:00:00.000Z",
                                    Rerolls = 1
                                },
                                events = new Events
                                {
                                    tokens = tokens
                                }
                            });

                            await Handlers.PushOne<Account>("accountId", accountId, new Dictionary<string, object>
                            {
                                {
                                    "commoncore.Season", BsonDocument.Parse(seasonJson)
                                }
                            });

                            AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", accountId))[0];
                        }

                        SeasonClass seasonObject = AccountDataParsed.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season);

                        if (seasonObject != null)
                        {
                            dynamic persistentScores = new ExpandoObject();
                            persistentScores.Hype = seasonObject.events.persistentScores.Hype;
                            ((IDictionary<string, object>)persistentScores)[$"Hype_{Season}"] = seasonObject.events.persistentScores.Hype;
                            return Content(JsonConvert.SerializeObject(new
                            {
                                events = jsonResponse,
                                player = new
                                {
                                    accountId,
                                    gameId = "Fortnite",
                                    groupIdentity = new { },
                                    pendingPayouts = new List<string>(),
                                    pendingPenalties = new { },
                                    persistentScores = new
                                    {
                                        seasonObject.events.persistentScores.Hype,
                                    },
                                    teams = new { },
                                    seasonObject.events.tokens,
                                },
                                templates = jsonResponse2,
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("DownloadEnpoint: " + ex.Message);
            }
            return Content(JsonConvert.SerializeObject(new
            {
                events = new List<object>(),
                player = new
                {
                    accountId,
                    gameId = "Fortnite",
                    groupIdentity = new { },
                    pendingPayouts = new List<string>(),
                    pendingPenalties = new { },
                    persistentScores = new { },
                    teams = new { },
                    tokens = new string[0],
                },
                templates = new List<object>(),
            }));
        }

        [HttpGet("v1/search/{accountId}")]
        public async Task<IActionResult> SearchPlayer(string accountId, [FromQuery] string prefix)
        {
            var Search = new List<object>();
            try
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    var Users = await Handlers.FindSimilar<User>("Username", prefix, 5, $".*{Regex.Escape(prefix)}\\d*.*");
                    Console.WriteLine("TEST");
                    if (Users != "Error")
                    {
                        Console.WriteLine(Users);
                        List<User> FortniteList = JsonConvert.DeserializeObject<List<User>>(Users);
                        Console.WriteLine("TEST2");
                        if (FortniteList != null)
                        {
                            foreach (User user in FortniteList)
                            {
                                Search.Add(new
                                {
                                    accountId = user.AccountId,
                                    matches = new List<object>()
                                    {
                                        new
                                        {
                                            value = user.Username,
                                            platform = "epic"
                                        }
                                    },
                                    matchType = prefix.ToLower() == user.Username.ToLower() ? "exact" : "prefix",
                                    epicMutuals = 0, // check other friends ig?
                                    sortPosition = Search.Count
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SearchPlayers -> " + ex.Message);
            }
            
            return Ok(Search);
        }

        //api/v1/user/setting

        [HttpPost("v1/user/setting")]
        public async Task<IActionResult> SettingsUser()
        {
            Response.ContentType = "application/json";
            try
            {
                var FormRequest = HttpContext.Request.Form;

                string accountId = "";

                if (FormRequest.TryGetValue("accountId", out var accountId1))
                {
                    accountId = accountId1;
                }


                Console.WriteLine(accountId);
                var AccountData = await Handlers.FindOne<Account>("accountId", accountId);
                if (AccountData != "Error")
                {
                    Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)?[0];
                    if (AccountDataParsed != null)
                    {
                        //return Ok(new List<object>() {
                        //    new
                        //    {
                        //        accountId = accountId,
                        //        key = "avatar",
                        //        value = $"{AccountDataParsed.athena.Items[AccountDataParsed.athena.last_applied_loadout]["attributes"]["locker_slots_data"]["slots"]["character"]["items"][0]}"
                        //    },
                        //    new {
                        //        accountId = accountId,
                        //        key = "avatarBackground",
                        //        value = "[\"#B4F2FE\",\"#00ACF2\",\"#005679\"]" // TEMP DON't WRRORY!
                        //    }
                        //});
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(Array.Empty<string>());
        }

        [HttpGet("/waitingroom/api/waitingroom")]
        public IActionResult WaitingRoom()
        {
            return StatusCode(204);
        }

        [HttpGet("/eulatracking/api/public/agreements/fn/account/{accountId}")]
        public IActionResult eulatracking(string accountId)
        {
            return StatusCode(204);
        }

        //catalog/api/shared/bulk/offers

        [HttpGet("/catalog/api/shared/bulk/offers")]
        public IActionResult Catoffers()
        {
            Response.ContentType = "application/json";
            return Content("{}");
        }

        // /api/v1/assets/Fortnite/++Fortnite+Release-15.50/15526472?

        [HttpPost("v1/assets/Fortnite/{version}/{number}")]
        public IActionResult AssetsV1(string version, string number)
        {
            Response.ContentType = "application/json";
            return Ok(new
            {
                FortPlaylistAthena = new
                {
                    meta = new
                    {
                        promotion = 0
                    },
                    assets = new { }
                }
            });
        }
    }
}
