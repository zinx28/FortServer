using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.LeaderBoard;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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
                        Console.WriteLine("USING CACHED CODE!");
                        foreach (var item in test.stat)
                        {
                            Console.WriteLine("2 " + item.Key);
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
        [HttpPost("leaderboards/type/group/stat/{statName}/window/{tab}")]
        public async Task<ActionResult> GrabLeaderboardStats(string statName, string tab)
        {
            Response.ContentType = "application/json";
            try
            {
                List<object> entrieslist = new List<object>();
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.Latin1))
                {
                    string requestBody = await reader.ReadToEndAsync();
                    var array = JsonConvert.DeserializeObject<List<string>>(requestBody);

                    if(array != null)
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
                            //foreach (var item in array)
                            //{
                            //    entrieslist.Add(new
                            //    {
                            //        accountId = item,
                            //        value = leaderBoardStats.stat.try
                            //    });
                            //}
                        }
                    }

                    return Ok(new
                    {
                        entries = entrieslist,
                        statName = statName,
                        statWindow = tab
                    });
                                 // var Playlist = Request.Query["playlist"].FirstOrDefault();
                    //if (Playlist == "pc_m0_p9")
                    //{
                    //    List<string> accountIds = new List<string>();
                    //    Console.WriteLine("USING CACHED CODE!");
                    //    foreach (var item in UpdateLeaderBoard.LeaderboardCached.Squads)
                    //    {
                    //        Console.WriteLine("2 " + item.AccountId);
                    //        accountIds.Add(item.AccountId);
                    //    }
                    //    return Ok(new
                    //    {
                    //        accountId,
                    //        cohortAccounts = accountIds,
                    //        expiresAt = "9999-12-31T00:00:00.000Z",
                    //        playlist = Playlist
                    //    });
                    //}
                    //else
                    //{
                    //    return Ok(new
                    //    {
                    //        accountId,
                    //        cohortAccounts = new string[]
                    //        {
                    //            accountId
                    //        },
                    //        expiresAt = "9999-12-31T00:00:00.000Z",
                    //        playlist = Playlist
                    //    });
                    //};
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
