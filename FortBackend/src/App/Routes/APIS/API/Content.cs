using FortBackend.src.App.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace FortBackend.src.App.Routes.APIS.API
{
    [ApiController]
    [Route("content/api/pages/fortnite-game")]
    public class ContentController : ControllerBase
    {
        class ContentJson
        {
            public List<Dictionary<string, object>> emergencynotice { get; set; }
            public List<Dictionary<string, object>> news { get; set; }
            public List<Dictionary<string, object>> dynamicbackground { get; set; }
            public object tournamentinformation { get; set; } = new
            {
                containerName = new Dictionary<string, object>()
            };
        }
        [HttpGet]
        public async Task<IActionResult> ContentApi()
        {
            Response.ContentType = "application/json";
            try
            {
                var userAgent = Request.Headers["User-Agent"].ToString();
                string season = "2";

                season = (await Grabber.SeasonUserAgent(Request)).ToString();
                if(season == "10")
                {
                    season = "x";
                }
                dynamic jsonData = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"src\\Resources\\Json\\content.json"));
                dynamic test = JsonObject.Parse(jsonData); //dynamicbackgrounds.news

                Console.WriteLine(test["dynamicbackgrounds"]["backgrounds"]["backgrounds"].ToString());
                test["dynamicbackgrounds"]["backgrounds"]["backgrounds"] = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(new[] {
                        new
                        {
                            stage = $"season{season}",
                            _type = "DynamicBackground",
                            key = "lobby"
                        }
                }));
                return Ok(test);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok(new { });
            }

        }
    }
}
