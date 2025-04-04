﻿using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.EpicResponses.Storefront;
using FortLibrary.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Newtonsoft.Json;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Storefront
{
    [ApiController]
    [Route("fortnite/api/calendar/v1/timeline")]
    public class TimelineApiController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GrabTimeline()
        {
            Response.ContentType = "application/json";
            try
            {
                string Json = System.IO.File.ReadAllText(PathConstants.ShopJson.Shop);
                if (Json == null)
                {
                    return BadRequest(new { });
                }

                ShopJson shopData = JsonConvert.DeserializeObject<ShopJson>(Json)!;

                if (shopData == null) { return BadRequest(new { }); } // if null return

                VersionClass season = await SeasonUserAgent(Request);

                string LobbyBackground = $"LobbySeason{season.Season}";
                if (season.Season == 2)
                {
                    LobbyBackground = "LobbyWinterDecor";
                }

                TimelineResponse Response = new TimelineResponse
                {
                    channels = new TimelineResponseChannels
                    {
                        ClientMatchmaking = new ClientMatchmakingTL() { cacheExpire = shopData?.expiration! },
                        StandaloneStore = new ClientMatchmakingTL()
                        {
                            states = new object[]
                            {
                                new
                                {
                                    validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    activeEvents = new string[0],
                                    state = new
                                    {
                                        activePurchaseLimitingEventIds = new string[0],
                                        storefront = new { },
                                        rmtPromotionConfig = new string[0],
                                        storeEnd = Saved.DeserializeGameConfig.SeasonEndDate
                                    }
                                }
                            },
                            cacheExpire = shopData?.expiration!
                        },
                        tk = new ClientMatchmakingTL()
                        {
                            states = new object[]
                            {
                                new
                                {
                                    validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    activeEvents = new string[0],
                                    state = new
                                    {
                                        k = new string[0]
                                    }
                                }
                            },
                            cacheExpire = shopData?.expiration!
                        },
                        CommunityVotes = new ClientMatchmakingTL()
                        {
                            states = new object[]
                            {
                                new
                                {
                                    validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    activeEvents = new string[0],
                                    state = new
                                    {
                                        electionId = "", // i want to look at this in the future
                                        candidates = new string[0],
                                        electionEnds = shopData?.expiration,
                                        numWinnners = 1
                                    }
                                }
                            },
                            cacheExpire = shopData?.expiration!

                        },
                        FeaturedIslands = new ClientMatchmakingTL()
                        {
                            states = new object[]
                                {
                                    new
                                    {
                                         validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                         activeEvents = new string[0],
                                         state = new
                                         {
                                             islandCodes = new string[0],
                                             playlistCuratedContent = new { },
                                             playlistCuratedHub = new { },
                                             islandTemplates = new string[0]
                                         }
                                    }
                                },
                            cacheExpire = shopData?.expiration!
                        },
                        ClientEvents = new ClientEventsTL()
                        {
                            states = new List<ClientEventsStates>
                            {
                                new ClientEventsStates
                                {
                                    validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                    activeEvents = new List<ActiveEventData>
                                    {
                                        new ActiveEventData
                                        {
                                            eventType = $"EventFlag.Season{season.Season}",
                                            activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                                            activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                        },
                                        new ActiveEventData
                                        {
                                            eventType = $"EventFlag.{LobbyBackground}",
                                            activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                                            activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                        }
                                    },
                                    state = new ClientEventsStatesState
                                    {
                                        activeStorefronts = new object[] { },
                                        eventNamedWeights = new object { },
                                        seasonNumber = season.Season,
                                        seasonTemplateId = $"AthenaSeason:athenaseason{season.Season}",
                                        matchXpBonusPoints = 0,
                                        seasonBegin = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        seasonEnd = Saved.DeserializeGameConfig.SeasonEndDate,
                                        seasonDisplayedEnd = Saved.DeserializeGameConfig.SeasonEndDate,
                                        weeklyStoreEnd =  shopData?.expiration!,
                                        stwEventStoreEnd = shopData?.expiration!,
                                        stwWeeklyStoreEnd = shopData?.expiration!,
                                        dailyStoreEnd =  shopData?.expiration!
                                    }
                                }
                            },
                            cacheExpire = shopData?.expiration!
                        }
                    },
                    eventsTimeOffsetHrs = 0,
                    cacheIntervalMins = 15,
                    currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };

                if (season.Season == 2)
                {
                    // this is for the 50v50 gamemode
                    Response.channels.ClientEvents.states[0].activeEvents.Add(new ActiveEventData
                    {
                        eventType = $"EventFlag.BR_Allow50v50",
                        activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                        activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    });

                    Response.channels.ClientEvents.states[0].activeEvents.Add(new ActiveEventData
                    {
                        eventType = $"EventFlag.SupplyDropGift",
                        activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                        activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    });

                    Response.channels.ClientEvents.states[0].activeEvents.Add(new ActiveEventData
                    {
                        eventType = $"EventFlag.WinterBattleBus",
                        activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                        activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    });

                    Response.channels.ClientEvents.states[0].activeEvents.Add(new ActiveEventData
                    {
                        eventType = $"EventFlag.BR.CandyCaneGuns",
                        activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                        activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    });

                    // removes like playlists on old ass seasons
                    Response.channels.ClientEvents.states[0].activeEvents.Add(new ActiveEventData
                    {
                        eventType = $"EventFlag.BR.DisallowSquad",
                        activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                        activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    });
                }

                // TODO ONLY SHOW THE EVENTS FOR THE ACTUAL SEASON... WILL SHOW ALL UNLESS YOU DONT HAVE IT IN THE TIMELINE FILE DUH

                foreach (var item in Saved.BackendCachedData.TimelineData.ClientEvents)
                {
                    Response.channels.ClientEvents.states[0].activeEvents.Add(item);
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Challenge();
        }
    }
}
