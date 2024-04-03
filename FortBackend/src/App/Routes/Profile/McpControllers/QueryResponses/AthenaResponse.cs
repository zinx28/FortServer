using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.FortniteServices.Events;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.Utilities.Shop.Helpers.Class;
using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using static FortBackend.src.App.Utilities.Helpers.Grabber;

namespace FortBackend.src.App.Routes.Profile.McpControllers.QueryResponses
{
    public class AthenaResponse
    {
        class ItemThingygyydsf
        {
            public string templateId { get; set; } = string.Empty;
            public object attributes { get; set; } = new object();
            public int quantity { get; set; } = 0;
        }
        public static async Task<Mcp> Grab(string AccountId, string ProfileId, VersionClass Season, int RVN, ProfileCacheEntry profileCacheEntry)
        {
            try
            {
                bool FoundSeasonDataInProfile = profileCacheEntry.AccountData.commoncore.Seasons.Any(season => season.SeasonNumber == Season.Season);

                if (!FoundSeasonDataInProfile)
                {
                    string[] tokens = new string[0];
                    if (Season.Season >= 8 && Season.Season < 23)
                    {
                        tokens = new string[] {
                            $"ARENA_S{Season.Season}_Division1"
                        };
                    }

                    SeasonClass seasonJson = new SeasonClass
                    {
                        SeasonNumber = Season.Season,
                        BookLevel = 1,
                        BookXP = 0,
                        BookPurchased = false,
                        Quests = new Dictionary<string, object>(),
                        BattleStars = 0,
                        DailyQuests = new DailyQuests
                        {
                            Interval = "0001-01-01T00:00:00.000Z",
                            Rerolls = 1
                        },
                        events = new Events
                        {
                            tokens = tokens
                        }
                    };

                    profileCacheEntry.AccountData.commoncore.Seasons.Add(seasonJson);
                }

                List<SeasonClass> Seasons = profileCacheEntry.AccountData.commoncore.Seasons;

                if (profileCacheEntry.AccountData.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season);

                    if (seasonObject != null)
                    {
                        if (profileCacheEntry.AccountData.athena.RVN == profileCacheEntry.AccountData.athena.CommandRevision)
                        {
                            profileCacheEntry.AccountData.athena.RVN = +1;
                        }

                        DailyQuests quest_manager = seasonObject.DailyQuests;
                        DateTime inputDateTime1;
                        if (DateTime.TryParseExact(quest_manager.Interval, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out inputDateTime1)) { }
                        Mcp AthenaClass = new Mcp()
                        {
                            profileRevision = profileCacheEntry.AccountData.athena.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = profileCacheEntry.AccountData.athena.RVN,
                            profileChanges = new List<object>()
                            {
                                new ProfileChange
                                {
                                    ChangeType = "fullProfileUpdate",
                                    Profile = new ProfileData
                                    {
                                        _id = profileCacheEntry.AccountData.AccountId,
                                       // Update = "",
                                        Created = DateTime.Parse("2021-03-07T16:33:28.462Z"),
                                        Updated = profileCacheEntry.AccountData.athena.Updated,
                                        rvn = profileCacheEntry.AccountData.athena.RVN,
                                        WipeNumber = 1,
                                        accountId = AccountId,
                                        profileId = ProfileId,
                                        version = "no_version",
                                        stats = new Stats
                                        {
                                            attributes = new AthenaStatsAttributes
                                            {
                                                use_random_loadout = false,
                                                past_seasons = new List<object>(),
                                                season_match_boost = seasonObject.season_match_boost,
                                                loadouts =  profileCacheEntry.AccountData.athena.loadouts,
                                                mfa_reward_claimed = false,
                                                rested_xp_overflow = 0,
                                                last_xp_interaction = "9999-12-10T22:14:37.647Z",
                                                quest_manager = new {
                                                    dailyLoginInterval = inputDateTime1.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                                    dailyQuestRerolls = quest_manager.Rerolls
                                                },
                                                creative_dynamic_xp = new { },
                                                season = new SeasonStats
                                                {
                                                    numWins = 0,
                                                    numHighBracket = 0,
                                                    numLowBracket = 0,
                                                },
                                                battlestars = seasonObject.battlestars_currency,
                                                vote_data = new { },
                                                battlestars_season_total = seasonObject.battlestars_currency,
                                                lifetime_wins = 0,
                                                rested_xp_exchange = 1,
                                                rested_xp_cumulative = 0,
                                                rested_xp_mult = 0,
                                                season_friend_match_boost = seasonObject.season_friend_match_boost,
                                                active_loadout_index = Array.IndexOf(profileCacheEntry.AccountData.athena.loadouts, profileCacheEntry.AccountData.athena.last_applied_loadout),
                                                purchased_bp_offers = new List<object> { },
                                                last_applied_loadout = profileCacheEntry.AccountData.athena.last_applied_loadout?.ToString() ?? "",
                                                xp = seasonObject.BookXP,
                                                rested_xp = seasonObject.BookXP,
                                                accountLevel = seasonObject.Level,
                                                level = seasonObject.Level,
                                                book_purchased = seasonObject.BookPurchased,
                                                book_xp = seasonObject.BattleStars,
                                                season_num = Season.Season,
                                                book_level = seasonObject.BookLevel
                                            }
                                        },
                                        items = new Dictionary<string, object>(),
                                        commandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                                    }
                                }
                            },
                            profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                            responseVersion = 1,
                        };

                        var ProfileChange = AthenaClass.profileChanges[0] as ProfileChange;
                        if (ProfileChange != null)
                        {

                            if (Saved.DeserializeConfig.FullLockerForEveryone)
                            {
                                //ProfileChange.Profile.items = Saved.DeserializeConfig.FullLocker_AthenaItems;
                                foreach (var kvp in Saved.DeserializeConfig.FullLocker_AthenaItems)
                                {
                                    ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else
                            {
                                foreach (var kvp in profileCacheEntry.AccountData.athena.Items)
                                {
                                    ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);
                                }
                            }

                            foreach(var kvp in seasonObject.DailyQuests.Daily_Quests)
                            {
                                var Value = kvp.Value;
                                var AthenaItemDynamicData = new AthenaItemDynamic
                                {
                                    templateId = Value.templateId,
                                    attributes = new Dictionary<string, object>
                                    {
                                        { "creation_time", Value.attributes.creation_time },
                                        { "level", Value.attributes.level },
                                        { "item_seen", Value.attributes.item_seen },
                                        { "playlists", Value.attributes.playlists },
                                        { "sent_new_notification", Value.attributes.sent_new_notification },
                                        { "challenge_bundle_id", Value.attributes.challenge_bundle_id },
                                        { "xp_reward_scalar", Value.attributes.xp_reward_scalar },
                                        { "challenge_linked_quest_given", Value.attributes.challenge_linked_quest_given },
                                        { "quest_pool", Value.attributes.quest_pool },
                                        { "quest_state", Value.attributes.quest_state },
                                        { "bucket", Value.attributes.bucket },
                                        { "last_state_change_time", Value.attributes.last_state_change_time },
                                        { "challenge_linked_quest_parent", Value.attributes.challenge_linked_quest_parent },
                                        { "max_level_bonus", Value.attributes.max_level_bonus },
                                        { "xp", Value.attributes.xp },
                                        { "quest_rarity", Value.attributes.quest_rarity },
                                        { "favorite", Value.attributes.favorite }

                                    },
                                    quantity = Value.quantity,
                                };

                                Value.attributes.ObjectiveState.ForEach(e =>
                                {
                                    AthenaItemDynamicData.attributes.Add(e.Name, e.Value);
                                });

                                ProfileChange.Profile.items.Add(kvp.Key, AthenaItemDynamicData);
                            }

                            foreach (var kvp in profileCacheEntry.AccountData.athena.loadouts_data)
                            {
                                ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);

                                if (kvp.Key == "sandbox_loadout" && ProfileChange.Profile.stats.attributes != null)
                                {
                                    AthenaStatsAttributes LoadoutAttributes = ProfileChange.Profile.stats.attributes as AthenaStatsAttributes;
                                    if (LoadoutAttributes != null)
                                    {
                                        LoadoutAttributes.favorite_backpack = kvp.Value.attributes.locker_slots_data.slots.backpack.items[0];
                                        LoadoutAttributes.favorite_character = kvp.Value.attributes.locker_slots_data.slots.character.items[0];
                                        LoadoutAttributes.favorite_dance = kvp.Value.attributes.locker_slots_data.slots.dance.items;
                                        LoadoutAttributes.favorite_glider = kvp.Value.attributes.locker_slots_data.slots.glider.items[0];
                                        LoadoutAttributes.favorite_itemwraps = kvp.Value.attributes.locker_slots_data.slots.itemwrap.items;
                                        LoadoutAttributes.favorite_loadingscreen = kvp.Value.attributes.locker_slots_data.slots.loadingscreen.items[0];
                                        LoadoutAttributes.favorite_musicpack = kvp.Value.attributes.locker_slots_data.slots.musicpack.items[0];
                                        LoadoutAttributes.favorite_pickaxe = kvp.Value.attributes.locker_slots_data.slots.pickaxe.items[0];
                                        LoadoutAttributes.favorite_skydivecontrail = kvp.Value.attributes.locker_slots_data.slots.skydivecontrail.items[0];
                                        LoadoutAttributes.banner_color = kvp.Value.attributes.banner_color_template;
                                        LoadoutAttributes.banner_icon = kvp.Value.attributes.banner_icon_template;
                                    }
                                }
                            }

                          
                            if(ProfileId == "profile0")
                            {
                                foreach (var profileChange in Saved.DeserializeConfig.DefaultBanners_Items)
                                {
                                    ProfileChange.Profile.items.Add(profileChange.Key, profileChange.Value);
                                }
                            }
                           

                            int GrabPlacement3 = profileCacheEntry.AccountData.commoncore.Items.Select((pair, index) => (pair.Key, pair.Value, index))
                            .TakeWhile(pair => !pair.Key.Equals("Currency")).Count();

                            if (GrabPlacement3 != -1)
                            {
                                var Value = profileCacheEntry.AccountData.commoncore.Items["Currency"];
                         
                                if (Value.templateId != null || Value != null)
                                {
                                    if (Value.templateId == "Currency:MtxPurchased")
                                    {
                                        ProfileChange.Profile.items.Add("Currency", new
                                        {
                                            templateId = "Currency:MtxPurchased",
                                            attributes = new
                                            {
                                                platform = "Shared"
                                            },
                                            quantity = Value.quantity,
                                        });
                                    }
                                }
                            }
                        }else
                        {
                            Logger.Error("WHY IS THIS NULL WTFFFFFFFFFFF");
                        }

                        return AthenaClass;
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"AthenaResponse: {ex.Message}");
            }

            return new Mcp();
        }
    }
}
