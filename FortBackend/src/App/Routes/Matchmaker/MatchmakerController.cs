using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.MongoDB.Module;
using System.IdentityModel.Tokens.Jwt;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities;
using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Matchmaker;
using FortLibrary.ConfigHelpers;
using FortLibrary;

namespace FortBackend.src.App.Routes.Matchmaker
{
    [ApiController]
    [Route("fortnite/api")]
    public class MatchmakerController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();


        [HttpGet("matchmaking/session/findPlayer/{accountId}")]
        public IActionResult FindPlayer(string accountId)
        {
            return StatusCode(200);
        }

        [HttpGet("matchmaking/session/{sessionId}")]
        public async Task<IActionResult> MatchmakerSession(string sessionId)
        {
            Response.ContentType = "application/json";
            try
            {
                FortConfig config = Saved.DeserializeConfig;
                CachedDataClass BackendCachedData = Saved.BackendCachedData;

                string JsonContent = ""; // no idea
                if (!config.CustomMatchmaker)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync($"{BackendCachedData.DefaultProtocol}{config.MatchmakerIP}:{config.MatchmakerPort}/v1/devers/servers");
                        response.EnsureSuccessStatusCode();
                        JsonContent = await response.Content.ReadAsStringAsync(); // just guess if its on the same server
                        if (JsonContent == null)
                        {
                            return Ok(new { });
                        }
                    }
                    catch (WebException ex)
                    {
                        Logger.Error(ex.Message, "MatchmakerSession");
                        return Ok(new { });
                    }
                }

                MMTicket jsonObject = new MMTicket();
                if (!string.IsNullOrEmpty(JsonContent)) {
                    List<Server> servers = JsonConvert.DeserializeObject<List<Server>>(JsonContent)!;
                    if (servers != null && servers.Count > 0)
                    {
                        var filteredServers = servers.Where(server => server.Session == sessionId).ToList();

                        if (filteredServers.Any())
                        {
                            jsonObject = new MMTicket
                            {
                                SessionId = sessionId,
                                OwnerId = Guid.NewGuid().ToString("N").ToUpper(),
                                OwnerName = filteredServers[0].Name,
                                ServerName = filteredServers[0].Name,
                                ServerAddress = filteredServers[0].Ip,
                                ServerPort = filteredServers[0].Port,
                                MaxPublicPlayers = filteredServers[0].MaxPlayers,
                                Attributes = new MMTicketAttributes
                                {
                                    Region = filteredServers[0].Region,
                                    PlaylistName = filteredServers[0].Playlist,
                                    SessionKey = Guid.NewGuid().ToString("N").ToUpper()
                                },
                                TotalPlayers = filteredServers[0].Current,
                                AllowJoinInProgress = filteredServers[0].JoinAble,
                                AllowInvites = filteredServers[0].JoinAble,
                                BuildUniqueId = Request.Cookies["buildUniqueId"] ?? "0",
                                LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                            };
                        }
                    }
                }
                else
                {
                    if (config.CustomMatchmaker)
                    {
                        jsonObject = new MMTicket
                        {
                            SessionId = sessionId,
                            OwnerId = Guid.NewGuid().ToString("N").ToUpper(),
                            ServerAddress = config.GameServerIP,
                            ServerPort = config.GameServerPort,
                            MaxPublicPlayers = 100,
                            Attributes = new MMTicketAttributes
                            {
                                SessionKey = Guid.NewGuid().ToString("N").ToUpper()
                            },
                            BuildUniqueId = Request.Cookies["buildUniqueId"] ?? "0",
                            LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        };
                    }
                }



                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(jsonObject));


                string json = System.Text.Json.JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions
                {
                    WriteIndented = true
                });



                return Content(json);
            }
            catch (Exception ex)
            {
                Console.Write("TEST " + ex.Message);
            }
            return Ok(new { });
        }

        [HttpPost("matchmaking/session")]
        public async Task<IActionResult> CreateSession()
        {
            Response.ContentType = "application/json";
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];
                var accessToken = token.Replace("eg1~", "");

                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(accessToken);
                string[] tokenParts = decodedToken.ToString().Split('.');

                if (tokenParts.Length == 2)
                {
                    var payloadJson = tokenParts[1];
                    if (string.IsNullOrEmpty(payloadJson))
                    {
                        dynamic payload = JsonConvert.DeserializeObject(payloadJson)!;
                        Console.WriteLine(payload);
                        if (payload == null)
                        {
                            return BadRequest(new { });
                        }

                        var displayName = payload.dn;
                        var AccountId = payload.sub;
                        var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                        var AccountData = await Handlers.FindOne<Account>("accountId", AccountId);
                        var UserData = await Handlers.FindOne<User>("accountId", AccountId);

                        if (AccountData != "Error" && UserData != "Error")
                        {
                            Account AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(AccountData)![0];
                            User UserDataParsed = JsonConvert.DeserializeObject<User[]>(UserData)![0];

                            if (AccountDataParsed != null && UserDataParsed != null)
                            {
                                if (AccountDataParsed != null && AccountData.ToString().Contains(accessToken))
                                {
                                    if (UserDataParsed.banned)
                                    {
                                        return Ok(new { });
                                    }
                                }
                            }
                        }
                    }
                }
                return Ok(new { });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Newest Api No way " + ex.Message);
                return Ok(new { });
            }
        }

        [HttpGet("game/v2/matchmakingservice/ticket/player/{accountId}")]
        public async Task<IActionResult> MatchmakeTicket(string accountId, [FromQuery] string bucketId)
        {
            Response.ContentType = "application/json";
            try
            {
                FortConfig config = Saved.DeserializeConfig;
                CachedDataClass BackendCachedData = Saved.BackendCachedData;

                var token = Request.Headers["Authorization"].ToString().Split("bearer ")[1];
                var accessToken = token.Replace("eg1~", "");

                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(accessToken);
                string[] tokenParts = decodedToken.ToString().Split('.');

                if (tokenParts.Length == 2)
                {
                    var payloadJson = tokenParts[1];
                    if (!string.IsNullOrEmpty(payloadJson))
                    {
                        dynamic payload = JsonConvert.DeserializeObject(payloadJson)!;

                        if (payload == null)
                        {
                            return BadRequest(new { });
                        }

                        var displayName = payload.dn;
                        var AccountId = payload.sub;
                        var clientId = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "clid")?.Value;

                        if (string.IsNullOrEmpty(AccountId.ToString()))
                        {
                            Logger.Error("ACCOUNT ID IS NULL AND WILL NOT GO THROUGH WITH THE REUQEST", "MATCHMKAER");
                            return BadRequest(new { });
                        }


                        ProfileCacheEntry profileCacheEntry = await GrabData.Profile(AccountId.ToString());
                        if (profileCacheEntry != null && profileCacheEntry.UserData != null && !string.IsNullOrEmpty(profileCacheEntry.AccountId))
                        {
                            if (profileCacheEntry.AccountData != null && profileCacheEntry.UserData != null)
                            {
                                //if (AccountData.ToString().Contains(accessToken))
                               // {
                                    if (profileCacheEntry.UserData.banned)
                                    {
                                        return Ok(new { });
                                    }
                                //}
                            }
                            

                            CookieOptions cookieOptions = new CookieOptions
                            {
                            };

                            if (string.IsNullOrEmpty(bucketId) || !bucketId.GetType().Equals(typeof(string)))
                            {
                                return BadRequest();
                            }

                            string[] bucketIdParts = bucketId.Split(':');
                            if (bucketIdParts.Length != 4)
                            {
                                return BadRequest();
                            }

                            string region = bucketIdParts[2];
                            string playlist = bucketIdParts[3];
                            string BuildId = bucketIdParts[0];
                            var customcode = "";

                            try { customcode = HttpContext.Request.Query["player.option.customKey"]; } catch { }

                            Response.Cookies.Append("buildUniqueId", BuildId, cookieOptions);

                            var jsonObject = new MatchmakerTicket
                            {
                                accountId = accountId,
                                BuildId = BuildId,
                                Playlist = playlist,
                                Region = region,
                                CustomKey = customcode ?? "NONE",
                                AccessToken = profileCacheEntry.UserData.accesstoken,
                                Priority = false,
                                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                            };

                            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);
                            string encryptedData = GenerateAES.EncryptAES256(jsonData, config.JWTKEY);
                            string WssConnection = BackendCachedData.DefaultProtocol == "http://" ? "ws://" : "wss://";

                            Console.WriteLine(new
                            {
                                serviceUrl = $"{WssConnection}{config.MatchmakerIP}:{config.MatchmakerPort}",
                                ticketType = "mms-player",
                                payload = "account",
                                signature = encryptedData
                            }); // remove please

                            return Ok(new
                            {
                                serviceUrl = $"{WssConnection}{config.MatchmakerIP}:{config.MatchmakerPort}",
                                ticketType = "mms-player",
                                payload = "account",
                                signature = encryptedData
                            });
                        }
                    }
                }

                return Ok(new { });
            }
            catch (Exception ex)
            {
                Logger.Error($"ERROR IN MATCHMAKER!!! -> {ex.Message}", "MATCHMAKER");
                return Ok(new { });
            }
        }

        [HttpGet("game/v2/matchmaking/account/{accountId}/session/{sessionId}")]
        public IActionResult MatchmakeSession(string accountId, string sessionId)
        {
            Response.ContentType = "application/json";
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(sessionId))
            {
                return BadRequest();
            }

            return Ok(new
            {
                accountId,
                sessionId,
                key = "none"
            });

        }

        [HttpPost("matchmaking/session/{sessionId}/join")]
        public IActionResult MatchmakeJoinSession(string sessionId)
        {
            //try
            //{


                return StatusCode(204);
            //}
            //catch (Exception ex)
            //{

            //}

         //   return StatusCode(403);
        }

        [HttpPost("matchmaking/session/{sessionId}/players")]
        public IActionResult SessionPlayers(string sessionId)
        {
            return StatusCode(204);
        }
    }
}
