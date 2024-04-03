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
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Saved;
using Amazon.Runtime.Internal.Transform;

namespace FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses
{
    public class CommonCoreResponse
    {
        public static async Task<Mcp> Grab(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            try
            {
                bool FoundSeasonDataInProfile = profileCacheEntry.AccountData.commoncore.Seasons.Any(season => season.SeasonNumber == Season.Season);

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

                //AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", AccountId))[0];

                //if (AccountDataParsed == null)
                //{
                //    return new Mcp();
                //}


                Mcp CommonCoreClass = new Mcp()
                {
                    profileRevision = profileCacheEntry.AccountData.commoncore.RVN,
                    profileId = ProfileId,
                    profileChangesBaseRevision = profileCacheEntry.AccountData.commoncore.RVN,
                    profileChanges = new List<dynamic>
                    {
                        new ProfileChange
                        {
                            ChangeType = "fullProfileUpdate",
                            Profile = new ProfileData
                            {
                                _id = profileCacheEntry.AccountData.AccountId,
                                Update = "",
                                Created = DateTime.Parse("2021-03-07T16:33:28.462Z"),
                                Updated = profileCacheEntry.AccountData.commoncore.Updated,
                                rvn = profileCacheEntry.AccountData.commoncore.RVN,
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
                                        mtx_affiliate = profileCacheEntry.AccountData.commoncore.mtx_affiliate,
                                        undo_cooldowns = new List<string>(),
                                        mtx_affiliate_set_time = "",
                                        import_friends_claimed = new object[0],
                                        mtx_purchase_history = profileCacheEntry.AccountData.commoncore.mtx_purchase_history,
                                        inventory_limit_bonus = 0,
                                        current_mtx_platform = profileCacheEntry.AccountData.commoncore.current_mtx_platform,
                                        weekly_purchases = profileCacheEntry.AccountData.commoncore.weekly_purchases,
                                        daily_purchases = profileCacheEntry.AccountData.commoncore.daily_purchases,
                                        ban_history = new object[0],
                                        in_app_purchases = new object[0],
                                        permissions = new List<Dictionary<string, object>>(),
                                        undo_timeout = "min",
                                        monthly_purchases = new object[0],
                                        allowed_to_send_gifts = profileCacheEntry.AccountData.commoncore.allowed_to_send_gifts,
                                        mfa_enabled = false,
                                        allowed_to_receive_gifts = profileCacheEntry.AccountData.commoncore.allowed_to_receive_gifts,
                                        gift_history = new object[0],
                                    }
                                },
                                items = new Dictionary<string, object>(), //profileCacheEntry.AccountData.commoncore.Items,
                                commandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                            }
                        }
                    },
                    profileCommandRevision = profileCacheEntry.AccountData.commoncore.CommandRevision,
                    serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                    responseVersion = 1,
                };
                var ProfileChange = CommonCoreClass.profileChanges[0] as ProfileChange;
                if (ProfileChange != null)
                {
                    foreach (var profileChange in profileCacheEntry.AccountData.commoncore.Items)
                    {
                        ProfileChange.Profile.items.Add(profileChange.Key, new
                        {
                            templateId = profileChange.Value.templateId,
                            attributes = new
                            {
                                platform = profileCacheEntry.AccountData.commoncore.current_mtx_platform
                            },
                            quantity = profileChange.Value.quantity,
                        });
                    }
                    foreach (var profileChange in Saved.DeserializeConfig.DefaultBanners_Items)
                    {
                        ProfileChange.Profile.items.Add(profileChange.Key, profileChange.Value);
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
