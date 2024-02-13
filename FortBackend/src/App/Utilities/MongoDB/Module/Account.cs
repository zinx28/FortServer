using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.MongoDB.Module
{
    [BsonIgnoreExtraElements]
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("accountId")]
        public string AccountId { get; set; } = string.Empty;

        [BsonElement("DiscordId")]
        public string DiscordId { get; set; } = string.Empty;


        [BsonElement("athena")]
        public Athena athena { get; set; } = new Athena();

        [BsonElement("commoncore")]
        public CommonCore commoncore { get; set; } = new CommonCore();

        // last !

        [BsonElement("accessToken")]
        public string[] AccessToken { get; set; } = new string[0];

        [BsonElement("refreshToken")]
        public string[] RefreshToken { get; set; } = new string[0];

        [BsonElement("clientToken")]
        public string[] ClientToken { get; set; } = new string[0];
    }

    public class Athena
    {
        [BsonElement("Updated")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        [BsonElement("items")]
        [BsonIgnoreIfNull]
        public List<Dictionary<string, object>> Items { get; set; } = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>
            {
                ["sandbox_loadout"] = new
                {
                    templateId = "CosmeticLocker:cosmeticlocker_athena",
                    attributes = new
                    {
                        locker_slots_data = new
                        {
                            slots = new
                            {
                                musicpack = new
                                {
                                    items = new List<string> { "" }
                                },
                                character = new
                                {
                                    items = new List<string> { "" },
                                    ActiveVariants = new string[0]
                                },
                                backpack = new
                                {
                                    items = new List<string> { "" }
                                },
                                pickaxe = new
                                {
                                    items = new List<string> { "" }
                                },
                                skydivecontrail = new
                                {
                                    items = new List<string> { "" }
                                },
                                dance = new
                                {
                                    items = new string[]
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
                                loadingscreen = new
                                {
                                    items = new List<string> { "" }
                                },
                                glider = new
                                {
                                    items = new List<string> { "" }
                                },
                                itemwrap = new
                                {
                                   items = new string[]
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
                            },
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
            },
            new Dictionary<string, object>
            {
                ["AthenaPickaxe:DefaultPickaxe"] = new
                {
                    attributes = new
                    {
                        favorite = false,
                        item_seen = true,
                        level = 0,
                        max_level_bonus = 0,
                        rnd_sel_cnt = 0,
                        variants = new List<object>(),
                        xp = 0,
                    },
                    templateId = "AthenaPickaxe:DefaultPickaxe",
                    quantity = 1
                }
            },
            new Dictionary<string, object>
            {
                ["AthenaGlider:DefaultGlider"] = new
                {
                    attributes = new
                    {
                        favorite = false,
                        item_seen = true,
                        level = 0,
                        max_level_bonus = 0,
                        rnd_sel_cnt = 0,
                        variants = new List<object>(),
                        xp = 0,
                    },
                    templateId = "AthenaGlider:DefaultGlider",
                    quantity = 1
                }
            },
            new Dictionary<string, object>
            {
                ["AthenaDance:EID_DanceMoves"] = new
                {
                    attributes = new
                    {
                        favorite = false,
                        item_seen = true,
                        level = 0,
                        max_level_bonus = 0,
                        rnd_sel_cnt = 0,
                        variants = new List<object>(),
                        xp = 0,
                    },
                    templateId = "AthenaDance:EID_DanceMoves",
                    quantity = 1
                }
            }
        };

        [BsonElement("loadouts")]
        public string[] loadouts { get; set; } = new string[]
        {
            "sandbox_loadout"
        };


        [BsonElement("last_applied_loadout")]
        public string last_applied_loadout { get; set; } = "sandbox_loadout";

        [BsonElement("battlestars")]
        public int BattleStars { get; set; } = 500;

        [BsonElement("gold")]
        public int Gold { get; set; } = 500;

        [BsonElement("omnichips")]
        public int OmniChips { get; set; } = 500;

        [BsonElement("RVN")]
        public int RVN { get; set; } = 1;

        [BsonElement("CommandRevision")]
        public int CommandRevision { get; set; } = 0;
    }

    public class CommonCore
    {
        [BsonElement("Updated")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        [BsonElement("items")]
        [BsonIgnoreIfNull]
        public List<Dictionary<string, object>> Items { get; set; } = new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object>
            {
                ["Currency"] = new
                {
                    templateId = "Currency:MtxPurchased",
                    attributes = new
                    {
                        platform = "EpicPC"
                    },
                    quantity = 1000
                }
            }
        };


        [BsonElement("Season")]
        [BsonIgnoreIfNull]
        public Season[] Seasons { get; set; } = new Season[]
        {
            new Season {
                SeasonNumber = 12,
                Level = 1,
                BookLevel = 1,
                BattleStars = 0,
                BookXP = 0,
                BookPurchased = false,
                Quests = new List<Dictionary<string, object>>(),
                PinnedQuests = new List<Dictionary<string, object>>(),
                DailyQuests = new DailyQuests(),
                season_friend_match_boost = 0,
                season_match_boost = 0,
                intro_game_played = false,
                battlestars_currency = 0
            }
        };

        [BsonElement("ban_status")]
        [BsonIgnoreIfNull]
        public BanStatus BanStatus { get; set; } = new BanStatus();

        [BsonElement("ban_history")]
        [BsonIgnoreIfNull]
        public BanHistory BanHistory { get; set; } = new BanHistory();

        [BsonElement("mtx_affiliate")]
        public string mtx_affiliate { get; set; } = "";

        [BsonElement("weekly_purchases")]
        public List<Dictionary<string, object>> weekly_purchases { get; set; } = new List<Dictionary<string, object>>();

        [BsonElement("daily_purchases")]
        public List<Dictionary<string, object>> daily_purchases { get; set; } = new List<Dictionary<string, object>>();

        [BsonElement("allowed_to_send_gifts")]
        public bool allowed_to_send_gifts { get; set; } = true;

        [BsonElement("allowed_to_receive_gifts")]
        public bool allowed_to_receive_gifts { get; set; } = true;


        [BsonElement("RVN")]
        public int RVN { get; set; } = 1;

        [BsonElement("CommandRevision")]
        public int CommandRevision { get; set; } = 0;
    }

    public class BanHistory
    {
        [BsonElement("banCount")]
        public Dictionary<string, int> BanCount { get; set; } = new Dictionary<string, int>();

        [BsonElement("banTier")]
        public object BanTier { get; set; }
    }

    public class BanStatus
    {
        [BsonElement("bRequiresUserAck")]
        public bool RequiresUserAck { get; set; }

        [BsonElement("banReasons")]
        public List<string> BanReasons { get; set; } = new List<string>();

        [BsonElement("bBanHasStarted")]
        public bool BanHasStarted { get; set; }

        [BsonElement("banStartTimeUtc")]
        public DateTime BanStartTime { get; set; }

        [BsonElement("banDurationDays")]
        public double BanDurationDays { get; set; }

        [BsonElement("exploitProgramName")]
        public string ExploitProgramName { get; set; } = "NotProper";

        [BsonElement("additionalInfo")]
        public string AdditionalInfo { get; set; } = "NotProper";

        [BsonElement("competitiveBanReason")]
        public string CompetitiveBanReason { get; set; } = "NotProper";
    };
    public class DailyQuests
    {
        [BsonElement("dailyLoginInterval")]
        [JsonProperty("dailyLoginInterval")]
        public string Interval { get; set; } = DateTime.MinValue.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [BsonElement("dailyQuestRerolls")]
        [JsonProperty("dailyQuestRerolls")]
        public int Rerolls { get; set; } = 1;
    }
    public class Season
    {
        [BsonElement("season")]
        [JsonProperty("season")]
        public int SeasonNumber { get; set; } = 12;

        [BsonElement("season_match_boost")]
        [JsonProperty("season_match_boost")]
        public int season_match_boost = 0;

        [BsonElement("season_friend_match_boost")]
        [JsonProperty("season_friend_match_boost")]
        public int season_friend_match_boost = 0;

        [BsonElement("book_level")]
        [JsonProperty("book_level")]
        public int BookLevel { get; set; } = 1;

        [BsonElement("level")]
        [JsonProperty("level")]
        public int Level { get; set; } = 1;

        [BsonElement("battlestars_currency")]
        [JsonProperty("battlestars_currency")]
        public int battlestars_currency { get; set; } = 0;

        [BsonElement("book_xp")]
        [JsonProperty("book_xp")]
        public int BookXP { get; set; } = 0;

        [BsonElement("book_purchased")]
        [JsonProperty("book_purchased")]
        public bool BookPurchased { get; set; } = false;

        [BsonElement("lastclaimeditem")]
        [JsonProperty("lastclaimeditem")]
        public int lastclaimeditem { get; set; } = 0;

        [BsonElement("Quests")]
        [JsonProperty("Quests")]
        [BsonIgnoreIfNull]
        public List<Dictionary<string, object>> Quests { get; set; } = new List<Dictionary<string, object>>();

        [BsonElement("PinnedQuests")]
        [JsonProperty("PinnedQuests")]
        [BsonIgnoreIfNull]
        public List<Dictionary<string, object>> PinnedQuests { get; set; } = new List<Dictionary<string, object>>();

        [BsonElement("PinnedPartyQuests")]
        [JsonProperty("PinnedPartyQuests")]
        [BsonIgnoreIfNull]
        public List<Dictionary<string, object>> PinnedPartyQuests { get; set; } = new List<Dictionary<string, object>>();

        [BsonElement("quest_manager")]
        [JsonProperty("quest_manager")]
        public DailyQuests DailyQuests { get; set; } = new DailyQuests();

        [BsonElement("BattleStars")]
        [JsonProperty("BattleStars")]
        public int BattleStars { get; set; } = 0;

        [BsonElement("intro_game_played")]
        [JsonProperty("intro_game_played")]
        public bool intro_game_played { get; set; } = false;
    }
}
