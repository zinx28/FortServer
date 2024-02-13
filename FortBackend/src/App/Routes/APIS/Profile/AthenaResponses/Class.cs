namespace FortBackend.src.App.Routes.APIS.Profile.AthenaResponses
{
    public class Class
    {
        public class ProfileChange
        {
            public string ChangeType { get; set; } = "fullProfileUpdate";
            public string _id { get; set; } = "RANDOM";
            public ProfileData Profile { get; set; }
        }

        public class ProfileData
        {
            public string _id { get; set; } = "RANDOM";
            public string Update { get; set; } = "";
            public DateTime Created { get; set; } = DateTime.Parse("2021-03-07T16:33:28.462Z");
            public DateTime Updated { get; set; } = DateTime.Parse("2021-05-20T14:57:29.907Z");
            public int rvn { get; set; }
            public int WipeNumber { get; set; } = 1;
            public string accountId { get; set; } = "0";
            public string profileId { get; set; } = "notproper";
            public string version { get; set; } = "no_version";
            public Stats69 stats { get; set; } = new Stats69();
            public Dictionary<string, object> items { get; set; } = new Dictionary<string, object>();
            public int commandRevision { get; set; } = 5;
        }

        public class Items
        {
            public SandboxLoadout sandbox_loadout { get; set; } = new SandboxLoadout();
            public Loadout loadout_1 { get; set; } = new Loadout();
            public AthenaPickaxe DefaultPickaxe { get; set; } = new AthenaPickaxe();
            public AthenaGlider DefaultGlider { get; set; } = new AthenaGlider();
            public AthenaDance EID_DanceMoves { get; set; } = new AthenaDance();
        }

        public class SandboxLoadout
        {
            public string templateId { get; set; } = "0";
            public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
            public int quantity { get; set; } = 0;
        }

        public class SandboxLoadoutSlots
        {
            public LockerSlotsData slots { get; set; } = new LockerSlotsData();
        }

        public class SandboxLoadoutAttributes
        {
            public SandboxLoadoutSlots locker_slots_data { get; set; } = new SandboxLoadoutSlots();
            public int use_count { get; set; }
            public string banner_icon_template { get; set; } = "notproper";
            public string banner_color_template { get; set; } = "notproper";
            public string locker_name { get; set; } = "notproper";
            public bool item_seen { get; set; }
            public bool favorite { get; set; }
        }

        public class LockerSlotsData
        {
            public Slots69 MusicPack { get; set; } = new Slots69();
            public Slots Character { get; set; } = new Slots();
            public Slots Backpack { get; set; } = new Slots();
            public Slots SkyDiveContrail { get; set; } = new Slots();
            public Slots Dance { get; set; } = new Slots();
            public Slots LoadingScreen { get; set; } = new Slots();
            public Slots Pickaxe { get; set; } = new Slots();
            public Slots Glider { get; set; } = new Slots();
            public Slots ItemWrap { get; set; } = new Slots();
        }

        public class Slots
        {
            public List<string> items { get; set; } = new List<string>();
            public List<string> activeVariants { get; set; } = new List<string>();
        }

        public class Slots69
        {
            public List<string> items { get; set; } = new List<string>();
        }

        public class Loadout
        {
            public string templateId { get; set; } = "notproper";
            public SandboxLoadoutAttributes attributes { get; set; } = new SandboxLoadoutAttributes();
            public int quantity { get; set; }
        }

        public class AthenaPickaxe
        {
            public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
            public string templateId { get; set; } = "notproper";
        }

        public class AthenaGlider
        {
            public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
            public string templateId { get; set; } = "notproper";
        }

        public class AthenaDance
        {
            public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
            public string templateId { get; set; } = "notproper";
        }


        public class AthenaItem
        {
            public AthenaItemAttributes attributes { get; set; } = new AthenaItemAttributes();
            public string templateId { get; set; } = "notproper";
        }

        public class AthenaItemAttributes
        {
            public bool favorite { get; set; }
            public bool item_seen { get; set; }
            public int level { get; set; }
            public int max_level_bonus { get; set; }
            public int rnd_sel_cnt { get; set; }
            public List<object> variants { get; set; } = new List<object>();
            public int xp { get; set; }
        }

        public class Stats69
        {
            public StatsAttributes attributes { get; set; } = new StatsAttributes();
        }

        public class StatsAttributes
        {
            public bool intro_game_played { get; set; }
            public int inventory_limit_bonus { get; set; }
            public bool allowed_to_send_gifts { get; set; }
            public bool mfa_enabled { get; set; }
            public bool allowed_to_receive_gifts { get; set; }
            public bool use_random_loadout { get; set; }
            public List<object> past_seasons { get; set; } = new List<object>();
            public int season_match_boost { get; set; }
            public string[] loadouts { get; set; }
            public bool mfa_reward_claimed { get; set; }
            public int rested_xp_overflow { get; set; }
            public string last_xp_interaction { get; set; } = "notproper";
            public object quest_manager { get; set; } = new object();
            public int book_level { get; set; }
            public int season_num { get; set; }
            public int book_xp { get; set; }
            public object creative_dynamic_xp { get; set; } = "notproper";
            public SeasonStats season { get; set; } = new SeasonStats();
            public int battlestars { get; set; }
            public object vote_data { get; set; } = new object();
            public int battlestars_season_total { get; set; }
            public int lifetime_wins { get; set; }
            public bool book_purchased { get; set; }
            public int rested_xp_exchange { get; set; }
            public int level { get; set; }
            public int rested_xp { get; set; }
            public double rested_xp_mult { get; set; }
            public int accountLevel { get; set; }
            public int rested_xp_cumulative { get; set; }
            public int xp { get; set; }
            public int season_friend_match_boost { get; set; }
            public int active_loadout_index { get; set; }
            public List<object> purchased_bp_offers { get; set; } = new List<object>();
            public string last_applied_loadout { get; set; } = "notproper";
            public string favorite_musicpack { get; set; } = "notproper";
            public string banner_icon { get; set; } = "notproper";
            public string[] favorite_itemwraps { get; set; } = new string[0];
            public string favorite_skydivecontrail { get; set; } = "notproper";
            public string favorite_pickaxe { get; set; } = "notproper";
            public string favorite_glider { get; set; } = "notproper";
            public string favorite_backpack { get; set; } = "notproper";
            public string[] favorite_dance { get; set; } = new string[0];
            public string favorite_loadingscreen { get; set; } = "notproper";
            public string banner_color { get; set; } = "notproper";
            public string favorite_character { get; set; } = "notproper";
        }

        public class SeasonStats
        {
            public int numWins { get; set; }
            public int numHighBracket { get; set; }
            public int numLowBracket { get; set; }
        }

        public class Athena
        {
            public int profileRevision { get; set; }
            public string profileId { get; set; }
            public int profileChangesBaseRevision { get; set; }
            public List<ProfileChange> profileChanges { get; set; } = new List<ProfileChange>();
            public DateTime serverTime { get; set; }
            public int profileCommandRevision { get; set; } = 0;
            public int responseVersion { get; set; } = 1;
        }
    }
}
