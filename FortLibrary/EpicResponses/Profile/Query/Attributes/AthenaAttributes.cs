namespace FortLibrary.EpicResponses.Profile.Query.Attributes
{
    public class AthenaStatsAttributes
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
        public string last_xp_interaction { get; set; } = string.Empty;
        public object quest_manager { get; set; } = new object();
        public int book_level { get; set; }
        public int season_num { get; set; }
        public int book_xp { get; set; }
        public object creative_dynamic_xp { get; set; } = string.Empty;
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
        public string last_applied_loadout { get; set; } = string.Empty;
        public string favorite_musicpack { get; set; } = string.Empty;
        public string banner_icon { get; set; } = string.Empty;
        public List<string> favorite_itemwraps { get; set; } = new List<string>();
        public string favorite_skydivecontrail { get; set; } = string.Empty;
        public string favorite_pickaxe { get; set; } = string.Empty;
        public string favorite_glider { get; set; } = string.Empty;
        public string favorite_backpack { get; set; } = string.Empty;
        public List<string> favorite_dance { get; set; } = new List<string>();
        public string favorite_loadingscreen { get; set; } = string.Empty;
        public string banner_color { get; set; } = string.Empty;
        public string favorite_character { get; set; } = string.Empty;
    }

    public class SeasonStats
    {
        public int numWins { get; set; }
        public int numHighBracket { get; set; }
        public int numLowBracket { get; set; }
    }
}
