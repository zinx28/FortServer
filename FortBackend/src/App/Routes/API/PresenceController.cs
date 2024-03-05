using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("presence/api/v1")]
    public class PresenceController : ControllerBase
    {
        [HttpGet("_/{accountId}/settings/subscriptions")]
        public async Task<ActionResult> GrabSettings(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("_/{accountId}/last-online")]
        public async Task<ActionResult> GrabLastOnline(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("_/{accountId}/subscriptions")]
        public async Task<ActionResult> SubWow(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("_/{accountId}/subscriptions/nudged")]
        public async Task<ActionResult> GrabNudged(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }


        [HttpPost("_/{accountId}/subscriptions/broadcast")]
        public async Task<ActionResult> Subscriptionsbroadcast(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }
    }
}
