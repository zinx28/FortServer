using FortLibrary.EpicResponses.Profile.Query.Items;
using Newtonsoft.Json;

namespace FortLibrary.EpicResponses.Profile.Quests
{
    public class DailyQuestsData
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public DailyQuestsDataDB attributes { get; set; } = new DailyQuestsDataDB();
        [JsonProperty("quantity")]
        public int quantity { get; set; } = 1;
    }

    public class DailyQuestsDataDB
    {
        [JsonProperty("creation_time")]
        public string creation_time { get; set; } = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [JsonProperty("unlock_epoch")]
        public string unlock_epoch { get; set; } = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [JsonProperty("level")]
        public int level { get; set; } = -1;

        [JsonProperty("item_seen")]
        public bool item_seen { get; set; } = false;

        [JsonProperty("grantedquestinstanceids")]
        public string[] grantedquestinstanceids { get; set; } = new string[0];

        [JsonProperty("playlists")]
        public List<object> playlists { get; set; } = new List<object>();

        [JsonProperty("sent_new_notification")]
        public bool sent_new_notification { get; set; } = false;

        [JsonProperty("challenge_bundle_id")]
        public string challenge_bundle_id { get; set; } = "";

        [JsonProperty("xp_reward_scalar")]
        public int xp_reward_scalar { get; set; } = 1;

        [JsonProperty("challenge_linked_quest_given")]
        public string challenge_linked_quest_given { get; set; } = "";

        [JsonProperty("quest_pool")]
        public string quest_pool { get; set; } = "";

        [JsonProperty("quest_state")]
        public string quest_state { get; set; } = "active";

        [JsonProperty("bucket")]
        public string bucket { get; set; } = "";

        [JsonProperty("last_state_change_time")]
        public string last_state_change_time { get; set; } = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [JsonProperty("challenge_linked_quest_parent")]
        public string challenge_linked_quest_parent { get; set; } = "";

        [JsonProperty("max_level_bonus")]
        public int max_level_bonus { get; set; } = 0;

        [JsonProperty("max_allowed_bundle_level")]
        public int max_allowed_bundle_level { get; set; } = 0;

        [JsonProperty("has_unlock_by_completion")]
        public bool has_unlock_by_completion { get; set; } = false;

        [JsonProperty("num_granted_bundle_quests")]
        public int num_granted_bundle_quests { get; set; } = 0;

        [JsonProperty("num_quests_completed")]
        public int num_quests_completed { get; set; } = 0;

        [JsonProperty("num_progress_quests_completed")]
        public int num_progress_quests_completed { get; set; } = 0;

        [JsonProperty("xp")]
        public int xp { get; set; } = 0;

        [JsonProperty("quest_rarity")]
        public string quest_rarity { get; set; } = "uncommon";

        [JsonProperty("favorite")]
        public bool favorite { get; set; } = false;


        // THIS IS NORMALLY DYNAMIC BUT BaCkEnD hAtEs ThAt IdEa

        public List<DailyQuestsObjectiveStates> ObjectiveState { get; set; } = new List<DailyQuestsObjectiveStates>();
    }

    public class DailyQuestsObjectiveStates
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; } = 0;
        public int MaxValue { get; set; } = 0;
    }

    public class DailyQuestsDataAttributes
    {
        [JsonProperty("creation_time")]
        public string creation_time { get; set; } = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        [JsonProperty("level")]
        public int level { get; set; } = -1;

        [JsonProperty("item_seen")]
        public bool item_seen { get; set; } = false;

        [JsonProperty("playlists")]
        public List<object> playlists { get; set; } = new List<object>();

        [JsonProperty("sent_new_notification")]
        public bool sent_new_notification { get; set; } = false;

        [JsonProperty("challenge_bundle_id")]
        public string challenge_bundle_id { get; set; } = "";

        [JsonProperty("xp_reward_scalar")]
        public int xp_reward_scalar { get; set; } = 1;

        [JsonProperty("challenge_linked_quest_given")]
        public string challenge_linked_quest_given { get; set; } = "";

        [JsonProperty("quest_pool")]
        public string quest_pool { get; set; } = "";

        [JsonProperty("quest_state")]
        public string quest_state { get; set; } = "active";

        [JsonProperty("bucket")]
        public string bucket { get; set; } = "";

        [JsonProperty("last_state_change_time")]
        public string last_state_change_time { get; set; } = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        
        [JsonProperty("challenge_linked_quest_parent")]
        public string challenge_linked_quest_parent { get; set; } = "";

        [JsonProperty("max_level_bonus")]
        public int max_level_bonus { get; set; } = 0;

        [JsonProperty("xp")]
        public int xp { get; set; } = 0;

        [JsonProperty("quest_rarity")]
        public string quest_rarity { get; set; } = "uncommon";

        [JsonProperty("favorite")]
        public bool favorite { get; set; } = false;
    }
}
