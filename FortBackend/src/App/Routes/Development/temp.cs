using Microsoft.AspNetCore.Mvc;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;
using System.Text;
using MongoDB.Driver;
using FortBackend.src.App.Utilities.Classes;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using System.Text.RegularExpressions;

namespace FortBackend.src.App.Routes.Development
{
    [ApiController]
    [Route("temp")]
    public class TempController : ControllerBase
    {
        private IMongoDatabase _database;
        public TempController(IMongoDatabase database)
        {
            _database = database;
        }
        [HttpGet("yeah69")]
        public IActionResult YeahImsoGayyy()
        {
            return Ok(new { });
        }
       

      /* [HttpGet("/skin/{accountId}")]
        public async Task<IActionResult> ReturnPlayerSkin(string accountId)
        {
            try
            {


            }
            catch (Exception ex)
            {

            }
        }*/

        [HttpGet("/callback")]
        public async Task<IActionResult> CallBack([FromQuery] string code)
        {
            try
            {
                Config config = Saved.DeserializeConfig;

                if (string.IsNullOrEmpty(config.ApplicationClientID) || string.IsNullOrEmpty(config.ApplicationURI) || string.IsNullOrEmpty(config.ApplicationSecret))
                {
                    return Ok(new { test = "Blank Application Info" });
                }
                var Client = new HttpClient();
                var formData = new Dictionary<string, string>()
                {
                    { "client_id", config.ApplicationClientID },
                    { "client_secret", config.ApplicationSecret },
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", config.ApplicationURI }
                };
                var content = new FormUrlEncodedContent(formData);
                var response = await Client.PostAsync("https://discord.com/api/oauth2/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
                if (responseData == null)
                {
                    return Ok(new { test = "Null!" });
                }

                if (responseData.TryGetValue("access_token", out var accessToken))
                {


                    var client2 = new HttpClient();
                    client2.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    var response2 = await client2.GetAsync("https://discord.com/api/users/@me/guilds");
                    var responseContent2 = await response2.Content.ReadAsStringAsync();


                    List<Server> responseData2 = JsonConvert.DeserializeObject<List<Server>>(responseContent2);
                    bool IsInServer = false;
                    foreach (Server item in responseData2)
                    {
                        //Console.WriteLine(item.id);
                        if (item.id == Saved.DeserializeConfig.ServerID)
                        {
                            IsInServer = true;
                        }
                    }
                    HttpContext httpContext = HttpContext;
                    if (httpContext == null)
                    {
                        return Ok(new { test = "Context is null" });
                    }

                    if (IsInServer)
                    {
                        // Only call this api if they are in the server
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                        response = await client.GetAsync("https://discord.com/api/users/@me");
                        responseContent = await response.Content.ReadAsStringAsync();

                        if (responseContent == null)
                        {
                            return Ok(new { test = "Server Sided Error" });
                        }
                        //Console.WriteLine(responseContent);
                        UserInfo responseData1 = JsonConvert.DeserializeObject<UserInfo>(responseContent);

                        if (responseData1 == null)
                        {
                            return Ok(new { test = "Server Sided Error -> 2" });
                        }
                        var username = responseData1.username;
                        var GlobalName = responseData1.global_name;
                        var id = responseData1.id;
                        var email = responseData1.email;

                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(id))
                        {
                            return Ok(new { test = "why is the response wrong" });
                        }

                        var FindDiscordID = await Handlers.FindOne<User>("DiscordId", id);
                        if (FindDiscordID != "Error")
                        {
                            string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");

                            var UpdateResponse = await Handlers.UpdateOne<User>("DiscordId", id, new Dictionary<string, object>()
                            {
                                { "accesstoken", NewAccessToken }
                            });

                            if (UpdateResponse != "Error")
                            {
                                User UserData = JsonConvert.DeserializeObject<User[]>(FindDiscordID)?[0];
                                if (UserData != null)
                                {
                                    if (UserData.banned)
                                    {
                                        return Ok(new { test = "Banned" });
                                    }

                                    Console.WriteLine("TEST");
                                    string[] UserIp = new string[] { httpContext.Connection.RemoteIpAddress?.ToString() };

                                    IMongoCollection<StoreInfo> StoreInfocollection = _database.GetCollection<StoreInfo>("StoreInfo");
                                    var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIps, httpContext.Connection.RemoteIpAddress?.ToString());
                                    var count = await StoreInfocollection.CountDocumentsAsync(filter);
                                    bool BanUser = false;
                                    if (count > 0)
                                    {
                                        BanUser = true;

                                        var update = Builders<StoreInfo>.Update
                                            .PushEach(e => e.UserIds, new[] { UserData.AccountId });

                                        var updateResult = await StoreInfocollection.UpdateManyAsync(filter, update);
                                        Console.WriteLine("TEST1");
                                        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                                        {
                                            Console.WriteLine("TEST2");
                                            Console.WriteLine(responseData1.id);
                                            await BanAndWebHooks.Init(Saved.DeserializeConfig, responseData1);

                                            await Handlers.UpdateOne<User>("DiscordId", UserData.DiscordId, new Dictionary<string, object>()
                                            {
                                               { "banned", true }
                                            });

                                            return Ok(new { test = "Banned" });
                                        }
                                    }else
                                    {
                                        if (!UserData.UserIps.Contains(UserIp[0]))
                                        {
                                            await Handlers.PushOne<User>("DiscordId", id, new Dictionary<string, object>()
                                            {
                                                { "UserIps", httpContext.Connection.RemoteIpAddress?.ToString() }
                                            }, false);
                                        }
                                    }

                                    return Redirect("http://127.0.0.1:2158/callback?code=" + NewAccessToken);
                                }
                            }
                            else
                            {
                                return Ok(new { test = "error!! couldnt login" });
                            }
                        }
                        else
                        {
                            var FindUserId = await Handlers.FindOne<User>("username", GlobalName);
                            if (FindUserId != "Error")
                            {
                                FindUserId = await Handlers.FindOne<User>("username", GlobalName);
                                if (FindUserId != "Error")
                                {
                                    // Create the account
                                    return Ok(new { test = "username is in use but lkets make you a acc" });
                                }
                                else
                                {
                                    string NewAccessToken = await CreateAccount.Init(httpContext, _database, responseData1, true);

                                    return Redirect("http://127.0.0.1:2158/callback?code=" + NewAccessToken);
                                }
                            }
                            else
                            {
                                string NewAccessToken = await CreateAccount.Init(httpContext, _database, responseData1);
                                
                                return Redirect("http://127.0.0.1:2158/callback?code=" + NewAccessToken);
                            }
                        }
                    }
                    else
                    {
                        // user is not in the server!

                        return Ok(new { test = "User is not in discord server!" });
                    }


                    //https://discord.com/api/users/@me
                    //Console.WriteLine(responseContent);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { test = "Unknown Issue!" });
        }

        [HttpGet("/image/{image}")]
        public async Task<IActionResult> ImageEnd(string image)
        {
            try
            {
                // Prevent Weird Characters
                if (!Regex.IsMatch(image, "^[a-zA-Z\\-\\._]+$"))
                {
                    return BadRequest("Invalid image parameter");
                }
                var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src/Resources/Image", image);
                if (System.IO.File.Exists(imagePath))
                {
                    return PhysicalFile(imagePath, "image/jpeg");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
