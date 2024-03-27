using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Items;
using FortBackend.src.App.Utilities.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

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
    }

    public class LockerSlotData
    {
        public Slots musicpack { get; set; } = new Slots();
        public Slots character { get; set; } = new Slots();
        public Slots backpack { get; set; } = new Slots();
        public Slots pickaxe { get; set; } = new Slots();
        public Slots skydivecontrail { get; set; } = new Slots();
        public Slots dance { get; set; } = new Slots();
        public Slots loadingscreen { get; set; } = new Slots();
        public Slots glider { get; set; } = new Slots();
        public Slots itemwrap { get; set; } = new Slots();
    }

    public class Loadouts
    {
        public SandboxLoadoutSlots locker_slots_data { get; set; } = new SandboxLoadoutSlots();
        public string banner_icon_template { get; set; } = string.Empty;
        public string banner_color_template { get; set; } = string.Empty;
        public string locker_name { get; set; } = string.Empty;
        public bool favorite { get; set; } = false;
        public int use_count { get; set; } = 1;

        public bool item_seen { get; set; } = false;
    }

    public class Athena
    {
        [BsonElement("Updated")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        [BsonElement("items")]
        //[BsonIgnoreIfNull]
        public Dictionary<string, AthenaItem> Items { get; set; }
        //public List<ProfileItem> Items { get; set; } = new List<ProfileItem>();


        [BsonElement("loadouts_data")]
        //[JsonProperty("loadouts_data")] // we will now use loadouts for this.. this will be 
        public Dictionary<string, SandboxLoadout> loadouts_data { get; set; }


        [BsonElement("loadouts")]
        public string[] loadouts { get; set; } = new string[]
        {
            "sandbox_loadout"
        };


        [BsonElement("last_applied_loadout")]
        public string last_applied_loadout { get; set; } = "sandbox_loadout";

        [BsonElement("battlestars")]
        public int BattleStars { get; set; } = 0;

        [BsonElement("gold")]
        public int Gold { get; set; } = 0;

        [BsonElement("omnichips")]
        public int OmniChips { get; set; } = 0;

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
        public Dictionary<string, CommonCoreItem> Items { get; set; } = new Dictionary<string, CommonCoreItem>();

        [BsonElement("Season")]
        [BsonIgnoreIfNull]
        public List<SeasonClass> Seasons { get; set; } = new List<SeasonClass>()
        {
            new SeasonClass {
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

        [BsonElement("current_mtx_platform")]
        public string current_mtx_platform { get; set; } = "EpicPC";

        [BsonElement("mtx_affiliate")]
        public string mtx_affiliate { get; set; } = "";

        [BsonElement("mtx_purchase_history")]
        public MtxPurchaseHistory mtx_purchase_history { get; set; } = new MtxPurchaseHistory();

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

    public class ProfileItem
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = "NOTOWRKING";

        [JsonProperty("attributes")]
        public object attributes { get; set; } = new object { };

        [JsonProperty("quantity")]
        public int quantity { get; set; } = 0;

        //public ProfileItem()
        //{
        //    attributes = new object();
        //}
    }

    public class MtxPurchaseHistory
    {
        [BsonElement("refundsUsed")]
        public int refundsUsed { get; set; } = 0;

        [BsonElement("refundCredits")]
        public int refundCredits { get; set; } = 0;

        [BsonElement("purchases")]
        [BsonIgnoreIfNull]
        public List<object> purchases { get; set; } = new List<object>();

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
    public class SeasonClass
    {
        [BsonElement("season")]
        [JsonProperty("season")]
        public int SeasonNumber { get; set; } = 12;

        [BsonElement("events")]
        [JsonProperty("events")]
        public Events events { get; set; } = new Events();

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

    public class Events
    {
        [BsonElement("persistentScores")]
        [JsonProperty("persistentScores")]
        public PersistentScores persistentScores { get; set; } = new PersistentScores();
        [BsonElement("tokens")]
        [JsonProperty("tokens")]
        public string[] tokens { get; set; } = new string[0];
    }

    public class PersistentScores
    {
        [BsonElement("Hype")]
        [JsonProperty("Hype")]
        public int Hype { get; set; } = 0;
    }
}
