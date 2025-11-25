using FortBackend.src.App.Utilities.ADMIN;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.ConfigHelpers;
using FortLibrary.Dynamics.Dashboard;
using FortLibrary.EpicResponses.FortniteServices.Content;
using FortLibrary.EpicResponses.FortniteServices.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;

namespace FortBackend.src.App.Routes.ADMIN
{
    /*
     *  Admin only
     */
    public class CreateBody
    {
        // need to move to FortLibrary

        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        // fails just pre-set time
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime EndTime { get; set; } = DateTime.UtcNow.AddHours(3);
        public string GivenItem { get; set; } = string.Empty;
        public string ItemQuantity { get; set; } = "1";
        public string CupPlacement { get; set; } = "50"; // top 50
    }

    public class CacheCupsData
    {
        public string ID { get; set; } = string.Empty;
        public string Region { get; set; } = "EU";
    }

    [Route("/admin/new/dashboard/content")]
    public class AdminCashCup : Controller
    {
        [HttpPost("cups/create")]
        public async Task<IActionResult> CreateCup()
        {
            Response.ContentType = "application/json";

            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken.ToLower()!)!;
                    if (adminData != null)
                    {
                        if (adminData.RoleId > AdminDashboardRoles.Moderator)
                        {
                            using var reader = new StreamReader(Request.Body);
                            string rawRequestBody = await reader.ReadToEndAsync();

                            if (!string.IsNullOrEmpty(rawRequestBody))
                            {
                                CreateBody createBody = JsonConvert.DeserializeObject<CreateBody>(rawRequestBody)!;

                                Console.WriteLine(JsonConvert.SerializeObject(createBody));
                                // stupid as check i could probably do a better way
                                if (createBody != null && (
                                    string.IsNullOrEmpty(createBody.title) ||
                                    string.IsNullOrEmpty(createBody.description) 
                                ) || createBody == null) {
                                    return Json(new
                                    {
                                        message = "Title or Description is empty",
                                        error = true,
                                    });
                                }

                                // if errors then thats just a skill issue
                                int CupPlacement = int.Parse(createBody.CupPlacement ?? "50"); // shouldnt hit this 50 ever lol
                                int ItemQuantity = int.Parse(createBody.ItemQuantity ?? "1");
                                // Creates cups, with cache

                                //RANDOM genratedid bc why not
                                string GeneratedID = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12);

                                List<object> Scores = JsonConvert.DeserializeObject<List<object>>(await System.IO.File.ReadAllTextAsync(PathConstants.Templates.Score)) ?? new List<object>();

                                TemplateC GGs = new TemplateC()
                                {
                                    eventTemplateId = $"eventTemplate_{GeneratedID}",
                                    matchCap = 7, // ill add this to the dashboard in the future w/ public/private events
                                    playlistId = "Playlist_ShowdownTournament_Solo", // dropdown maybe bc this is still the same on latest
                                    // this needs a cleanup
                                    tiebreakerFormula = new
                                    {
                                        basePointsBits = 11,
                                        components = new List<object> {
                                            new
                                            {
                                                trackedStat = "VICTORY_ROYALE_STAT",
                                                bits = 4,
                                                aggregation = "sum",
                                            },
                                            new
                                            {
                                                trackedStat = "TEAM_ELIMS_STAT_INDEX",
                                                bits = 13,
                                                aggregation = "avg",
                                                multiplier = 100
                                            },
                                            new
                                            {
                                                trackedStat = "TEAM_ELIMS_STAT_INDEX",
                                                bits = 13,
                                                aggregation = "avg",
                                                multiplier = 100
                                            },
                                             new
                                            {
                                                trackedStat = "TIME_ALIVE_STAT",
                                                bits = 13,
                                                aggregation = "avg",
                                            },
                                        }
                                    },
                                    payoutTable = new List<object>
                                    {
                                        new
                                        {
                                            scoringType = "rank",
                                            ranks = new List<object> {
                                                new
                                                {
                                                    threshold = CupPlacement,
                                                    payouts = new List<object>() {// need to implemetn a better way
                                                        new
                                                        {
                                                            rewardType = "game",
                                                            rewardMode = "standard",
                                                            value = createBody.GivenItem,
                                                            quantity = ItemQuantity,
                                                            notifiesPlayer = true
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    scoringRules = Scores,
                                    liveSessionAttributes = new List<object>()
                                };

                                EventC eventC = new EventC()
                                {
                                    announcementTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    beginTime = createBody.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    endTime = createBody.EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    displayDataId = $"display_{GeneratedID}",
                                    eventGroup = $"eventgroup_{GeneratedID}",
                                    eventId = $"event_{GeneratedID}",
                                    eventWindows = new List<EventWindowC>
                                    {
                                        new EventWindowC
                                        {
                                            eventWindowId = GeneratedID,
                                            eventTemplateId = $"eventTemplate_{GeneratedID}",
                                            beginTime = createBody.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            countdownBeginTime =  createBody.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            endTime = createBody.EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            leaderboardId = "LeaderBoardDefFRFR",
                                            round = 1,
                                            scoreLocations = new List<object>
                                            {
                                                new
                                                {
                                                    leaderboardDefId = "LeaderBoardDefFRFR",
                                                    isMainWindowLeaderboard = true,
                                                    leaderboardId = "LeaderBoardDefFRFR",
                                                    scoreMode = "window",
                                                }
                                            },
                                            teammateEligibility = "all",
                                            visibility = "public",
                                            metadata = new
                                            {
                                                ServerReplays = false,
                                                RoundType = "Qualifiers",
                                                liveSpectateAccessToken = "WeeklyTournamentSpectator"
                                            }
                                        }
                                    },
                                    gameId = "Fortnite",
                                    link = new
                                    {
                                        type = "br:tournament",
                                        code = $"tournament_{GeneratedID}",
                                        version = 1
                                    },
                                    metadata = new
                                    {
                                        TeamLockType = "Window",
                                        minimumAccountLevel = 15,
                                        DisqualifyType = "Window",
                                        RegionLockType = "Window",
                                        AccountLockType = "Window"
                                    }
                                };

                                var TemplatePath = PathConstants.CacheData($"Template_{GeneratedID}.json");
                                System.IO.File.WriteAllText(TemplatePath, JsonConvert.SerializeObject(GGs, Formatting.Indented));

                                var EventPath = PathConstants.CacheData($"Event_{GeneratedID}.json");
                                System.IO.File.WriteAllText(EventPath, JsonConvert.SerializeObject(eventC, Formatting.Indented));

                                // custom data helps dashboard/data fetching for download.json
                                // + need to remove data maybe like after 10 cups remove the other cups thats already used
                                var DeserizlisedCupContent = CupCache.cacheCupsDatas;
                                DeserizlisedCupContent.Insert(0, new CacheCupsData
                                {
                                    ID = GeneratedID,
                                    Region = "EU"
                                });
                                CupCache.Update(); // wow man
                                // need to work on this kinda

                                // this will be changable in the future! (only way to change is just going to content.json and changing hex
                                NewsManager.ContentConfig.tournamentinformation.Insert(0, new TournamentInformation
                                {
                                    title_color = "FFFFFF",
                                    loading_screen_image = "http://127.0.0.1:1111/image/LoadingScreen.png",
                                    background_text_color = "161616",
                                    background_right_color = "0054D3",
                                    poster_back_image = "http://127.0.0.1:1111/image/BackPoster.jpg",
                                    pin_earned_text = "Winner!",
                                    tournament_display_id = $"display_{GeneratedID}",
                                    highlight_color = "F7FF00",
                                    schedule_info = "Various Dates",
                                    primary_color = "FFFFFF",
                                    flavor_description = "",
                                    poster_front_image = "http://127.0.0.1:1111/image/BackPoster.jpg",
                                    short_format_title = "Solos Tournament",
                                    title_line_2 = "",
                                    title_line_1 = createBody.title,
                                    shadow_color = "161616",
                                    details_description = createBody.description,
                                    background_left_color = "00265F",
                                    long_format_title = "",
                                    poster_fade_color = "001A41",
                                    secondary_color = "161616",
                                    playlist_tile_image = "http://127.0.0.1:1111/image/LoadingScreen.png",
                                    base_color = "FFFFFF"
                                });
                                NewsManager.Update();

                                Logger.Log(createBody.title);
                                return Json(new
                                {
                                    message = "200",
                                    error = false,
                                });
                            }else
                            {
                                return Json(new
                                {
                                    message = "Raw Body Is Empty",
                                    error = true,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "AdminCashCup");
            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }


        // returns data
        [HttpGet("cups/{id}")]
        public async Task<IActionResult> CupDataaa(string id)
        {
            Response.ContentType = "application/json";

            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string? authToken))
                {
                    AdminData adminData = Saved.CachedAdminData.Data?.FirstOrDefault(e => e.AccessToken.ToLower() == authToken.ToLower()!)!;
                    if (adminData != null)
                    {
                        var DeserizlisedCupContent = CupCache.cacheCupsDatas.FirstOrDefault(e => e.ID == id);
                            
                            if(DeserizlisedCupContent != null)
                            {
                                Logger.Log(DeserizlisedCupContent.ID);
                                var TournamentInfo = NewsManager.ContentConfig.tournamentinformation.FirstOrDefault(e => e.tournament_display_id == $"display_{id}");
                                if (TournamentInfo != null)
                                {
                                    return Json(new
                                    {
                                        body = new
                                        {
                                            title = TournamentInfo.title_line_1,
                                            description = TournamentInfo.details_description,
                                        },
                                        error = false,
                                    });
                                }
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                message = "Raw Body Is Empty",
                                error = true,
                            });
                        }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(new
            {
                message = "Couldn't find content",
                error = true,
            });
        }
    }
}
