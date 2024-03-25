using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Driver;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Routes.Development
{
    public class CreateAccount
    {
        public CreateAccount() { }

        public static async Task<string> Init(HttpContext httpContext, IMongoDatabase _database, UserInfo responseData1, bool Global = false)
        {
            var username = responseData1.username;
            var GlobalName = responseData1.global_name;
            var id = responseData1.id;
            var email = responseData1.email;

            IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
            IMongoCollection<UserFriends> UserFriendscollection = _database.GetCollection<UserFriends>("UserFriends");
            IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");


            string AccountId = Guid.NewGuid().ToString();
            string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
            string[] UserIp = new string[] { httpContext.Connection.RemoteIpAddress?.ToString() };

            User UserData = new User
            {
                AccountId = AccountId,
                DiscordId = id,
                Username = Global ? GlobalName : username,
                Email = Generate.RandomString(10) + "@fortbackend.com",
                accesstoken = NewAccessToken,
                UserIps = UserIp,
                Password = Generate.RandomString(15)
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
            return NewAccessToken;
        }
    }
}
