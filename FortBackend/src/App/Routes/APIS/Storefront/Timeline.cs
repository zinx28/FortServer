using FortBackend.src.App.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FortBackend.src.App.Routes.APIS.Storefront
{
    [ApiController]
    [Route("fortnite/api/calendar/v1/timeline")]
    public class TimelineApiController : ControllerBase
    {
        //class ShopData
        //{
        //    public string expiration { get; set; }
        //    public string cacheExpire { get; set; }
        //    public string 
        //}
        [HttpGet]
        public async Task<IActionResult> GrabTimeline()
        {
            Response.ContentType = "application/json";
            try
            {
                string Json = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/Json/shop/shop.json"));
                if(Json == null) {
                    return BadRequest(new {});
                }
                dynamic shopData = JsonConvert.DeserializeObject(Json);

                int season = await Grabber.SeasonUserAgent(Request);

                var Response = new
                {
                    channels = new Dictionary<string, object>
                    {
                        {
                            "client-matchmaking", new
                            {
                                states = new object[] { },
                                cacheExpire = $"{shopData.expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}"
                            }
                        },
                        {
                            "client-events", new
                            {
                                states = new object[]
                                {
                                    new
                                    {
                                        validFrom = "2019-01-21T18:36:38.383Z",
                                        activeEvents = new[]
                                        {
                                            new
                                            {
                                                eventType = $"EventFlag.Season{season}",
                                                activeUntil = "9999-12-31T23:59:59.999Z",
                                                activeSince = "2020-01-010T23:59:59.999Z"
                                            },
                                            new
                                            {
                                                eventType = $"EventFlag.LobbySeason{season}",
                                                activeUntil = "9999-12-31T23:59:59.999Z",
                                                activeSince = "2020-01-01T23:59:59.999Z"
                                            }
                                        },
                                        state = new
                                        {
                                            activeStorefronts = new object[] { },
                                            eventNamedWeights = new object { },
                                            seasonNumber = season,
                                            seasonTemplateId = $"AthenaSeason:athenaseason{season}",
                                            matchXpBonusPoints = 0,
                                            seasonBegin = "2020-01-01T00:00:00Z",
                                            seasonEnd = "2067-01-01T00:00:00Z",
                                            seasonDisplayedEnd = "2067-01-01T00:00:00Z",
                                            weeklyStoreEnd =  $"{shopData.expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}",
                                            stwEventStoreEnd = "9999-12-31T23:59:59.999Z",
                                            stwWeeklyStoreEnd = "9999-12-31T23:59:59.999Z",
                                            dailyStoreEnd =  $"{shopData.expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}"
                                        }
                                    }
                                },
                                cacheExpire = $"{shopData.cacheExpire.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}"
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
