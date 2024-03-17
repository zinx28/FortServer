using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Bson;
using System.Globalization;
using Newtonsoft.Json;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes;
using FortBackend.src.App.Utilities;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses
{
    public class CommonCoreResponse
    {
        public static async Task<Mcp> Grab(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed)
        {
            try
            {
                bool FoundSeasonDataInProfile = AccountDataParsed.commoncore.Seasons.Any(season => season.SeasonNumber == Season.Season);

                //if (!FoundSeasonDataInProfile)
                //{
                //    string seasonJson = JsonConvert.SerializeObject(new SeasonClass
                //    {
                //        SeasonNumber = Season.Season,
                //        BookLevel = 1,
                //        BookXP = 0,
                //        BookPurchased = false,
                //        Quests = new List<Dictionary<string, object>>(),
                //        BattleStars = 0,
                //        DailyQuests = new DailyQuests
                //        {
                //            Interval = "0001-01-01T00:00:00.000Z",
                //            Rerolls = 1
                //        },
                //        arena = new Arena
                //        {
                //            tokens = new string[] {
                //                $"ARENA_S{Season.Season}_Division1"
                //            }
                //        }
                //    });

                //    await Handlers.PushOne<Account>("accountId", AccountId, new Dictionary<string, object>
                //    {
                //        {
                //            "commoncore.Season", BsonDocument.Parse(seasonJson)
                //        }
                //    });
                //}

                AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", AccountId))[0];

                if (AccountDataParsed == null)
                {
                    return new Mcp();
                }


                Mcp CommonCoreClass = new Mcp()
                {
                    profileRevision = AccountDataParsed.commoncore.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = AccountDataParsed.commoncore.RVN,
                    profileChanges = new List<dynamic>
                    {
                        new ProfileChange
                        {
                            ChangeType = "fullProfileUpdate",
                            Profile = new ProfileData
                            {
                                _id = AccountDataParsed.AccountId,
                                Update = "",
                                Created = DateTime.Parse("2021-03-07T16:33:28.462Z"),
                                Updated = AccountDataParsed.commoncore.Updated,
                                rvn = AccountDataParsed.commoncore.RVN,
                                WipeNumber = 1,
                                accountId = AccountId,
                                profileId = ProfileId,
                                version = "no_version",
                                stats = new Stats
                                {
                                    attributes = new CommonCoreStatsAttributes
                                    {
                                        personal_offers = new object[0],
                                        intro_game_played = true,
                                        import_friends_played = new object[0],
                                        mtx_affiliate = AccountDataParsed.commoncore.mtx_affiliate,
                                        undo_cooldowns = new List<string>(),
                                        mtx_affiliate_set_time = "",
                                        import_friends_claimed = new object[0],
                                        mtx_purchase_history = AccountDataParsed.commoncore.mtx_purchase_history,
                                        inventory_limit_bonus = 0,
                                        current_mtx_platform = AccountDataParsed.commoncore.current_mtx_platform,
                                        weekly_purchases = AccountDataParsed.commoncore.weekly_purchases,
                                        daily_purchases = AccountDataParsed.commoncore.daily_purchases,
                                        ban_history = new object[0],
                                        in_app_purchases = new object[0],
                                        permissions = new List<Dictionary<string, object>>(),
                                        undo_timeout = "min",
                                        monthly_purchases = new object[0],
                                        allowed_to_send_gifts = AccountDataParsed.commoncore.allowed_to_send_gifts,
                                        mfa_enabled = false,
                                        allowed_to_receive_gifts = AccountDataParsed.commoncore.allowed_to_receive_gifts,
                                        gift_history = new object[0],
                                    }
                                },
                                items = new Dictionary<string, object>(),
                                commandRevision = AccountDataParsed.commoncore.CommandRevision,
                            }

                            }
                    },
                    profileCommandRevision = AccountDataParsed.commoncore.CommandRevision,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    responseVersion = 1,
                };

                List<Dictionary<string, object>> items = AccountDataParsed.commoncore.Items;

                foreach (Dictionary<string, object> item in items)
                {
                    try
                    {
                        string key = item.Keys.FirstOrDefault(k => k.Contains("Currency")) ?? "";
                        if (item.TryGetValue(key, out object value) && value is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic itemAttributes1 = JsonConvert.DeserializeObject(value.ToString());
                            if (itemAttributes1.templateId != null || value != null)
                            {
                                if (itemAttributes1.templateId == "Currency:MtxPurchased")
                                {

                                    Loadout itemAttributes = JsonConvert.DeserializeObject<Loadout>(value.ToString());
                                    var ProfileChange = CommonCoreClass.profileChanges[0] as ProfileChange;

                                    if (itemAttributes != null)
                                    {

                                        ProfileChange.Profile.items.Add(key, new
                                        {
                                            itemAttributes.templateId,
                                            attributes = new
                                            {
                                                platform = "EpicPC"
                                            },
                                            itemAttributes.quantity,
                                        });
                                    }
                                }
                                else
                                {
                                    //idk
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }
                }

                return CommonCoreClass;
            }
            catch (Exception ex)
            {
                Logger.Error($"CommonCoreResponse: {ex.Message}");
            }

            return new Mcp();
        }
    }
}
