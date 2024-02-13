using FortBackend.src.App.Utilities.Classes.EpicResponses.Content;
using FortBackend.src.App.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using ZstdSharp.Unsafe;

namespace FortBackend.src.App.Routes.APIS.API
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
                string season = "2";

                season = (await Grabber.SeasonUserAgent(Request)).ToString();
                if (season == "10")
                {
                    season = "x";
                }

                var cacheKey = $"ContentEndpointKey-{season}";
                if (memoryCache.TryGetValue(cacheKey, out ContentJson cachedResult))
                {
                    return cachedResult;
                }

                var ResponseIG = new ContentJson
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
          
                Console.WriteLine("TEST");

                var jsonData = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\content.json"));
                ContentConfig contentconfig = JsonConvert.DeserializeObject<ContentConfig>(jsonData); //dynamicbackgrounds.news

                if(contentconfig != null)
                {
                    contentconfig.battleroyalenews.motds.ForEach(x =>
                    {
                        ResponseIG.battleroyalenews.news.motds.Add(new NewContentMotds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });

                        ResponseIG.battleroyalnewsv2.news.motds.Add(new NewContentV2Motds()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    contentconfig.battleroyalenews.messages.ForEach(x =>
                    {
                        ResponseIG.battleroyalenews.news.messages.Add(new NewContentMessages()
                        {
                            image = x.image,
                            title = x.title,
                            body = x.body,
                        });
                    });

                    contentconfig.emergencynotice.ForEach(x =>
                    {
                        ResponseIG.emergencynotice.news.messages.Add(new EmergencyNoticeNewsMessages()
                        {
                            title = x.title,
                            body = x.body,
                        });

                        ResponseIG.emergencynoticev2.emergencynotices.emergencynotices.Add(new EmergencyNoticeNewsV2Messages()
                        {
                            title = x.title,
                            body = x.body,
                        });
                    });
                }

                memoryCache.Set(cacheKey, ResponseIG, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                //var jsonData1 = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\ph.json"));
                //ContentJson contentconfig1 = JsonConvert.DeserializeObject<ContentJson>(jsonData); //dynamicbackgrounds.news

                //return Ok(contentconfig1);

                return ResponseIG;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new { });
            }

        }
    }
}
