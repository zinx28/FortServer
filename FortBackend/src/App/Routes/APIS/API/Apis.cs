using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.API
{
    [ApiController]
    [Route("api")]
    public class ApisController : ControllerBase
    {
        private IMongoDatabase _database;

        public ApisController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpPost("/datarouter/api/v1/public/data")]
        public IActionResult DataRouter()
        {
            return Ok();
        }
    }
}
