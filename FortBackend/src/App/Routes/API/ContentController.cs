using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers;
using FortLibrary.EpicResponses.FortniteServices.Content;
using FortLibrary.ConfigHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using ZstdSharp.Unsafe;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortBackend.src.App.Utilities.Constants;
using FortBackend.src.App.Utilities.Helpers.Cached;
using FortLibrary;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("content/api/pages/fortnite-game")]

    // I NEED TO REDO THIS AND MAKE IT SUPPORT OTHERS
    public class ContentController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<ContentJson>> ContentApi([FromServices] IMemoryCache memoryCache)
        {
            Response.ContentType = "application/json";
            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();
                var AcceptLanguage = Request.Headers["Accept-Language"].ToString();
                string season = "";
                season = (await SeasonUserAgent(Request)).Season.ToString();
                if (season == "10")
                {
                    season = "x";
                }

                if (string.IsNullOrEmpty(AcceptLanguage))
                {
                    AcceptLanguage = "en"; // weird
                }

                Console.WriteLine(AcceptLanguage);


                var cacheKey = $"ContentEndpointKey-{season}";
                if (memoryCache.TryGetValue(cacheKey, out ContentJson? cachedResult))
                {
                    if (cachedResult != null) { return cachedResult; }
                }



                var ContentJsonResponse = new ContentJson();
                if(NewsManager.ContentJsonResponse.TryGetValue(AcceptLanguage, out ContentJson Test)){
                    ContentJsonResponse = Test;
                }
                else
                {
                    ContentJsonResponse = NewsManager.ContentJsonResponse.FirstOrDefault(e => e.Key == "en").Value;
                }

                string LobbyBackground = $"season{season}";
                if (season == "2")
                {
                    LobbyBackground = "LobbyWinterDecor";
                }

                ContentJsonResponse.dynamicbackgrounds = new DynamicBackground()
                {
                    backgrounds = new DynamicBackgrounds()
                    {
                        backgrounds = new List<DynamicBackgroundList>
                        {
                            new DynamicBackgroundList
                            {
                                stage = LobbyBackground,
                                _type = "DynamicBackground",
                                key = "lobby"
                            }
                        }

                    }
                };
                //NewsManager



                memoryCache.Set(cacheKey, ContentJsonResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });

                //var jsonData1 = System.IO.File.ReadAllText(Path.Combine(PathConstants.BaseDir, $"src\\Resources\\Json\\ph.json"));
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
                Logger.Error(ex.Message);
            }
            return Ok(new { });
        }


        // not added the stupid cachcing to this
        [HttpPost("/api/v1/fortnite-br/surfaces/motd/target")]
        public async Task<ActionResult<ContentJson>> MOTDTARGET()
        {

            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();
                var AcceptLanguage = Request.Headers["Accept-Language"].ToString();

                if (string.IsNullOrEmpty(AcceptLanguage))
                {
                    AcceptLanguage = "en"; // weird
                }

                var ContentJsonResponse = new motdTarget();
                if (NewsManager.MotdJsonResponse.TryGetValue(AcceptLanguage, out motdTarget Test))
                {
                    ContentJsonResponse = Test;
                }
                else
                {
                    ContentJsonResponse = NewsManager.MotdJsonResponse.FirstOrDefault(e => e.Key == "en").Value;
                }

                var jsonResponse = JsonConvert.SerializeObject(ContentJsonResponse, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return Content(jsonResponse, "application/json");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "MOTDTARGET");
            }
            return Ok(new { });
        }
    }
}
