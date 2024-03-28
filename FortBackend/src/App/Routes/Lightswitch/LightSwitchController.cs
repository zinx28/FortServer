using FortBackend.src.App.Utilities.Classes.EpicResponses;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Lightswitch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.Lightswitch
{
    [ApiController]
    [Route("lightswitch/api/service/bulk/status")]
    public class LightSwitchApiController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<LightSwitchData>> LightSwitch()
        {
            Response.ContentType = "application/json";

            var lightSwitchData = new List<LightSwitchData>
            {
                new LightSwitchData()
            };

            return Ok(lightSwitchData);
        }
    }
}
