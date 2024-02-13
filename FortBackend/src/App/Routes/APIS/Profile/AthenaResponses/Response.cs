using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using FortBackend.src.App.Utilities.MongoDB.Module;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using static FortBackend.src.App.Routes.APIS.Profile.AthenaResponses.Class;

namespace FortBackend.src.App.Routes.APIS.Profile.AthenaResponses
{
    public class Response
    {
        public static async Task<Class.Athena> AthenaResponse(string AccountId, string ProfileId, int Season, string RVN, Account AthenaDataParsed)
        {
            try
            {
                bool FoundSeasonDataInProfile = false;
                foreach (Season SeasonData in AthenaDataParsed.commoncore.Seasons)
                {
                    Console.WriteLine(SeasonData.SeasonNumber);
                    Console.WriteLine(Season);
                    Console.WriteLine(SeasonData.SeasonNumber == Season);
                    if (SeasonData.SeasonNumber == Season)
                    {
                        Console.WriteLine("FOUND");
                        FoundSeasonDataInProfile = true;
                    }
                }

                if (!FoundSeasonDataInProfile)
                {
                    string seasonJson = JsonConvert.SerializeObject(new Season
                    {
                        SeasonNumber = Season,
                        BookLevel = 1,
                        BookXP = 0,
                        BookPurchased = false,
                        Quests = new List<Dictionary<string, object>>(),
                        BattleStars = 0,
                        DailyQuests = new DailyQuests
                        {
                            Interval = "0001-01-01T00:00:00.000Z",
                            Rerolls = 1
                        }
                    });

                    await Handlers.PushOne<Account>("accountId", AccountId, new Dictionary<string, object>
                    {
                        {
                            "commoncore.Season", BsonDocument.Parse(seasonJson)
                        }
                    });
                }

              
                AthenaDataParsed = JsonConvert.DeserializeObject<Account[]>(await Handlers.FindOne<Account>("accountId", AccountId))[0];
              
                if (AthenaDataParsed == null)
                {
                    return new Class.Athena();
                }

                Console.WriteLine(AthenaDataParsed.commoncore.Seasons);

                Season[] Seasons = AthenaDataParsed.commoncore.Seasons;

                if (AthenaDataParsed.commoncore.Seasons != null)
                {
                    foreach (Season seasonObject in Seasons)
                    {
                        Console.WriteLine(seasonObject.SeasonNumber);
                        Console.WriteLine(Season);
                        if (seasonObject.SeasonNumber == Season)
                        {
                            Console.WriteLine("CORRECT SEASON!");
                            DailyQuests quest_manager = seasonObject.DailyQuests;
                            DateTime inputDateTime1;
                            if (DateTime.TryParseExact(quest_manager.Interval, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out inputDateTime1)) { }
                            Class.Athena AthenaClass = new Class.Athena()
                            {
                                profileRevision = int.Parse(AthenaDataParsed.athena.RVN.ToString() ?? "0"),
                                profileId = ProfileId,
                                profileChangesBaseRevision = AthenaDataParsed.athena.RVN,
                                profileChanges = new List<Class.ProfileChange>
                                {
                                    new ProfileChange
                                    {
                                         ChangeType = "fullProfileUpdate",
                                        _id = "RANDOM",
                                        Profile = new ProfileData
                                        {
                                            _id = "RANDOM",
                                            Update = "",
                                            Created = DateTime.Parse("2021-03-07T16:33:28.462Z"),
                                            Updated = DateTime.Parse("2021-05-20T14:57:29.907Z"),
                                            rvn = AthenaDataParsed.athena.RVN,
                                            WipeNumber = 1,
                                            accountId = AccountId,
                                            profileId = ProfileId,
                                            version = "no_version",
                                            stats = new Stats69
                                            {
                                                  attributes = new StatsAttributes
                                                  {
                                                    use_random_loadout = false,
                                                    past_seasons = new List<object>(),
                                                    season_match_boost = seasonObject.season_match_boost,
                                                    loadouts =  AthenaDataParsed.athena.loadouts,
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
                                                    level = seasonObject.Level,
                                                    rested_xp_exchange = 1,
                                                    rested_xp_cumulative = 0,
                                                    rested_xp_mult = 0,
                                                    season_friend_match_boost = seasonObject.season_friend_match_boost,
                                                    active_loadout_index = 0,
                                                    purchased_bp_offers = new List<object> { },
                                                    last_applied_loadout = AthenaDataParsed.athena.last_applied_loadout?.ToString() ?? "",
                                                    //favorite_musicpack = AthenaDataParsed.athena.MusicPack.Items?.ToString() ?? "",
                                                    //banner_icon = AthenaDataParsed.athena.Banner.BannerIcon?.ToString() ?? "",
                                                    //banner_color = AthenaDataParsed.athena.Banner.BannerColor?.ToString() ?? "",
                                                    //favorite_character = AthenaDataParsed.athena.Character.Items?.ToString() ?? "",
                                                    //favorite_itemwraps = AthenaDataParsed.athena.ItemWrap.Items ?? new string[0],
                                                    //favorite_skydivecontrail = AthenaDataParsed.athena.SkydiveContrail.Items?.ToString() ?? "",
                                                    //favorite_pickaxe = AthenaDataParsed.athena.Pickaxe.Items?.ToString() ?? "",
                                                    //favorite_glider = AthenaDataParsed.athena.Glider.Items?.ToString() ?? "",
                                                    //favorite_backpack = AthenaDataParsed.athena.Backpack.Items?.ToString() ?? "",
                                                    //favorite_dance = AthenaDataParsed.athena.Dance.Items ?? new string[0],
                                                    //favorite_loadingscreen = AthenaDataParsed.Athena.LoadingScreen.Items?.ToString() ?? ""
                                                }
                                            },
                                            items = new Dictionary<string, object>(),
                                            commandRevision = AthenaDataParsed.athena.CommandRevision,
                                        }
                                    
                                     }
                                },
                                profileCommandRevision = AthenaDataParsed.athena.CommandRevision,
                                serverTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                                responseVersion = 1,
                            };

                            List<Dictionary<string, object>> items = AthenaDataParsed.athena.Items;

                            foreach (Dictionary<string, object> item in items)
                            {
                                try
                                {
                                    string Key = "";
                                    object Value = "";

                                    foreach (KeyValuePair<string, object> KeyValuePair in item)
                                    {
                                        Key = KeyValuePair.Key;
                                        Value = KeyValuePair.Value;

                                        try
                                        {
                                            int AthenaIndex = Key.IndexOf("Athena");
                                            Key = Key.Substring(AthenaIndex);
                                        }
                                        catch {/*idk*/}
                                    }

                                    var itemValue = item[Key] as Dictionary<string, object>;

                                    if (itemValue == null)
                                    {
                                        if (Value is Newtonsoft.Json.Linq.JObject)
                                        {
                                            dynamic itemAttributes1 = JsonConvert.DeserializeObject(Value.ToString());
                                            Console.WriteLine(Value);

                                            // Loadouts
                                            if (itemAttributes1.templateId != null && itemAttributes1.templateId == "CosmeticLocker:cosmeticlocker_athena")
                                            {
                                                Loadout itemAttributes = JsonConvert.DeserializeObject<Loadout>(Value.ToString());

                                                if (itemAttributes != null)
                                                {
                                                    AthenaClass.profileChanges[0].Profile.items.Add(Key, itemAttributes);
                                                }
                                            }
                                            else
                                            {
                                                // Items
                                                AthenaItem ItemAttributes = JsonConvert.DeserializeObject<AthenaItem>(Value.ToString()); 

                                                if(ItemAttributes != null)
                                                {
                                                    AthenaClass.profileChanges[0].Profile.items.Add(Key, ItemAttributes);
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
            }
            catch (Exception ex)
            {
                Logger.Error($"AthenaResponse: {ex.Message}");
            }

            return new Class.Athena();
        }
    }
}
