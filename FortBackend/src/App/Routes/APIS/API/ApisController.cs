using Discord;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Content;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Numerics;

namespace FortBackend.src.App.Routes.APIS.API
{
    [ApiController]
    [Route("api")]
    public class ApisController : ControllerBase
    {
        [HttpPost("/datarouter/api/v1/public/data")]
        public async Task<IActionResult> DataRouter()
        {
            var queryParameters = HttpContext.Request.Query;
            Console.WriteLine("Query Parameters:");
            foreach (var (key, value) in queryParameters)
            {
                Console.WriteLine($"{key}: {value}");
            }
            var headers = HttpContext.Request.Headers;
            Console.WriteLine("\nHeaders:");
            foreach (var (key, value) in headers)
            {
                Console.WriteLine($"{key}: {value}");
            }

            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                try
                {
                    var requestBody = await reader.ReadToEndAsync();
                    Console.WriteLine($"Request Body: {requestBody}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading request body: {ex.Message}");
                }
            }
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
                accountId = accountId,
                stats = new { } // stats like "smth": number
            });
        }

        ///api/v1/events/Fortnite/download/644812f9-5e5e-4fd4-a670-b306e5956fd9
        [HttpGet("v1/events/Fortnite/download/{accountId}")]
        public IActionResult DownloadEndpoint(string accountId)
        {
            return Ok(new
            {
                player = new
                {
                    gameId = "Fortnite",
                    accountId = accountId,
                    tokens = new List<string>()
                    {
                        "ARENA_S15_Division1"
                    },
                    teams = new { },
                    pendingPayouts = new List<string>(),
                    pendingPenalties = new { },
                    persistentScores = new {
                        Hype = 69
                    },
                    groupIdentity = new { },
                },
                events = new List<object>()
                {
                    new
                    {
                        announcementTime = "2000-01-29T08:00:00.000Z",
                        appId = "",
                        gameId = "Fortnite",
                        beginTime = "2000-00-00T00:00:00.000Z",
                        endTime = "9999-00-00T00:00:000Z",
                        displayDataId = "arena_solo",
                        environment = "",
                        eventId = "epicgames_Arena_S15_Solo",
                        eventGroup = "",
                        eventWindows = new List<object>()
                        {
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division1_Solo",
                                eventWindowId = "Arena_S15_Division1_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 25,
                                    divisionRank = 0
                                },
                                payoutDelay = 30,
                                round = 0,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division1"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division2_Solo",
                                eventWindowId = "Arena_S15_Division2_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 75,
                                    divisionRank = 1
                                },
                                payoutDelay = 30,
                                round = 1,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division2"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division3_Solo",
                                eventWindowId = "Arena_S15_Division3_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 125,
                                    divisionRank = 2
                                },
                                payoutDelay = 30,
                                round = 2,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division3"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division4_Solo",
                                eventWindowId = "Arena_S15_Division4_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 175,
                                    divisionRank = 3
                                },
                                payoutDelay = 30,
                                round = 3,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division4"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division5_Solo",
                                eventWindowId = "Arena_S15_Division5_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 225,
                                    divisionRank = 4
                                },
                                payoutDelay = 30,
                                round = 4,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division5"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division6_Solo",
                                eventWindowId = "Arena_S15_Division6_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 300,
                                    divisionRank = 5
                                },
                                payoutDelay = 30,
                                round = 5,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division6"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division7_Solo",
                                eventWindowId = "Arena_S15_Division7_Solo",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 9999999999,
                                    divisionRank = 6
                                },
                                payoutDelay = 30,
                                round = 6,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division7"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             }
                        },
                        link = "",
                        metadata = new
                        {
                            TrackedStats = new string[]
                            {
                                "PLACEMENT_STAT_INDEX",
                                "TEAM_ELIMS_STAT_INDEX",
                                "MATCH_PLAYED_STAT"
                            },
                            minimumAccountLevel = 0,
                        },
                        platformMappings = new { },
                        platforms = new string[]
                        {
                            "PS4",
                            "XboxOne",
                            "Switch",
                            "Android",
                            "IOS",
                            "Windows"
                        },
                        regionMapping = new { },
                        regions = new string[] {
                            "NAE",
                            "ME",
                            "NAW",
                            "OCE",
                            "ASIA",
                            "EU",
                            "BR"
                        },

                    },
                    new
                    {
                        announcementTime = "2000-01-29T08:00:00.000Z",
                        appId = "",
                        gameId = "Fortnite",
                        beginTime = "2000-00-00T00:00:00.000Z",
                        endTime = "9999-00-00T00:00:000Z",
                        displayDataId = "arena_duos",
                        environment = "",
                        eventId = "epicgames_Arena_S15_Duos",
                        eventGroup = "",
                        eventWindows = new List<object>()
                        {
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division1_Duos",
                                eventWindowId = "Arena_S15_Division1_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 25,
                                    divisionRank = 0
                                },
                                payoutDelay = 30,
                                round = 0,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division1"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division2_Duos",
                                eventWindowId = "Arena_S15_Division2_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 75,
                                    divisionRank = 1
                                },
                                payoutDelay = 30,
                                round = 1,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division2"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division3_Duos",
                                eventWindowId = "Arena_S15_Division3_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 125,
                                    divisionRank = 2
                                },
                                payoutDelay = 30,
                                round = 2,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division3"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division4_Duos",
                                eventWindowId = "Arena_S15_Division4_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 175,
                                    divisionRank = 3
                                },
                                payoutDelay = 30,
                                round = 3,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division4"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division5_Duos",
                                eventWindowId = "Arena_S15_Division5_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 225,
                                    divisionRank = 4
                                },
                                payoutDelay = 30,
                                round = 4,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division5"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division6",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division6_Duos",
                                eventWindowId = "Arena_S15_Division6_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 300,
                                    divisionRank = 5
                                },
                                payoutDelay = 30,
                                round = 5,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division6"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division7"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             },
                             new
                             {
                                additionalRequiremenets = new string[] {},
                                beginTime = "2000-00-00T00:00:00.000Z",
                                endTime = "9999-00-00T00:00:000Z",
                                blackoutPeriods = new string[0],
                                canLiveSpectate = false,
                                countdownBeginTime = "2000-00-00T00:00:00.000Z",
                                eventTemplateId = "eventTemplate_Arena_S15_Division7_Duos",
                                eventWindowId = "Arena_S15_Division7_Duos",
                                isTBD = false,
                                metadata = new
                                {
                                    RoundType = "Arena",
                                    ThresholdToAdvanceDivision = 9999999999,
                                    divisionRank = 6
                                },
                                payoutDelay = 30,
                                round = 6,
                                requireAllTokens = new string[]
                                {
                                    "ARENA_S15_Division7"
                                },
                                requireAnyTokens = new string[0],
                                requireNoneTokensCaller = new string[]
                                {
                                    "ARENA_S15_Division1",
                                    "ARENA_S15_Division2",
                                    "ARENA_S15_Division3",
                                    "ARENA_S15_Division4",
                                    "ARENA_S15_Division5",
                                    "ARENA_S15_Division6"
                                },
                                requireAllTokensCaller = new string[0],
                                storeLocations = new List<object>()
                                {
                                    new
                                    {
                                        storeMode = "winsow",
                                        leaderboardId = "Fortnite_EU",
                                        useIndividualScores = false
                                    }
                                },
                                visibility = "public",
                                teammateEligibility = "all",
                                regionMappings = new { },
                             }
                        },
                        link = "",
                        metadata = new
                        {
                            TrackedStats = new string[]
                            {
                                "PLACEMENT_STAT_INDEX",
                                "TEAM_ELIMS_STAT_INDEX",
                                "MATCH_PLAYED_STAT"
                            },
                            minimumAccountLevel = 0,
                        },
                        platformMappings = new { },
                        platforms = new string[]
                        {
                            "PS4",
                            "XboxOne",
                            "Switch",
                            "Android",
                            "IOS",
                            "Windows"
                        },
                        regionMapping = new { },
                        regions = new string[] {
                            "NAE",
                            "ME",
                            "NAW",
                            "OCE",
                            "ASIA",
                            "EU",
                            "BR"
                        },

                    }
                },
                templates = new List<object>()
                {
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division1_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division2_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division3_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division4_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division5_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division6_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division7_Solo",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Solo",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division1_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division2_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division3_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division4_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division5_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division6_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    },
                    new {
                        eventTemplateId = "eventTemplate_Arena_S15_Division7_Duos",
                        gameId = "Fortnite",
                        matchCap = 100,
                        persistentScoreId = "Hype",
                        playlistId = "Playlist_ShowdownAlt_Duos",
                        scoringRules = new List<object>
                        {
                            new
                            {
                                matchRule = "lte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                         keyValue = 1,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    },
                                    new
                                    {
                                         keyValue = 3,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 7,
                                         multiplicative = false,
                                         pointsEarned = 2
                                    },
                                    new
                                    {
                                         keyValue = 12,
                                         multiplicative = false,
                                         pointsEarned = 3
                                    }
                                },
                                trackedStat = "PLACEMENT_STAT_INDEX"
                            },
                            new
                            {
                                matchRule = "gte",
                                rewardTiers = new List<object>()
                                {
                                    new
                                    {
                                        keyValue = 1,
                                        multiplicative = true,
                                        pointsEarned = 1
                                    }
                                },
                                trackedStat = "TEAM_ELIMS_STAT_INDEX"
                            }
                        }
                    }
                },
                leaderboardDefs = new List<object>(),
                scoringRuleSets = new { },
                payoutTables = new { },
                scores = new { }
            });
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
                    if(AccountDataParsed != null)
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

        [HttpPost("/v1/assets/Fortnite/{version}/{number}")]
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
