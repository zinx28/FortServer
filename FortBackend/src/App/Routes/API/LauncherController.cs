using Microsoft.AspNetCore.Mvc;

namespace FortBackend.src.App.Routes.API
{
    //launcher/api/public/distributionpoints
    [ApiController]
    [Route("launcher/api")]
    public class LauncherController : ControllerBase
    {
        [HttpGet("public/distributionpoints")]
        public IActionResult DistributionPoints()
        {
            return Ok(new
            {
                  distributions = new string[]
                  {
                    "https://download.epicgames.com/",
                    "https://download2.epicgames.com/",
                    "https://download3.epicgames.com/",
                    "https://download4.epicgames.com/",
                    "https://epicgames-download1.akamaized.net/",
                    "https://fastly-download.epicgames.com/"
                  }
            });
        }
    }
}
