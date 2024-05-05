using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Saved;
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

            
                var Response = new
                {
                    channels = new Dictionary<string, object>
                    {
                        {
                            "client-matchmaking", new
                            {
                                states = new object[] { },
                                cacheExpire = shopData?.expiration
                            }
                        },
                        {
                            "standalone-store", new
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
                                cacheExpire = shopData?.expiration
                            }
                        },
                        {
                            "tk", new
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
                                cacheExpire = shopData?.expiration
                            }
                        },
                        {
                            "community-votes", new
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
                                cacheExpire = shopData?.expiration
                            }
                        },
                        {
                            "featured-islands", new
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
                                cacheExpire = shopData?.expiration
                            }
                        },
                        {
                            "client-events", new
                            {
                                states = new object[]
                                {
                                    new
                                    {
                                        validFrom = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        activeEvents = new[]
                                        {
                                            new
                                            {
                                                eventType = $"EventFlag.Season{season.Season}",
                                                activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                                                activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                            },
                                            new
                                            {
                                                eventType = $"EventFlag.LobbySeason{season.Season}",
                                                activeUntil = Saved.DeserializeGameConfig.SeasonEndDate,
                                                activeSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                            }
                                        },
                                        state = new
                                        {
                                            activeStorefronts = new object[] { },
                                            eventNamedWeights = new object { },
                                            seasonNumber = season.Season,
                                            seasonTemplateId = $"AthenaSeason:athenaseason{season.Season}",
                                            matchXpBonusPoints = 0,
                                            seasonBegin = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                            seasonEnd = Saved.DeserializeGameConfig.SeasonEndDate,
                                            seasonDisplayedEnd = Saved.DeserializeGameConfig.SeasonEndDate,
                                            weeklyStoreEnd =  shopData?.expiration,
                                            stwEventStoreEnd = shopData?.expiration,
                                            stwWeeklyStoreEnd = shopData?.expiration,
                                            dailyStoreEnd =  shopData?.expiration
                                        }
                                    }
                                },
                                cacheExpire = shopData?.expiration
                            }
                        }
                    },
                    eventsTimeOffsetHrs = 0,
                    cacheIntervalMins = 15,
                    currentTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                };
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Challenge();
            }
        }
    }
}
