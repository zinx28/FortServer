using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.FortniteServices.Content;
using FortBackend.src.App.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using ZstdSharp.Unsafe;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("content/api/pages/fortnite-game")]
    public class ContentController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<ContentJson>> ContentApi([FromServices] IMemoryCache memoryCache)
        {
            Response.ContentType = "application/json";
            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();
                string season = "";
                season = (await SeasonUserAgent(Request)).Season.ToString();
                if (season == "10")
                {
                    season = "x";
                }

                var cacheKey = $"ContentEndpointKey-{season}";
                if (memoryCache.TryGetValue(cacheKey, out ContentJson? cachedResult))
                {
                    if(cachedResult != null) { return cachedResult; }  
                }

                var ContentJsonResponse = new ContentJson
                {

                    dynamicbackgrounds = new DynamicBackground()
                    {
                        backgrounds = new DynamicBackgrounds()
                        {
                            backgrounds = new List<DynamicBackgroundList>
                            {
                                new DynamicBackgroundList
                                {
                                    stage = $"season{season}",
                                    _type = "DynamicBackground",
                                    key = "lobby"
                                }
                            }

                        }
                    }
                };

                var jsonData = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\content.json"));
                ContentConfig contentconfig = JsonConvert.DeserializeObject<ContentConfig>(jsonData); //dynamicbackgrounds.news

                if (contentconfig != null)
                {
                    contentconfig.battleroyalenews.motds.ForEach(x =>
                    {
                        ContentJsonResponse.battleroyalenews.news.motds.Add(new NewContentMotds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });

                        ContentJsonResponse.battleroyalnewsv2.news.motds.Add(new NewContentV2Motds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    contentconfig.battleroyalenews.messages.ForEach(x =>
                    {
                        ContentJsonResponse.battleroyalenews.news.messages.Add(new NewContentMessages()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    contentconfig.emergencynotice.ForEach(x =>
                    {
                        ContentJsonResponse.emergencynotice.news.messages.Add(new EmergencyNoticeNewsMessages()
                        {
                            title = x.title,
                            body = x.body,
                        });

                        ContentJsonResponse.emergencynoticev2.emergencynotices.emergencynotices.Add(new EmergencyNoticeNewsV2Messages()
                        {
                            title = x.title,
                            body = x.body,
                        });
                    });

                    contentconfig.shopSections.ForEach(x =>
                    {
                        ContentJsonResponse.shopSections.sectionList.sections.Add(new ShopSectionsSectionsSEctions
                        {
                            sectionId = x.sectionId,
                            sectionDisplayName = x.sectionDisplayName,
                            landingPriority = x.landingPriority,
                        });
                    });

                    contentconfig.tournamentinformation.ForEach(x =>
                    {
                        ContentJsonResponse.tournamentinformation.tournament_info.tournaments.Add(x);
                    });

                    contentconfig.playlistinformation.ForEach(x =>
                    {
                        ContentJsonResponse.playlistinformation.playlist_info.playlists.Add(x);
                    });
                }

                memoryCache.Set(cacheKey, ContentJsonResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                //var jsonData1 = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\ph.json"));
                //ContentJson contentconfig1 = JsonConvert.DeserializeObject<ContentJson>(jsonData); //dynamicbackgrounds.news

                //return Ok(contentconfig1);
                var jsonResponse = JsonConvert.SerializeObject(ContentJsonResponse, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new { });
            }

        }
    }
}
