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
                            $"ARENA_S{Season}_Division1"
                        };
                    }

                    SeasonClass seasonJson = new SeasonClass
                    {
                        SeasonNumber = Season.Season,
                        BookLevel = 1,
                        BookXP = 0,
                        BookPurchased = false,
                        Quests = new List<Dictionary<string, object>>(),
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

                //AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", AccountId))[0];

                //if (AccountDataParsed == null)
                //{
                //    return new Mcp();
                //}

                List<SeasonClass> Seasons = profileCacheEntry.AccountData.commoncore.Seasons;

                if (profileCacheEntry.AccountData.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = profileCacheEntry.AccountData.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season);

                    if (seasonObject != null)
                    {
                        if (profileCacheEntry.AccountData.athena.RVN == profileCacheEntry.AccountData.athena.CommandRevision)
                        {
                            profileCacheEntry.AccountData.athena.RVN = +1;
                            //.Add($"athena.RVN", AccountDataParsed.athena.RVN + 1);
                            //await Handlers.UpdateOne<Account>("accountId", profileCacheEntry.AccountData.AccountId, new Dictionary<string, object>()
                            //{
                            //    {
                            //        $"athena.RVN", profileCacheEntry.AccountData.athena.RVN + 1
                            //    }
                            //});

                        }
                        Console.WriteLine("CORRECT SEASON!");
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
                                        Update = "",
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
                        List<Dictionary<string, object>> items = profileCacheEntry.AccountData.athena.Items;
                            
                        foreach (Dictionary<string, object> item in items)
                        {
                            try
                            {
                                string key = item.Keys.FirstOrDefault(k => k.Contains("Athena") || profileCacheEntry.AccountData.athena.loadouts.Any(x => k.Contains(x))) ?? "";
                                if (item.TryGetValue(key, out object value) && value is Newtonsoft.Json.Linq.JObject)
                                {
                                    if (value.ToString() != null)
                                    {
                                        var ValueAsString = value.ToString();
                                        ItemThingygyydsf itemAttributes1 = JsonConvert.DeserializeObject<ItemThingygyydsf>(ValueAsString.ToLower());

                                        if (itemAttributes1 != null)
                                        {
                                              
                                            if (itemAttributes1.templateId == "cosmeticlocker:cosmeticlocker_athena")
                                            {

                                                Loadout itemAttributes = JsonConvert.DeserializeObject<Loadout>(ValueAsString);

                                                if (ProfileChange != null && itemAttributes != null)
                                                {
                                                    ProfileChange.Profile.items.Add(key, itemAttributes);

                                                    if (key == "sandbox_loadout")
                                                    {
                                                        AthenaStatsAttributes AttributesFr = ProfileChange.Profile.stats.attributes as AthenaStatsAttributes;
                                                        if (AttributesFr != null)
                                                        {
                                                            AttributesFr.favorite_backpack = itemAttributes.attributes.locker_slots_data.slots.backpack.items[0];
                                                            AttributesFr.favorite_character = itemAttributes.attributes.locker_slots_data.slots.character.items[0];
                                                            AttributesFr.favorite_dance = itemAttributes.attributes.locker_slots_data.slots.dance.items;
                                                            AttributesFr.favorite_glider = itemAttributes.attributes.locker_slots_data.slots.glider.items[0];
                                                            AttributesFr.favorite_itemwraps = itemAttributes.attributes.locker_slots_data.slots.itemwrap.items;
                                                            AttributesFr.favorite_loadingscreen = itemAttributes.attributes.locker_slots_data.slots.loadingscreen.items[0];
                                                            AttributesFr.favorite_musicpack = itemAttributes.attributes.locker_slots_data.slots.musicpack.items[0];
                                                            AttributesFr.favorite_pickaxe = itemAttributes.attributes.locker_slots_data.slots.pickaxe.items[0];
                                                            AttributesFr.favorite_skydivecontrail = itemAttributes.attributes.locker_slots_data.slots.skydivecontrail.items[0];
                                                            AttributesFr.banner_color = itemAttributes.attributes.banner_color_template;
                                                            AttributesFr.banner_icon = itemAttributes.attributes.banner_icon_template;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                AthenaItem itemAttributes = JsonConvert.DeserializeObject<AthenaItem>(ValueAsString);

                                                if (ProfileChange != null && itemAttributes != null)
                                                {
                                                    ProfileChange.Profile.items.Add(key, itemAttributes);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                        }

                        if(ProfileChange != null )
                        {
                            int GrabPlacement3 = profileCacheEntry.AccountData.commoncore.Items.SelectMany((item, index) => new List<(Dictionary<string, object> Item, int Index)> { (Item: item, Index: index) })
                            .TakeWhile(pair => !pair.Item.ContainsKey("Currency")).Count();

                            if(GrabPlacement3 != -1)
                            {
                                var Value = profileCacheEntry.AccountData.commoncore.Items[GrabPlacement3]["Currency"];
                                dynamic itemAttributes1 = JsonConvert.DeserializeObject(Value.ToString());
                                if (itemAttributes1.templateId != null || Value != null)
                                {
                                    if (itemAttributes1.templateId == "Currency:MtxPurchased")
                                    {

                                        Loadout itemAttributes = JsonConvert.DeserializeObject<Loadout>(Value.ToString());
                                        Console.WriteLine(profileCacheEntry.AccountData.commoncore.Items[GrabPlacement3]);
                                        ProfileChange.Profile.items.Add("Currency", new
                                        {
                                            templateId = "Currency:MtxPurchased",
                                            attributes = new
                                            {
                                                platform = "Shared"
                                            },
                                            quantity = itemAttributes.quantity,
                                        });
                                    }
                                }
                            }
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
