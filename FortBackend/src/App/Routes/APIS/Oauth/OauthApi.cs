using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.Oauth
{
    [ApiController]
    [Route("account/api")]
    public class OauthApiController : ControllerBase
    {
        private IMongoDatabase _database;

        public OauthApiController(IMongoDatabase database)
        {
            _database = database;
        }
    }
}
