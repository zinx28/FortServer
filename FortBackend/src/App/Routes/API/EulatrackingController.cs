using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("eulatracking/api/shared/agreements/fn")]
    public class EulatrackingController : ControllerBase
    {
        [HttpGet]
        public IActionResult StatsProxy(string accountId)
        {
            return Ok(new {});
        }

    }
}
