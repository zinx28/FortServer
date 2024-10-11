using FortBackend.src.App.Utilities;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.Saved;
using Amazon.Runtime.Internal.Transform;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Query;
using FortLibrary.EpicResponses.Profile.Query.Attributes;
using FortLibrary;
using FortLibrary.MongoDB.Module;

namespace FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses
{
    public class CommonCoreResponse
    {
        public static async Task<Mcp> Grab(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            try
            {
                bool FoundSeasonDataInProfile = profileCacheEntry.AccountData.commoncore.Seasons.Any(season => season.SeasonNumber == Season.Season);

                if(profileCacheEntry.AccountData.commoncore.ban_status.banDurationDays == 0)
                {
                    profileCacheEntry.AccountData.commoncore.ban_status.bRequiresUserAck = false;
                    profileCacheEntry.AccountData.commoncore.ban_status.bBanHasStarted = false;
                    profileCacheEntry.AccountData.commoncore.ban_status.banDurationDays = 0;
                    profileCacheEntry.UserData.temp_banned = false;
                }

                if (profileCacheEntry.AccountData.commoncore.ban_status.bBanHasStarted)
                {
                    DateTime banStartTime = profileCacheEntry.AccountData.commoncore.ban_status.banStartTimeUtc;
                    double banDurationDays = profileCacheEntry.AccountData.commoncore.ban_status.banDurationDays;
                    DateTime banEndTime = banStartTime.AddDays(banDurationDays);
                    DateTime currentUtcTime = DateTime.UtcNow;
                    if (currentUtcTime >= banEndTime)
                    {
                        profileCacheEntry.AccountData.commoncore.ban_status.bRequiresUserAck = false;
                        profileCacheEntry.AccountData.commoncore.ban_status.bBanHasStarted = false;
                        profileCacheEntry.AccountData.commoncore.ban_status.banDurationDays = 0;
                        profileCacheEntry.UserData.temp_banned = false;
                    }
                }
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
                                //Update = "",
                                Created = profileCacheEntry.AccountData.JoinDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                Updated = profileCacheEntry.AccountData.commoncore.Updated.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
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
                                        ban_history = profileCacheEntry.AccountData.commoncore.ban_history,
                                        ban_status = profileCacheEntry.AccountData.commoncore.ban_status,
                                        in_app_purchases = new object[0],
                                        permissions = new List<Dictionary<string, object>>(),
                                        undo_timeout = "min",
                                        monthly_purchases = new object[0],
                                        allowed_to_send_gifts = profileCacheEntry.AccountData.commoncore.allowed_to_send_gifts,
                                        mfa_enabled = profileCacheEntry.AccountData.commoncore.mfa_enabled,
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
                    serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
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
                    foreach (var profileChange in profileCacheEntry.AccountData.commoncore.Gifts)
                    {
                        ProfileChange.Profile.items.Add(profileChange.Key, profileChange.Value);
                    }
                    foreach (var profileChange in Saved.BackendCachedData.DefaultBanners_Items)
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
