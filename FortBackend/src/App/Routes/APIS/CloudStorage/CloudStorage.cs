using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.CloudStorage
{
    [ApiController]
    [Route("fortnite/api/cloudstorage")]
    public class CloudStorageApiController : ControllerBase
    {
        private IMongoDatabase _database;

        public CloudStorageApiController(IMongoDatabase database)
        {
            _database = database;
        }

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
