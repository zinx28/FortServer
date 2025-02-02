﻿using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortLibrary.EpicResponses.FortniteServices.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using FortBackend.src.App.Utilities.Constants;
using FortLibrary.EpicResponses.Profile.Quests;
using FortLibrary;
using System.Text.Json.Serialization;
using FortBackend.src.App.Utilities.Helpers.Cached;

namespace FortBackend.src.App.Routes.API
{
    public class AvatarGulp
    {
        public string accountId { get; set; } = string.Empty;

        [JsonProperty("namespace")]
        [JsonPropertyName("namespace")]
        public string wownamespace { get; set; } = string.Empty;

        public string avatarId { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api")]
    public class ApisController : ControllerBase
    {
        ///v1/avatar/fortnite/ids
        ///

        [HttpGet("/v1/avatar/fortnite/ids")]
        public IActionResult AvatarIds()
        {
            Response.ContentType = "application/json";
            return Ok(new object[]
            {
                new AvatarGulp
                {
                    accountId = "therizzler",
                    wownamespace = "fortnite",
                    avatarId = "ATHENACHARACTER:CID_001_Athena_Commando_F_Default"
                }
            });
        }

        ///profile/languages
        [HttpPut("/profile/languages")]
        public IActionResult languages()
        {
            Response.ContentType = "application/json";
            return Ok(new { });
        }

        // NEED T OWROK ON THIS
        [HttpPut("/profile/privacy_settings")]
        public IActionResult privacy_settings()
        {
            Response.ContentType = "application/json";
            return Ok(new {
                privacySettings = new
                {
                    playRegion = "PRIVATE",
                    badges = "FRIENDS_ONLY",
                    languages = "languages"
                }
            });
        }

        //api/v2/interactions/latest/Fortnite/

        [HttpGet("v2/interactions/latest/Fortnite/{accountId}")]
        public IActionResult interactionsLatest(string accountId)
        {
            Response.ContentType = "application/json";
            return StatusCode(204);
        }

        [HttpGet("v2/interactions/aggregated/Fortnite/{accountId}")]
        public IActionResult interactionsAggregated(string accountId)
        {
            Response.ContentType = "application/json";
            return StatusCode(204);
        }


        [HttpPost("/datarouter/api/v1/public/data/{a?}")]

        // could give event data from the game? this could be miss used though
        //[HttpPost("/datarouter/api/v1/public/data")]
        public IActionResult DataRouter([FromRoute] string? a)
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
            return StatusCode(204) ;
        }

        [HttpPut("/profile/play_region")]


        public IActionResult PlayRegion()
        {
            Response.ContentType = "application/json";
            return Ok(new { });
        }

        [HttpPost("/api/v1/fortnite-br/interactions")]


        public IActionResult ForniteBRInteractions()
        {
           // Response.ContentType = "application/json";
            return StatusCode(200);
        }

        ///api/v1/fortnite-br/interactions

        [HttpPost("/region/check")]


        public IActionResult RegionCheck()
        {
            Response.ContentType = "application/json";
            return Ok(new
            {
                content_id = "AF9yLAAsklQALFTy",
                allowed = true,
                resolved = true,
                limit = "Res=656"
            });
        }

        [HttpGet("/region")]
        public IActionResult RegionAh()
        {
            Response.ContentType = "application/json";
            return Ok(new
            {
                continent = new
                {
                    code = "EU",
                    geoname_id = 6255148,
                    names = new
                    {
                        de = "Europa",
                        en = "Europe",
                        es = "Europa",
                        fr = "Europe",
                        ja = "ヨーロッパ",
                        pt_BR = "Europa",
                        ru = "Европа",
                        zh_CN = "欧洲"
                    }
                },
                country = new
                {
                    geoname_id = 2635167,
                    is_in_european_union = false,
                    iso_code = "GB",
                    names = new
                    {
                        de = "UK",
                        en = "United Kingdom",
                        es = "RU",
                        fr = "Royaume Uni",
                        ja = "英国",
                        pt_BR = "Reino Unido",
                        ru = "Британия",
                        zh_CN = "英国"
                    }
                },
                subdivisions = new object[]
                {
                    new
                    {
                        geoname_id = 6269131,
                        iso_code = "ENG",
                        names = new
                        {
                            de = "England",
                            en = "England",
                            es = "Inglaterra",
                            fr = "Angleterre",
                            ja = "イングランド",
                            pt_BR = "Inglaterra",
                            ru = "Англия",
                            zh_CN = "英格兰"
                        }
                    },
                    new
                    {
                        geoname_id = 3333121,
                        iso_code = "BNE",
                        names = new
                        {
                            de = "London Borough of Barnet",
                            en = "Barnet",
                            fr = "Barnet"
                        }
                    }
                }
            });
        }

        [HttpGet("v1/Fortnite/get")]
        public IActionResult FortniteGet()
        {
            Response.ContentType = "application/json";
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
                var profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null)
                {
                    var userAgent = Request.Headers["User-Agent"].ToString();
                    int Season;
                    Season = (await Grabber.SeasonUserAgent(Request)).Season;


                    string filePath1 = PathConstants.Templates.Events;
                    string json1 = System.IO.File.ReadAllText(filePath1);
                    var jsonResponse = JsonConvert.DeserializeObject<List<EventC>>(json1);

                    string filePath2 = PathConstants.Templates.Arena;
                    string json2 = System.IO.File.ReadAllText(filePath2);
                    var jsonResponse2 = JsonConvert.DeserializeObject<List<TemplateC>>(json2);

                    if (jsonResponse2 != null)
                    {
                        foreach (var templateC in jsonResponse2)
                        {
                            if (templateC.eventTemplateId.Contains("S15"))
                            {
                                templateC.eventTemplateId = templateC.eventTemplateId.Replace("S15", $"S{Season}");
                            }
                        }
                    }

                    if (jsonResponse != null)
                    {
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
                    }
                        

                    bool FoundSeasonDataInProfile = profileCacheEntry.AccountData.commoncore.Seasons.Any(season => season.SeasonNumber == Season);
                    string[] tokens = new string[0];
                    if (Season >= 8 && Season < 23)
                    {
                        tokens = new string[] {
                            $"ARENA_S{Season}_Division1"
                        };
                    }
                    if (!FoundSeasonDataInProfile)
                    {
                        profileCacheEntry.AccountData.commoncore.Seasons.Add(new SeasonClass
                        {
                            SeasonNumber = Season,
                            SeasonXP = 0,
                            BookLevel = 1,
                            BookXP = 0,
                            BookPurchased = false,
                            Quests = new Dictionary<string, DailyQuestsData>(),
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
                    }

                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons!.FirstOrDefault(season => season.SeasonNumber == Season)!;

                    if (seasonObject != null)
                    {
                        dynamic persistentScores = new ExpandoObject();
                        persistentScores.Hype = seasonObject.events.persistentScores.Hype;
                        ((IDictionary<string, object>)persistentScores)[$"Hype_{Season}"] = seasonObject.events.persistentScores.Hype;
                        if (Season >= 8 && Season < 23)
                        {
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
                        else
                        {
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
                                    persistentScores = new
                                    {
                                        seasonObject.events.persistentScores.Hype,
                                    },
                                    teams = new { },
                                    seasonObject.events.tokens,
                                },
                                templates = new List<object>(),
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

        //content-controls/

        [HttpGet("/content-controls/{accountId}")]
        public IActionResult ContentControls(string accountId)
        {
            return StatusCode(201);
        }

        [HttpGet("/sdk/v1/product/prod-fn")]
        public IActionResult ProdFN()
        {
            Response.ContentType = "application/json";
            try
            {
                var DefaultSDKPath = PathConstants.FN_PROD;

                if (System.IO.File.Exists(DefaultSDKPath))
                {
                    var ReadFile = System.IO.File.ReadAllText(DefaultSDKPath);
                    if (!string.IsNullOrEmpty(ReadFile)) { return Content(ReadFile, "application/json"); }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "SDKDEFAULT!!");
            }

            return Ok(new { });
        }



        [HttpGet("v1/search/{accountId}")]
        public async Task<IActionResult> SearchPlayer(string accountId, [FromQuery] string prefix)
        {
            Response.ContentType = "application/json";
            var Search = new List<object>();
            try
            {
                if (!string.IsNullOrEmpty(prefix))
                {
                    var Users = await Handlers.FindSimilar<User>("Username", prefix, 5, $".*{Regex.Escape(prefix)}\\d*.*");

                    if (Users != "Error")
                    {
                        List<User> FortniteList = JsonConvert.DeserializeObject<List<User>>(Users)!;

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

        //v1/epic-settings/public/users/372da84236e342c297ca36599deb669d/values

        [HttpGet("/v1/epic-settings/public/users/{accountId}/values")]
        public IActionResult EpicSettings(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                
                string ConfigFilePath = PathConstants.EpicSettings;

                if (Path.Exists(ConfigFilePath))
                {
                    string ConfigFile = System.IO.File.ReadAllText(ConfigFilePath);

                    return Content(ConfigFile, "application/json");
                }
                else
                {
                    Logger.Error(ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ApiController-EpicSettings");
            }

            return NoContent();
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
                    if(!string.IsNullOrEmpty(accountId1)) 
                         accountId = accountId1!;
                }
                var profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null)
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
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
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

        //hotconfigs/v2/livefn.json 

        [HttpGet("/hotconfigs/v2/{filename}")]
        public IActionResult HotConfigs(string filename)
        {
            Response.ContentType = "application/json";
            try
            {
                if (!Regex.IsMatch(filename, "^[a-zA-Z\\-\\._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }

                string ConfigFilePath = Path.Combine(PathConstants.BaseDir, filename);
               
                if (Path.Exists(ConfigFilePath))
                {
                    string ConfigFile = System.IO.File.ReadAllText(ConfigFilePath);

                    return Content(ConfigFile, "application/json");
                }
                else
                {
                    Logger.Error(ConfigFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CloudDIR");
            }

            return NoContent();
        }

        // /api/v1/assets/Fortnite/++Fortnite+Release-15.50/15526472?

        [HttpPost("v1/assets/Fortnite/{version}/{number}")]
        public async Task<IActionResult> AssetsV1(string version, string number)
        {
            Response.ContentType = "application/json";
            try
            {
                //using (var reader = new StreamReader(HttpContext.Request.Body))
                //{
                //    try
                //    {
                //        var requestBody = await reader.ReadToEndAsync();
                //        Console.WriteLine($"Request Body: {requestBody}");

                //        //CreativeDiscoveryAssetsResponse


                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Error reading request body: {ex.Message}");
                //    }
                //}
                var jsonResponse = JsonConvert.SerializeObject(NewsManager.CreativeDiscoveryAssetsResponse.DATA, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {

            }
            return Ok(new
            {
                FortCreativeDiscoverySurface = new
                {
                    meta = new
                    {
                        promotion = 0
                    },
                    assets = new { }
                }
            });
        }

        [HttpGet("/sdk/v1/default")]
        public IActionResult SDKDEFAULT()
        {
            Response.ContentType = "application/json";
            try
            {
                var DefaultSDKPath = PathConstants.SdkDefault;

                if (System.IO.File.Exists(DefaultSDKPath))
                {
                    var ReadFile = System.IO.File.ReadAllText(DefaultSDKPath);
                    if (!string.IsNullOrEmpty(ReadFile)) { return Content(ReadFile, "application/json"); }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "SDKDEFAULT!!");
            }

            return Ok(new { });
        }
    }
}
