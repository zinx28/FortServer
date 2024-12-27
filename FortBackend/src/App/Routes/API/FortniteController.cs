using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary;
using FortLibrary.MongoDB.Module;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text;

namespace FortBackend.src.App.Routes.API
{
    [ApiController]
    [Route("fortnite/api")]
    public class FortniteApiController : ControllerBase
    {
        [HttpPost("game/v2/profileToken/verify/{accountId}")]
        public IActionResult VerifyProfileToken(string accountId)
        {
            return StatusCode(204);
        }
        //fortnite/api/game/v2/grant_access
        [HttpPost("game/v2/grant_access/{accountId}")]
        public IActionResult GrantAccess(string accountId)
        {
            return StatusCode(204);
        }


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

        // Ones ik ig
        // founderfriendinvite
        // founderfriendinvite_xbox
        // mobileinvite ~ this is ios based
        [HttpGet("game/v2/friendcodes/{accountId}/epic")]
        public IActionResult FriendsCode(string accountId)
        {
            return Ok(new List<object>()
            {
                new
                {
                    codeId = "RANDOM",
                    codeType= "CodeToken:mobileinvite",
                    dateCreated = "2018-03-27T16:54:41.385Z"
                },

                new
                {
                    codeId = "TEST",
                    codeType= "CodeToken:founderfriendinvite_xbox",
                    dateCreated = "2018-03-27T16:54:41.385Z"
                },

                new
                {
                    codeId = "PENIS",
                    codeType= "CodeToken:founderfriendinvite",
                    dateCreated = "2018-03-27T16:54:41.385Z"
                }
            });
        }

        [HttpGet("versioncheck")]
        public IActionResult VersionCheck()
        {
            Response.ContentType = "application/json";

            return Ok(new
            {
                type = "NO_UPDATE"
            });
        }


        [HttpGet("v2/versioncheck/{version}")]
        public IActionResult VersionCheckV2(string version)
        {
            Response.ContentType = "application/json";

            return Ok(new
            {
                type = "NO_UPDATE"
            });
        }

        [HttpGet("game/v2/twitch/{accountId}")]
        public IActionResult TwitchAcc(string accountId)
        {
            Response.ContentType = "application/json";
            return StatusCode(203);
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

        [HttpGet("game/v2/world/info")]
        public IActionResult WorldInfo()
        {
            return Ok(new { });
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

        //fortnite/api/game/v2/tryPlayOnPlatform/account/

        [HttpPost("game/v2/tryPlayOnPlatform/account/{accountId}")]
        public IActionResult TryPlayOnPlatform(string accountId)
        {
            Response.ContentType = "text/plain";

            return Content("true");
        }

        //socialban/api/public/v1/
        [HttpGet("/socialban/api/public/v1/{accountId}")]
        public IActionResult SocialBan(string accountId)
        {
            Response.ContentType = "application/json";
            return Ok(new
            {
                bans = Array.Empty<string>(),
                warnings = Array.Empty<string>(),
            });
        }

        // gold
        [HttpGet("game/v2/br-inventory/account/{accountId}")]
        public async Task<IActionResult> Accinventory(string accountId)
        {
            Response.ContentType = "application/json";
            int globalcash = 0;
            try
            {
                var profileCacheEntry = await GrabData.Profile(accountId);
                if (profileCacheEntry != null)
                {
                    globalcash = profileCacheEntry.AccountData.athena.Gold;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "AccInventory");
            }

            return Ok(new
            {
                stash = new
                {
                    globalcash,
                }
            });
        }

        [HttpPost("feedback/{random}")]
        public IActionResult PostBug()
        {
            return Ok(new { });
        }
    }
}
