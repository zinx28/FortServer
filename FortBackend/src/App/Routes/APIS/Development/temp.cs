using Microsoft.AspNetCore.Mvc;
using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using System.Collections.Generic;
using FortBackend.src.App.Routes.Classes;
using static FortBackend.src.App.Routes.Classes.DiscordAuth;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System;
using FortBackend.src.App.Utilities.Helpers;
using MongoDB.Driver;
using Discord;

namespace FortBackend.src.App.Routes.APIS.Development
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
        private static Random random = new Random();
        private static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder randomString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }
            return randomString.ToString();
        }

        [HttpGet("/callback")]
        public async Task<IActionResult> CallBack([FromQuery] string code)
        {
            try
            {
                Config config = Saved.DeserializeConfig;

                if(string.IsNullOrEmpty(config.ApplicationClientID) || string.IsNullOrEmpty(config.ApplicationURI) || string.IsNullOrEmpty(config.ApplicationSecret))
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
                if(responseData == null)
                {
                    return Ok(new { test = "Null!" });
                }

                if (responseData.TryGetValue("access_token", out var accessToken))
                {
                  

                    var client2 = new HttpClient();
                    client2.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    var response2 = await client2.GetAsync("https://discord.com/api/users/@me/guilds");
                    var responseContent2 = await response2.Content.ReadAsStringAsync();

                  
                    List<DiscordAuth.Server> responseData2 = JsonConvert.DeserializeObject<List<DiscordAuth.Server>>(responseContent2);
                    bool IsInServer = false;
                    foreach (DiscordAuth.Server item in responseData2)
                    {
                        //Console.WriteLine(item);
                        if (item.id == "1204461240882307123")
                        {
                            //Console.WriteLine("IS IN CHAPTER DEV SERVER");
                            IsInServer = true;
                        }
                    }
                    if (IsInServer)
                    {
                        // Only call this api if they are in the server
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                        response = await client.GetAsync("https://discord.com/api/users/@me");
                        responseContent = await response.Content.ReadAsStringAsync();

                        if(responseContent == null)
                        {
                            return Ok(new { test = "Server Sided Error" });
                        }

                        UserInfo responseData1 = JsonConvert.DeserializeObject<UserInfo>(responseContent);

                        var username = responseData1.username;
                        var id = responseData1.id;
                        var email = responseData1.email;

                        if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(id))
                        {
                            return Ok(new { test = "why is the response wrong" });
                        }

                        var FindDiscordID = await Handlers.FindOne<User>("DiscordId", id);
                        if (FindDiscordID != "Error")
                        {
                            // returns the users token or what not

                            return Ok(new { test = "access" });
                        }
                        else
                        {
                            // create acc? ig can't erlaly be bothered rn
                            var FindUserId = await Handlers.FindOne<User>("username", username);
                            if (FindUserId != "Error")
                            {
                                // Create the account
                                return Ok(new { test = "username is in use but lkets make you a acc" });
                            }
                            else
                            {
                                IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                                IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");


                                string AccountId = Guid.NewGuid().ToString();
                                string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
                                User UserData = new User
                                {
                                    AccountId = AccountId,
                                    DiscordId = id,
                                    Username = username,
                                    Email = GenerateRandomString(10) + "@fortbackend.com",
                                    accesstoken = NewAccessToken,
                                    Password = GenerateRandomString(15)
                                };

                                Account AccountData = new Account
                                {
                                    AccountId = AccountId,
                                    DiscordId = id
                                };

                                Accountcollection.InsertOne(AccountData);
                                Usercollection.InsertOne(UserData);

                                return Ok(new { test = NewAccessToken });
                            }
                        }
                    }else
                    {
                        // user is not in the server!
                    }


                    //https://discord.com/api/users/@me
                    Console.WriteLine(responseContent);
                }
             
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return Ok(new { test = "ngl" });
        }

        // This will be removed at some point this is just for me to test without discord...
        [HttpGet("/devers/create")]
        public async Task<IActionResult> CreateAccount()
        {
            var FormRequest = HttpContext.Request.Form;

            var username = "";

            if (FormRequest.TryGetValue("username", out var usernameL))
            {
                username = usernameL;
            }

            var FindAccountId = await Handlers.FindOne<User>("username", username);
            if (FindAccountId == "Error")
            {

            }

            return Ok(new { test = "iidrk" });
        }

    }
}
