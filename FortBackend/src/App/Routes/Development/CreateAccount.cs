using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Routes.Development
{
    public class CreateAccount
    {
        public CreateAccount() { }
        public class SandboxLoadoutAttributes1
        {
            public object locker_slots_data { get; set; }
            public string banner_color_template { get; set; }
            public string banner_icon_template { get; set; }
            public string locker_name { get; set; }

            public int use_count { get; set; }
            public bool item_seen { get; set; }
            public bool favorite { get; set; }
        }

        public class ProfileItem1
        {
            public string templateId { get; set; }
            public SandboxLoadoutAttributes1 attributes { get; set; }
            public int quantity { get; set; }
        }
        public static async Task<string> Init(HttpContext httpContext, IMongoDatabase _database, UserInfo responseData1, bool Global = false)
        {
            try
            {
                var username = responseData1.username;
                var GlobalName = responseData1.global_name;
                var id = responseData1.id;

                //var email = responseData1.email;
                IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                IMongoCollection<UserFriends> UserFriendscollection = _database.GetCollection<UserFriends>("UserFriends");
                IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");


                string AccountId = Guid.NewGuid().ToString();
                string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
                string[] UserIp = new string[] { httpContext.Connection.RemoteIpAddress?.ToString() };


                IMongoCollection<StoreInfo> StoreInfocollection = _database.GetCollection<StoreInfo>("StoreInfo");
                var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIps, httpContext.Connection.RemoteIpAddress?.ToString());
                var count = await StoreInfocollection.CountDocumentsAsync(filter);
                bool BanUser = false;
                if (count > 0)
                {
                    BanUser = true;

                    var update = Builders<StoreInfo>.Update
                        .PushEach(e => e.UserIps, new[] { httpContext.Connection.RemoteIpAddress?.ToString() })
                        .PushEach(e => e.UserIds, new[] { AccountId });

                    var updateResult = await StoreInfocollection.UpdateManyAsync(filter, update);

                    if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                    {
                        if (Saved.DeserializeConfig.DetectedWebhookUrl != null)
                        {
                            await BanAndWebHooks.Init(Saved.DeserializeConfig, responseData1);
                        }
                    }
                }

                User UserData = new User
                {
                    AccountId = AccountId,
                    DiscordId = id,
                    Username = Global ? GlobalName : username,
                    Email = Generate.RandomString(10) + "@fortbackend.com",
                    accesstoken = NewAccessToken,
                    UserIps = UserIp,
                    banned = BanUser,
                    Password = Generate.RandomString(15)
                };

                UserFriends UserFriendsData = new UserFriends
                {
                    AccountId = AccountId,
                    DiscordId = id
                };

                Account AccountData = new Account
                {
                    AccountId = AccountId,
                    athena = new Athena()
                    {
                        Items = new Dictionary<string, AthenaItem>()
                        {
                            ["AthenaPickaxe:DefaultPickaxe"] = new AthenaItem
                            {
                                templateId = "AthenaPickaxe:DefaultPickaxe",
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = true
                                }
                            },
                            ["AthenaGlider:DefaultGlider"] = new AthenaItem
                            {
                                templateId = "AthenaGlider:DefaultGlider",
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = true
                                }
                            },
                            ["AthenaDance:EID_DanceMoves"] = new AthenaItem
                            {
                                templateId = "AthenaDance:EID_DanceMoves",
                                attributes = new AthenaItemAttributes
                                {
                                    item_seen = true
                                }
                            }
                        },
                        loadouts_data = new Dictionary<string, SandboxLoadout>()
                        {
                            ["sandbox_loadout"] = new SandboxLoadout() // dont need much just the default obj
                            {
                                attributes = new SandboxLoadoutAttributes
                                {
                                    locker_slots_data = new SandboxLoadoutSlots
                                    {
                                        slots = new LockerSlotsData
                                        {
                                            character = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            musicpack = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            backpack = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            pickaxe = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            skydivecontrail = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            loadingscreen = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            glider = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            dance = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    "", "", "", "", "", ""
                                                }
                                            },
                                            itemwrap = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    "", "", "", "", "", "", ""
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    commoncore = new CommonCore()
                    {
                        Items = new Dictionary<string, CommonCoreItem>()
                        {
                            ["Currency"] = new CommonCoreItem
                            {
                                templateId = "Currency:MtxPurchased",
                                attributes = new CommonCoreItemAttributes
                                {
                                    platform = "EpicPC"
                                },
                                quantity = 1000
                            }
                        }
                    },
                    DiscordId = id
                };

                await Accountcollection.InsertOneAsync(AccountData); // first if it fails then tell the user
                Usercollection.InsertOne(UserData);
                UserFriendscollection.InsertOne(UserFriendsData);
                return NewAccessToken;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "CreateAccount");
            }
            return "ERROR";
        }
    }
}
