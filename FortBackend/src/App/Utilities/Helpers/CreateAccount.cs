﻿using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.Encoders;
using FortBackend.src.App.Utilities.Helpers.UserManagement;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using static FortBackend.src.App.Utilities.Classes.DiscordAuth;

namespace FortBackend.src.App.Utilities.Helpers
{
    public class CreateAccount
    {
        public CreateAccount() { }
        public class SandboxLoadoutAttributes1
        {
            public object locker_slots_data { get; set; } = new object();
            public string banner_color_template { get; set; } = string.Empty;
            public string banner_icon_template { get; set; } = string.Empty;
            public string locker_name { get; set; } = string.Empty;

            public int use_count { get; set; }
            public bool item_seen { get; set; }
            public bool favorite { get; set; }
        }

        public class ProfileItem1
        {
            public string templateId { get; set; } = string.Empty;
            public SandboxLoadoutAttributes1 attributes { get; set; } = new SandboxLoadoutAttributes1();
            public int quantity { get; set; }
        }
        public static async Task<string> Init(HttpContext httpContext, IMongoDatabase _database, UserInfo responseData1, bool Global = false)
        {
            try
            {
                
                var Ip = "";
                if (Saved.Saved.DeserializeConfig.Cloudflare)
                {
                    Ip = httpContext.Request.Headers["CF-Connecting-IP"];
                }
                else
                {
                    Ip = httpContext.Connection.RemoteIpAddress!.ToString();
                }
                var username = responseData1.username;
                var GlobalName = responseData1.global_name;
                var DiscordId = responseData1.id;

                //var email = responseData1.email;
                IMongoCollection<User> Usercollection = _database.GetCollection<User>("User");
                IMongoCollection<UserFriends> UserFriendscollection = _database.GetCollection<UserFriends>("UserFriends");
                IMongoCollection<Account> Accountcollection = _database.GetCollection<Account>("Account");
                IMongoCollection<StatsInfo> Statscollection = _database.GetCollection<StatsInfo>("StatsInfo");


                string AccountId = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12) + DiscordId;
                string NewAccessToken = JWT.GenerateRandomJwtToken(15, "FortBackendIsSoCoolLetMeNutAllOverYou!@!@!@!@!");
                string[] UserIp = new string[] { Ip };


                IMongoCollection<StoreInfo> StoreInfocollection = _database.GetCollection<StoreInfo>("StoreInfo");
                var filter = Builders<StoreInfo>.Filter.AnyEq(b => b.UserIps, Ip);
                var count = await StoreInfocollection.CountDocumentsAsync(filter);
                bool BanUser = false;
                if (count > 0)
                {
                    BanUser = true;

                    var update = Builders<StoreInfo>.Update
                        .PushEach(e => e.UserIps, new[] { Ip })
                        .PushEach(e => e.UserIds, new[] { AccountId });

                    var updateResult = await StoreInfocollection.UpdateManyAsync(filter, update);

                    if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                    {
                        if (Saved.Saved.DeserializeConfig.DetectedWebhookUrl != null)
                        {
                            await BanAndWebHooks.Init(Saved.Saved.DeserializeConfig, responseData1);
                        }
                    }
                }

                User UserData = new User
                {
                    AccountId = AccountId,
                    DiscordId = DiscordId,
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
                    DiscordId = DiscordId
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
                                templateId = "CosmeticLocker:cosmeticlocker_athena",
                                attributes = new SandboxLoadoutAttributes
                                {
                                    locker_slots_data = new SandboxLoadoutSlots
                                    {
                                        slots = new LockerSlotsData
                                        {
                                            musicpack = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                }
                                            },
                                            character = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                },
                                                activevariants = new List<object>()
                                            },
                                            backpack = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                },
                                                activevariants = new List<object>()
                                            },
                                            pickaxe = new Slots
                                            {
                                                items = new List<string>
                                                {
                                                    ""
                                                },
                                                activevariants = new List<object>()
                                            },
                                            skydivecontrail = new Slots
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
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
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
                                            itemwrap = new Slots
                                            {
                                                items = new List<string>
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
                                        }
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
                        }
                    },
                    commoncore = new CommonCore()
                    {
                        Items = new Dictionary<string, CommonCoreItem>()
                        {
                            ["Currency"] = new CommonCoreItem
                            {
                                templateId = "Currency:MtxPurchased",
                                //attributes = new CommonCoreItemAttributes
                                //{
                                //     platform = "EpicPC"
                                // },
                                quantity = 1000
                            }
                        }
                    },
                    DiscordId = DiscordId
                };

                StatsInfo statsData = new StatsInfo()
                {
                    AccountId = AccountId,
                    DiscordId = DiscordId,
                    Gamemodes = new List<GamemodeStatsData>()
                    {
                        new GamemodeStatsData
                        {
                            Gamemode = "solo"
                        },
                        new GamemodeStatsData
                        {
                            Gamemode = "duos"
                        },
                        new GamemodeStatsData
                        {
                            Gamemode = "trios"
                        },
                        new GamemodeStatsData
                        {
                            Gamemode = "squad"
                        },
                        new GamemodeStatsData
                        {
                            Gamemode = "ltm"
                        }
                    }
                };

                await Accountcollection.InsertOneAsync(AccountData); // first if it fails then tell the user
                Usercollection.InsertOne(UserData);
                UserFriendscollection.InsertOne(UserFriendsData);
                Statscollection.InsertOne(statsData); // stats data
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