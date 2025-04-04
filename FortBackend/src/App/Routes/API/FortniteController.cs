using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary;
using FortLibrary.Dynamics;
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

        [HttpPost("game/v2/toxicity/account/{accountId}/report/{reportID}")]
        [AuthorizeToken]
        public async Task<IActionResult> ReportUserTox(string accountId, string reportID)
        {
            try
            {
                Response.ContentType = "application/json";

                if(string.IsNullOrEmpty(Saved.DeserializeConfig.ReportsWebhookUrl))
                    return Ok(new { });

                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    string RequestBody = await reader.ReadToEndAsync();
                    if (string.IsNullOrEmpty(RequestBody)) return Ok(new { }); // uhm?
                    Console.WriteLine(RequestBody);
                    //ReportUserClass
                    ReportUserClass reportUserClass = JsonConvert.DeserializeObject<ReportUserClass>(RequestBody)!;


                    ProfileCacheEntry profileCacheEntry = await GrabData.Profile(accountId);

                    if (!string.IsNullOrEmpty(profileCacheEntry.AccountId))
                    {
                        ProfileCacheEntry reportCacheEntry = await GrabData.Profile(reportID);
                        if (string.IsNullOrEmpty(reportCacheEntry.AccountId))
                            return Ok(new { });

                        var embed = new
                        {
                            title = "Reporter:",
                            fields = new[]
                            {
                                new
                                {
                                    name = "Reason",
                                    value = $"{reportUserClass.reason ?? "Unknown - Please Resend"}",
                                    inline = true
                                },
                                new
                                {
                                    name = "Choosing To Report",
                                    value = reportCacheEntry.UserData.Username ?? "uhm?",
                                    inline = true
                                },
                                new
                                {
                                    name = "Details",
                                    value = reportUserClass.details,
                                    inline = false
                                },
                                new
                                {
                                    name = "Game Id",
                                    value = reportUserClass.gameSessionId ?? "GameSessionID null",
                                    inline = true
                                },
                                new
                                {
                                    name = "Token",
                                    value = reportUserClass.token ?? "Token null!",
                                    inline = true
                                },
                                new
                                {
                                    name = "Playlist ID",
                                    value = reportUserClass.playlistName,
                                    inline = true
                                },
                            },
                            footer = new { text = $"This was sent by {profileCacheEntry.UserData.Username}" },
                            color = 0x00FFFF
                        };

                        string jsonEmbed = JsonConvert.SerializeObject(embed, Formatting.Indented);
                        string jsonPayload1 = JsonConvert.SerializeObject(new { embeds = new[] { embed } });

                        using (var httpClient = new HttpClient())
                        {
                            HttpContent httpContent1 = new StringContent(jsonPayload1, Encoding.UTF8, "application/json");
                            try
                            {
                                HttpResponseMessage response32 = await httpClient.PostAsync(Saved.DeserializeConfig.ReportsWebhookUrl, httpContent1);
                                if (response32.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("Message sent successfully!");
                                }
                                else
                                {
                                    Console.WriteLine($"Failed to send message. Status code: {response32.StatusCode}");
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                Console.WriteLine($"Error sending request: {ex.Message}");
                            }
                            //}

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "ReportUserTox");
            }

            return Ok(new { });
        }

    }
}
