using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Profile.Query.Items;
using FortLibrary.MongoDB;
using FortLibrary.MongoDB.Module;
using MongoDB.Driver;

namespace FortBackend.src.App.Utilities.MongoDB.Management
{
    public class MongoDBCreateAccount
    {
        public static async Task Init(CreateAccountArg CreateAccArg)
        {
            IMongoCollection<User> Usercollection = MongoDBStart.Database.GetCollection<User>("User");
            IMongoCollection<UserFriends> UserFriendscollection = MongoDBStart.Database.GetCollection<UserFriends>("UserFriends");
            IMongoCollection<Account> Accountcollection = MongoDBStart.Database.GetCollection<Account>("Account");
            IMongoCollection<StatsInfo> Statscollection = MongoDBStart.Database.GetCollection<StatsInfo>("StatsInfo");

            string AccountId = Guid.NewGuid().ToString("N").Replace("-", "").Substring(0, 12) + CreateAccArg.DiscordId;
            string NewAccessToken = JWT.GenerateRandomJwtToken(15, Saved.Saved.DeserializeConfig.JWTKEY);

            //string Password = BCrypt.Net.BCrypt.HashPassword(CreateAccArg.Password);

            User UserData = new User
            {
                AccountId = AccountId,
                DiscordId = CreateAccArg.DiscordId,
                Username = CreateAccArg.DisplayName,
                Email = CreateAccArg.Email,
                accesstoken = NewAccessToken,
                UserIps = new string[0],
                banned = false,
                Password = CryptoGen.HashPassword(CreateAccArg.Password),
            };

            UserFriends UserFriendsData = new UserFriends
            {
                AccountId = AccountId,
                DiscordId = CreateAccArg.DiscordId
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
                                                "AthenaPickaxe:DefaultPickaxe"
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
                                                "AthenaGlider:DefaultGlider"
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
                DiscordId = CreateAccArg.DiscordId
            };

            StatsInfo statsData = new StatsInfo()
            {
                AccountId = AccountId,
                DiscordId = CreateAccArg.DiscordId
            };

            await Accountcollection.InsertOneAsync(AccountData); // first if it fails then tell the user
            Usercollection.InsertOne(UserData);
            UserFriendscollection.InsertOne(UserFriendsData);
            Statscollection.InsertOne(statsData); // stats data
        }
    }
}
