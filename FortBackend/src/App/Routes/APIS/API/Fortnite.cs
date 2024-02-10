using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.API
{
    [ApiController]
    [Route("fortnite/api")]
    public class FortniteApiController : ControllerBase
    {
        private IMongoDatabase _database;

        public FortniteApiController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpGet("v2/versioncheck/{version}")]
        public IActionResult CheckToken(string version)
        {
            Response.ContentType = "application/json";
            var omg = new
            {
                type = "NO_UPDATE"
            };

            return Ok(omg);
        }

        [HttpGet("game/v2/enabled_features")]
        public IActionResult enabled_features()
        {
            Response.ContentType = "application/json";
            return Ok(Array.Empty<string>());
        }

        [HttpGet("storeaccess/v1/request_access/{accountId}")]
        public IActionResult request_access()
        {
            Response.ContentType = "application/json";
            return Ok();
        }
    }
}
