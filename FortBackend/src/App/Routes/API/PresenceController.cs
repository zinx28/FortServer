using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("presence/api/v1")]
    public class PresenceController : ControllerBase
    {
        [HttpGet("{_}/{accountId}/settings/subscriptions")]
        public ActionResult GrabSettings(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("{_}/{accountId}/last-online")]
        public ActionResult GrabLastOnline(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("{_}/{accountId}/subscriptions")]
        public ActionResult SubWow(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("{_}/{accountId}/subscriptions/nudged")]
        public ActionResult GrabNudged(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }


        [HttpPost("{accountId}/subscriptions/broadcast")]
        public ActionResult Subscriptionsbroadcast(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }
    }
}
