using FortBackend.src.App.Utilities.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FortBackend.src.App.Routes.APIS.API
{
    [ApiController]
    [Route("fortnite/api")]
    public class FortniteApiController : ControllerBase
    {
        [HttpGet("receipts/v1/account/{accountId}/receipts")]
        public IActionResult Receipts(string accountId)
        {   
            return Ok(new List<object>()
            {
                new
                {
                  appStore = "EpicPurchasingService",
                  appStoreId = "69",
                  receiptId = "69",
                  receiptInfo = "ENTITLEMENT"
                }
            });
        }


        [HttpGet("v2/versioncheck/{version}")]
        public IActionResult CheckToken(string version)
        {
            Response.ContentType = "application/json";

            return Ok(new
            {
                type = "NO_UPDATE"
            });
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

        [HttpGet("game/v2/privacy/account/{accountId}")]
        public IActionResult PrivacyAcc(string accountId)
        {
            Response.ContentType = "application/json";

            return Ok(new
            {
                accountId,
                optOutOfPublicLeaderboards = false
            });
        }
    }
}
