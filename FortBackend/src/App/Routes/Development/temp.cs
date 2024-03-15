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
                                return Ok(new { test = NewAccessToken });
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
                                    IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                                    IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");


                                    string AccountId = Guid.NewGuid().ToString();
                                    string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
                                    User UserData = new User
                                    {
                                        AccountId = AccountId,
                                        DiscordId = id,
                                        Username = GlobalName,
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
                            else
                            {
                                IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                                IMongoCollection<UserFriends> UserFriendscollection = _database.GetCollection<UserFriends>("UserFriends");
                                IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");


                                string AccountId = Guid.NewGuid().ToString();
                                string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
                                User UserData = new User
                                {
                                    AccountId = AccountId,
                                    DiscordId = id,
                                    Username = GlobalName,
                                    Email = GenerateRandomString(10) + "@fortbackend.com",
                                    accesstoken = NewAccessToken,
                                    Password = GenerateRandomString(15)
                                };

                                UserFriends UserFriendsData = new UserFriends
                                {
                                    AccountId = AccountId,
                                    DiscordId = id
                                };
                                //string RandomNewId = Guid.NewGuid().ToString();
                                Account AccountData = new Account
                                {
                                    AccountId = AccountId,
                                    athena = new Athena()
                                    {
                                        Items = new List<Dictionary<string, object>>()
                                        {
                                            new Dictionary<string, object>
                                            {
                                                ["sandbox_loadout"] = new
                                                {
                                                    templateId = "CosmeticLocker:cosmeticlocker_athena",
                                                    attributes = new
                                                    {
                                                        locker_slots_data = new
                                                        {
                                                            slots = new
                                                            {
                                                                musicpack = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                character = new
                                                                {
                                                                    items = new List<string> { "" },
                                                                    ActiveVariants = new string[0]
                                                                },
                                                                backpack = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                pickaxe = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                skydivecontrail = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                dance = new
                                                                {
                                                                    items = new string[]
                                                                    {
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        ""
                                                                    }
                                                                },
                                                                loadingscreen = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                glider = new
                                                                {
                                                                    items = new List<string> { "" }
                                                                },
                                                                itemwrap = new
                                                                {
                                                                   items = new string[]
                                                                    {
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        ""
                                                                    }
                                                                }
                                                            },
                                                        },
                                                        use_count = 0,
                                                        banner_color_template = "",
                                                        banner_icon_template = "",
                                                        locker_name = "",
                                                        item_seen = false,
                                                        favorite = false
                                                    },
                                                    quantity = 1
                                                }
                                            },
                                            new Dictionary<string, object>
                                            {
                                                ["AthenaPickaxe:DefaultPickaxe"] = new
                                                {
                                                    attributes = new
                                                    {
                                                        favorite = false,
                                                        item_seen = true,
                                                        level = 0,
                                                        max_level_bonus = 0,
                                                        rnd_sel_cnt = 0,
                                                        variants = new List<object>(),
                                                        xp = 0,
                                                    },
                                                    templateId = "AthenaPickaxe:DefaultPickaxe",
                                                    quantity = 1
                                                }
                                            },
                                            new Dictionary<string, object>
                                            {
                                                ["AthenaGlider:DefaultGlider"] = new
                                                {
                                                    attributes = new
                                                    {
                                                        favorite = false,
                                                        item_seen = true,
                                                        level = 0,
                                                        max_level_bonus = 0,
                                                        rnd_sel_cnt = 0,
                                                        variants = new List<object>(),
                                                        xp = 0,
                                                    },
                                                    templateId = "AthenaGlider:DefaultGlider",
                                                    quantity = 1
                                                }
                                            },
                                            new Dictionary<string, object>
                                            {
                                                ["AthenaDance:EID_DanceMoves"] = new
                                                {
                                                    attributes = new
                                                    {
                                                        favorite = false,
                                                        item_seen = true,
                                                        level = 0,
                                                        max_level_bonus = 0,
                                                        rnd_sel_cnt = 0,
                                                        variants = new List<object>(),
                                                        xp = 0,
                                                    },
                                                    templateId = "AthenaDance:EID_DanceMoves",
                                                    quantity = 1
                                                }
                                            }
                                        }
                                    },
                                    commoncore = new CommonCore()
                                    {
                                        Items = new List<Dictionary<string, object>>()
                                        {
                                            new Dictionary<string, object>
                                            {
                                                ["Currency"] = new
                                                {
                                                    templateId = "Currency:MtxPurchased",
                                                    attributes = new
                                                    {
                                                        platform = "EpicPC"
                                                    },
                                                    quantity = 1000
                                                }
                                            }
                                        }
                                    },
                                    DiscordId = id
                                };

                                Accountcollection.InsertOne(AccountData);
                                Usercollection.InsertOne(UserData);
                                UserFriendscollection.InsertOne(UserFriendsData);

                                return Ok(new { test = NewAccessToken });
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
