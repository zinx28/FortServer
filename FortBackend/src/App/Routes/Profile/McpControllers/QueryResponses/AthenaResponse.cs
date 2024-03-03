using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Classes.ConfigHelpers;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
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
        public static async Task<Mcp> Grab(string AccountId, string ProfileId, VersionClass Season, int RVN, Account AccountDataParsed)
        {
            try
            {
                bool FoundSeasonDataInProfile = AccountDataParsed.commoncore.Seasons.Any(season => season.SeasonNumber == Season.Season);

                if (!FoundSeasonDataInProfile)
                {
                    string seasonJson = JsonConvert.SerializeObject(new SeasonClass
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
                        arena = new Arena
                        {
                            tokens = new string[] {
                                $"ARENA_S{Season.Season}_Division1"
                            }
                        }
                    });

                    await Handlers.PushOne<Account>("accountId", AccountId, new Dictionary<string, object>
                    {
                        {
                            "commoncore.Season", BsonDocument.Parse(seasonJson)
                        }
                    });
                }

                AccountDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", AccountId))[0];

                if (AccountDataParsed == null)
                {
                    return new Mcp();
                }

                SeasonClass[] Seasons = AccountDataParsed.commoncore.Seasons;

                if (AccountDataParsed.commoncore.Seasons != null)
                {
                    SeasonClass seasonObject = AccountDataParsed.commoncore.Seasons?.FirstOrDefault(season => season.SeasonNumber == Season.Season);

                    if (seasonObject != null)
                    {
                        if (AccountDataParsed.athena.RVN == AccountDataParsed.athena.CommandRevision)
                        {
                            AccountDataParsed.athena.RVN = +1;
                            //.Add($"athena.RVN", AccountDataParsed.athena.RVN + 1);
                            await Handlers.UpdateOne<Account>("accountId", AccountDataParsed.AccountId, new Dictionary<string, object>()
                            {
                                {
                                    $"athena.RVN", AccountDataParsed.athena.RVN + 1
                                }
                            });

                        }
                        Console.WriteLine("CORRECT SEASON!");
                        DailyQuests quest_manager = seasonObject.DailyQuests;
                        DateTime inputDateTime1;
                        if (DateTime.TryParseExact(quest_manager.Interval, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out inputDateTime1)) { }
                        Mcp AthenaClass = new Mcp()
                        {
                            profileRevision = AccountDataParsed.athena.RVN,
                            profileId = ProfileId,
                            profileChangesBaseRevision = AccountDataParsed.athena.RVN,
                            profileChanges = new List<object>()
                            {
                                new ProfileChange
                                {
                                    ChangeType = "fullProfileUpdate",
                                    Profile = new ProfileData
                                    {
                                        _id = AccountDataParsed.AccountId,
                                        Update = "",
                                        Created = DateTime.Parse("2021-03-07T16:33:28.462Z"),
                                        Updated = AccountDataParsed.athena.Updated,
                                        rvn = AccountDataParsed.athena.RVN,
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
                                                loadouts =  AccountDataParsed.athena.loadouts,
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
                                                active_loadout_index = Array.IndexOf(AccountDataParsed.athena.loadouts, AccountDataParsed.athena.last_applied_loadout),
                                                purchased_bp_offers = new List<object> { },
                                                last_applied_loadout = AccountDataParsed.athena.last_applied_loadout?.ToString() ?? "",
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
                                        commandRevision = AccountDataParsed.athena.CommandRevision,
                                    }
                                }
                            },
                            profileCommandRevision = AccountDataParsed.athena.CommandRevision,
                            serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                            responseVersion = 1,
                        };




                        List<Dictionary<string, object>> items = AccountDataParsed.athena.Items;

                        foreach (Dictionary<string, object> item in items)
                        {
                            try
                            {
                                string key = item.Keys.FirstOrDefault(k => k.Contains("Athena") || AccountDataParsed.athena.loadouts.Any(x => k.Contains(x))) ?? "";
                                if (item.TryGetValue(key, out object value) && value is Newtonsoft.Json.Linq.JObject)
                                {
                                    if (value.ToString() != null)
                                    {
                                        var ValueAsString = value.ToString();
                                        ItemThingygyydsf itemAttributes1 = JsonConvert.DeserializeObject<ItemThingygyydsf>(ValueAsString.ToLower());

                                        if (itemAttributes1 != null)
                                        {
                                            var ProfileChange = AthenaClass.profileChanges[0] as ProfileChange;
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
