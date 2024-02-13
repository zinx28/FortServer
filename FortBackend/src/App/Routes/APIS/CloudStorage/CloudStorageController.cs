using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.CloudStorage
{
    [ApiController]
    [Route("fortnite/api/cloudstorage")]
    public class CloudStorageApiController : ControllerBase
    {
        [HttpGet("system")]
        public IActionResult SystemApi()
        {
            return Ok(new List<object>());
        }

        [HttpGet("system/config")]
        public IActionResult SytemConfigApi()
        {
            return Ok(new { });
        }
    }
}
