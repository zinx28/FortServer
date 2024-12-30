using FortLibrary.EpicResponses.LeaderBoard;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FortLibrary;
using FortLibrary.EpicResponses.Fortnite;
using System.Text.Json;

namespace FortBackend.src.App.Routes.Leaderboards
{
    [ApiController]
    [Route("fortnite/api")]
    public class StatsControllerController : ControllerBase
    {

        [HttpGet("game/v2/leaderboards/cohort/{accountId}")]
        public ActionResult EarlyStats(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                var Playlist = Request.Query["playlist"].FirstOrDefault();
                if (Playlist != null)
                {
                    LeaderBoardStats test = UpdateLeaderBoard.LeaderboardCached.Data.FirstOrDefault(e => e.statName.Contains(Playlist))!;
                    if (test != null)
                    {
                        List<string> accountIds = new List<string>();

                        foreach (var item in test.stat)
                        {
                            accountIds.Add(item.Key);
                        }
                        return Ok(new
                        {
                            accountId,
                            cohortAccounts = accountIds,
                            expiresAt = "9999-12-31T00:00:00.000Z",
                            playlist = Playlist
                        });
                    }
                }
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        //fortnite/api/leaderboards/type/group/stat/br_placetop3_pc_m0_p9/window/weekly
        [HttpPost("leaderboards/type/{type}/stat/{statName}/window/{tab}")]
        public async Task<ActionResult> GrabLeaderboardStats(string statName, string type, string tab)
        {
            Response.ContentType = "application/json";
            try
            {
                //group
                //global
                List<object> entrieslist = new List<object>();
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.Latin1))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var array = JsonConvert.DeserializeObject<List<string>>(requestBody);

                    if (array != null)
                    {
                        LeaderBoardStats leaderBoardStats = UpdateLeaderBoard.LeaderboardCached.Data.FirstOrDefault(e => e.statName.Contains(statName))!;
                        if (leaderBoardStats != null)
                        {

                            foreach (var item in leaderBoardStats.stat)
                            {
                                entrieslist.Add(new
                                {
                                    accountId = item.Key,
                                    value = item.Value,
                                });
                            }
                        }
                    }

                    return Ok(new
                    {
                        entries = entrieslist,
                        statName = statName,
                        statWindow = tab
                    });
                }
            }
            catch (JsonReaderException)
            {
                return Ok(new { });
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        //fortnite/api/statsv2/query
        // Doesn't work ofc
        [HttpPost("statsv2/query")]
        public async Task<ActionResult> LeaderBoardStatsQuery()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.Default))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    StatsBody StatsBody = JsonConvert.DeserializeObject<StatsBody>(requestBody);
                    // string RequestQuery = Request.Query["accountId"]!;
                    return Ok(new
                    {
                        owners = StatsBody.owners,
                        stats = StatsBody.stats
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "StatsV2");
            }

            return Ok(new string[0]);
        }

        [HttpGet("/statsproxy/api/statsv2/leaderboards/{windowId}")]
        [HttpGet("statsv2/leaderboards/{windowId}")]
        public async Task<ActionResult> LeaderBoardStatsNew(string windowId)
        {
            Response.ContentType = "application/json";
            try
            {
                List<object> entrieslist = new List<object>();

                LeaderBoardStats leaderBoardStats = UpdateLeaderBoard.LeaderboardCached.Data.FirstOrDefault(e => e.statName.Contains(windowId))!;
                if (leaderBoardStats != null)
                {

                    foreach (var item in leaderBoardStats.stat)
                    {
                        entrieslist.Add(new
                        {
                            account = item.Key,
                            value = item.Value,
                        });
                    }
                }


                return Ok(new
                {
                    entries = entrieslist,
                    maxSize = entrieslist.Count
                });
            }
            catch (JsonReaderException)
            {
                return Ok(new { });
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        [HttpGet("stats/accountId/{accountId}/bulk/window/{windowId}")]
        public async Task<ActionResult> WindowAllTime(string accountId, string windowId)
        {
            Response.ContentType = "application/json";
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    var Response = new List<object>();

                    foreach (var item in profileCacheEntry.StatsData.stats)
                    {
                        Response.Add(new
                        {
                            name = item.Key,
                            value = item.Value,
                            window = windowId,
                            ownerType = 1
                        });
                    }
                    return Ok(Response);
                }
            }
            catch (JsonReaderException)
            {
                return Ok(new { });
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        //statsproxy/api/statsv2/account/
        [HttpGet("/statsproxy/api/statsv2/account/{accountId}")]
        public async Task<IActionResult> StatsProxy(string accountId)
        {
            long ticksInOneDay = TimeSpan.TicksPerDay;
            long ticksInOneYear = 365 * ticksInOneDay;

            ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
            if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
            {

                return Ok(new
                {
                    startTime = 0,
                    endTime = ticksInOneYear,
                    accountId,
                    stats = profileCacheEntry.StatsData.stats // stats like "smth": number
                });
            }

            return Ok(new
            {
                startTime = 0,
                endTime = ticksInOneYear,
                accountId,
                stats = new { }
            });
        }


        [HttpGet("statsv2/account/{accountId}")]
        public async Task<ActionResult> StatsV2(string accountId)
        {
            Response.ContentType = "application/json";
            try
            {
                ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                {
                    var Response = new List<object>();
                    long ticksInOneDay = TimeSpan.TicksPerDay;
                    long ticksInOneYear = 365 * ticksInOneDay;

                    return Ok(new
                    {
                        startTime = 0,
                        endTime = ticksInOneYear,
                        accountId,
                        stats = profileCacheEntry.StatsData.stats
                    });
                }
            }
            catch (JsonReaderException)
            {
                return Ok(new { });
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }

        [HttpPost("/statsproxy/api/statsv2/query")]
        public async Task<ActionResult> ProxtyStatsV2([FromBody] JsonElement requestBody)
        {
            Response.ContentType = "application/json";
            try
            {
                long ticksInOneDay = TimeSpan.TicksPerDay;
                long ticksInOneYear = 365 * ticksInOneDay;
                if (requestBody.TryGetProperty("owners", out JsonElement ownersElement) && ownersElement.ValueKind == JsonValueKind.Array)
                {
                    var owners = System.Text.Json.JsonSerializer.Deserialize<List<string>>(ownersElement.GetRawText());
                    List<object> test = new();
                    var stats = new
                    {
                        br_collection_fish_gas_purple_length_s16 = 49,
                        s23_social_bp_level = 69
                    };
                    foreach (var owner in owners)
                    {
                        //   ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);
                        test.Add(new
                        {
                            startTime = 0,
                            endTime = ticksInOneYear,
                            owner,
                            stats
                        });
                    }

                    return Ok(test);

                }
               
            }
            catch (JsonReaderException)
            {
                return Ok(new { });
            }
            catch (Exception ex) { Logger.Error(ex.Message); }

            return Ok(new { });
        }
    }
}
