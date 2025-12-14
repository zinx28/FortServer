using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortLibrary.MongoDB.Module;
using FortBackend.src.App.Utilities.Saved;
using FortLibrary.EpicResponses.Profile;
using FortLibrary.EpicResponses.Profile.Query;
using FortLibrary.EpicResponses.Profile.Query.Attributes;
using FortLibrary.EpicResponses.Profile.Query.Items;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using static FortBackend.src.App.Utilities.Helpers.Grabber;
using FortLibrary;
using FortLibrary.EpicResponses.Profile.Quests;
using FortBackend.src.App.Utilities.Quests;
using FortLibrary.Dynamics;
using FortBackend.src.App.Utilities.Discord.Helpers.command;

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
                        SeasonXP = 0,
                        BookLevel = 1,
                        BookXP = 0,
                        BookPurchased = false,
                        Quests = new Dictionary<string, DailyQuestsData>(),
                        special_items = new(),
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
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season)!;

                    if (seasonObject != null)
                    {
                        if (profileCacheEntry.AccountData.athena.RVN == profileCacheEntry.AccountData.athena.CommandRevision)
                        {
                            profileCacheEntry.AccountData.athena.RVN =+ 1;
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
                                        Created = profileCacheEntry.AccountData.JoinDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        Updated = profileCacheEntry.AccountData.athena.Updated.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                                        rvn = profileCacheEntry.AccountData.athena.RVN,
                                        WipeNumber = 1,
                                        accountId = AccountId,
                                        profileId = ProfileId,
                                        version = "no_version",
                                        stats = new Stats
                                        {
                                            attributes = new AthenaStatsAttributes
                                            {
                                                use_random_loadout = profileCacheEntry.AccountData.athena.random_loadout,
                                                past_seasons = new List<object>(),
                                                loadouts =  profileCacheEntry.AccountData.athena.loadouts!,
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
                                                season_match_boost = seasonObject.season_match_boost,
                                                season_friend_match_boost = seasonObject.season_friend_match_boost,
                                                active_loadout_index = 0/*profileCacheEntry.AccountData.athena.loadouts.FindIndex((e) => e == profileCacheEntry.AccountData.athena.last_applied_loadout)*/,
                                                purchased_bp_offers = seasonObject.season_offers,
                                                last_applied_loadout = profileCacheEntry.AccountData.athena.last_applied_loadout?.ToString() ?? "",
                                                xp = seasonObject.SeasonXP,
                                                rested_xp = seasonObject.SeasonXP,
                                                accountLevel = seasonObject.Level,
                                                level = seasonObject.Level,
                                                book_purchased = seasonObject.BookPurchased,
                                                book_xp = seasonObject.BookXP,
                                                season_num = Season.Season,
                                                book_level = seasonObject.BookLevel,
                                                party_assist_quest = seasonObject.party_assist
                                            }
                                        },
                                        items = new Dictionary<string, object>(),
                                        commandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                                    }
                                }
                            },
                            profileCommandRevision = profileCacheEntry.AccountData.athena.CommandRevision,
                            serverTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            responseVersion = 1,
                        };

                        var ProfileChange = AthenaClass.profileChanges[0] as ProfileChange;
                        if (ProfileChange != null)
                        {

                            if (Saved.DeserializeConfig.FullLockerForEveryone)
                            {
                                //ProfileChange.Profile.items = Saved.DeserializeConfig.FullLocker_AthenaItems;
                                foreach (var kvp in Saved.BackendCachedData.FullLocker_AthenaItems)
                                {
                                    ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else
                            {
                                foreach (var kvp in profileCacheEntry.AccountData.athena.Items!)
                                {
                                    ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);
                                }
                            }

                            foreach (var kvp in seasonObject.special_items)
                                ProfileChange.Profile.items.Add(kvp.Key, kvp.Value);


                            // THIS IS INST THE PROPER WAY BUT IT'S BETTER NTO STORING THIS IN THE CODE UNLESS IS ACTUALLY NEEDED
                            var ResponseId = "";
                            if (WeeklyQuestManager.WeeklyQuestsSeasonAboveDictionary.TryGetValue($"Season{seasonObject.SeasonNumber}", out List<WeeklyQuestsJson>? WeeklyQuestsArray))
                            {
                                if(WeeklyQuestsArray.Count > 0)
                                {
                                    List<string> ResponseIgIdrk = new List<string>();
                            
                                    foreach(var kvp in WeeklyQuestsArray)
                                    {
                                        if (!kvp.BundleRequired.QuestBundleID) continue;
                                        ResponseId = $"ChallengeBundleSchedule:{kvp.BundleSchedule}";
                                        ResponseIgIdrk.Add($"ChallengeBundle:{kvp.BundleId}");
                                        //kvp.BundleId

                                        if (kvp.BundleRequired.RequiredLevel > seasonObject.Level) continue;
                                        //if (kvp.BundleRequired.Weekly)
                                        //{
                                        
                                        //}

                                        // kvp.BundleRequired.RequiredLevel

                                        int CompletedNum = 0;
                                        List<string> grantedquestinstanceids = new List<string>();
                                        foreach (var FreeBundles in kvp.BundlesObject)
                                        {
                                            if (FreeBundles.quest_data.RequireBP)
                                            {
                                                if (seasonObject.BookPurchased)
                                                {
                                                    if (seasonObject.Quests.TryGetValue(FreeBundles.templateId, out DailyQuestsData? value))
                                                    {
                                                        grantedquestinstanceids.Add(FreeBundles.templateId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (seasonObject.Quests.TryGetValue(FreeBundles.templateId, out DailyQuestsData? value))
                                                {
                                                    grantedquestinstanceids.Add(FreeBundles.templateId);
                                                }
                                            }

                                            if (FreeBundles.quest_data.IsWeekly && !FreeBundles.quest_data.ExtraQuests)
                                            {
                                                if (!FreeBundles.quest_data.Steps)
                                                {
                                                   // Console.Write("TEST " + FreeBundles.templateId);
                                                    var GrabQuestData = seasonObject.Quests.FirstOrDefault(e => e.Key == FreeBundles.templateId).Value;
                                                    if (GrabQuestData != null)
                                                    {
                                                        if (GrabQuestData.attributes.quest_state == "Claimed")
                                                        {
                                                            CompletedNum += 1;
                                                        }
                                                    }
                                                }
                                               
                                            }

                                        }

                                        

                                        //challenge_bundle_schedule_id
                                        var AthenaItemChallengeBundle = new AthenaItemDynamic
                                        {
                                            templateId = $"ChallengeBundle:{kvp.BundleId}",
                                            attributes = new Dictionary<string, object>
                                            {
                                                { "has_unlock_by_completion", false },
                                                { "num_quests_completed", CompletedNum },
                                                { "level", 0 },
                                                { "grantedquestinstanceids", grantedquestinstanceids.ToArray() },
                                                { "item_seen",  true },
                                                { "max_allowed_bundle_level", 0 },
                                                { "num_granted_bundle_quests", grantedquestinstanceids.Count() },
                                                { "max_level_bonus", 0 },
                                                { "challenge_bundle_schedule_id", ResponseId },
                                                { "num_progress_quests_completed", CompletedNum },
                                                { "xp", 0 },
                                                { "favorite", false }
                                            },
                                            quantity = 1,
                                        };

                                        ProfileChange.Profile.items.Add($"ChallengeBundle:{kvp.BundleId}", AthenaItemChallengeBundle);
                                    }

                                    var AthenaItemDynamicData = new AthenaItemDynamic
                                    {
                                        templateId = ResponseId,
                                        attributes = new Dictionary<string, object>
                                        {
                                            { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                            { "max_level_bonus", 0 },
                                            { "level", 0 },
                                            { "item_seen", true },
                                            { "xp", 0 },
                                            { "favorite", false },
                                            { "granted_bundles", ResponseIgIdrk.ToArray() }
                                        },
                                        quantity = 1,
                                    };

                                    ProfileChange.Profile.items.Add(ResponseId, AthenaItemDynamicData);
                                }
                            }

                            //foreach(var key in WeeklyQuestManager.BPSeasonBundleScheduleDictionary)
                            //{
                            //    Console.WriteLine(key.Key);
                            //}

                            if (WeeklyQuestManager.BPSeasonBundleScheduleDictionary.TryGetValue($"Season{seasonObject.SeasonNumber}", out List<WeeklyQuestsJson> BPQuestsArray))
                            {
                                //Console.WriteLine(seasonObject.SeasonNumber);
                                //Console.WriteLine(seasonObject.BookPurchased);
                                if (seasonObject.BookPurchased)
                                {
                                    if (BPQuestsArray.Count > 0)
                                    {
                                        var ResponseIG = new Dictionary<string, List<string>>();
                                        //List<string> ResponseIgIdrk = new List<string>();
                                        // var ResponseId = "";
                                        foreach (var kvp in BPQuestsArray)
                                        {
                                            if (!kvp.BundleRequired.QuestBundleID) continue;
                                            if (kvp.BundleRequired.RequiredLevel > seasonObject.Level) continue;

                                            List<string> FindFirstOrDe = ResponseIG.FirstOrDefault(e => e.Key == kvp.BundleSchedule).Value;
                                            if (FindFirstOrDe == null || FindFirstOrDe.Count() == 0)
                                            {
                                                ResponseIG[kvp.BundleSchedule] = new List<string> { $"ChallengeBundle:{kvp.BundleId}" };
                                                //ResponseIG.Add(kvp.BundleSchedule, new List<string>
                                                //{
                                                //    kvp.BundleId
                                                //});
                                            }
                                            else
                                            {
                                                FindFirstOrDe.Add($"ChallengeBundle:{kvp.BundleId}");
                                            }
                                            // kvp.BundleSchedule
                                            List<string> TEST2FRFR = new List<string>();
                                            foreach (var test in kvp.BundlesObject)
                                            {
                                                if (!test.quest_data.RequireBP) continue;

                                                TEST2FRFR.Add(test.templateId);
                                            }



                                            var AthenaItemBPData = new AthenaItemDynamic
                                            {
                                                templateId = $"ChallengeBundle:{kvp.BundleId}",
                                                attributes = new Dictionary<string, object>
                                                {
                                                    { "has_unlock_by_completion", false },
                                                    { "num_quests_completed", 0 },
                                                    { "level", 0 },
                                                    { "grantedquestinstanceids", TEST2FRFR.ToArray() },
                                                    { "item_seen",  true },
                                                    { "max_allowed_bundle_level", 0 },
                                                    { "num_granted_bundle_quests", TEST2FRFR.Count() },
                                                    { "max_level_bonus", 0 },
                                                    { "challenge_bundle_schedule_id", kvp.BundleSchedule },
                                                    { "num_progress_quests_completed", 0 },
                                                    { "xp", 0 },
                                                    { "favorite", false }
                                                },
                                                quantity = 1,
                                            };

                                            ProfileChange.Profile.items.Add($"ChallengeBundle:{kvp.BundleId}", AthenaItemBPData);

                                        }

                                        foreach (var kvp in ResponseIG)
                                        {
                                            var AthenaItemBPData = new AthenaItemDynamic
                                            {
                                                templateId = kvp.Key,
                                                attributes = new Dictionary<string, object>
                                                {
                                                    { "unlock_epoch", DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                                                    { "max_level_bonus", 0 },
                                                    { "level", 0 },
                                                    { "item_seen", true },
                                                    { "xp", 0 },
                                                    { "favorite", false },
                                                    { "granted_bundles", kvp.Value.ToArray() }
                                                },
                                                quantity = 1,
                                            };

                                            ProfileChange.Profile.items.Add(kvp.Key, AthenaItemBPData);
                                        }

                                    }
                                }
                            }



                            foreach (var kvp in seasonObject.Quests)
                            {
                                var Value = kvp.Value;
                               // Console.WriteLine(Value.templateId);
     
                                if (Value.templateId.Contains("Quest:"))
                                {
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
                                        //Console.WriteLine(e.Name);
                                        AthenaItemDynamicData.attributes.Add(e.Name, e.Value);
                                    });


                                    ProfileChange.Profile.items.Add(kvp.Key, AthenaItemDynamicData);
                                }
                                //else if()
                                //{

                                //}
                            }

                            foreach (var kvp in seasonObject.DailyQuests.Daily_Quests)
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
                                foreach (var profileChange in Saved.BackendCachedData.DefaultBanners_Items)
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
                        }
                        else
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
