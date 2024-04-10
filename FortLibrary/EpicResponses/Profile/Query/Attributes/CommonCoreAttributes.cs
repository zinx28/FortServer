using Newtonsoft.Json.Linq;

namespace FortLibrary.EpicResponses.Profile.Query.Attributes
{
    public class CommonCoreStatsAttributes
    {
        public object personal_offers { get; set; } = new object();
        public bool intro_game_played { get; set; }
        public object import_friends_played { get; set; } = new object();
        public string mtx_affiliate { get; set; } = string.Empty;
        public List<string> undo_cooldowns { get; set; } = new List<string>();
        public string mtx_affiliate_set_time { get; set; } = string.Empty;
        public object import_friends_claimed { get; set; } = new object();
        public object mtx_purchase_history { get; set; } = new object();
        public int inventory_limit_bonus { get; set; }
        public string current_mtx_platform { get; set; } = string.Empty;
        public List<Dictionary<string, object>> weekly_purchases { get; set; } = new List<Dictionary<string, object>>();
        public List<Dictionary<string, object>> daily_purchases { get; set; } = new List<Dictionary<string, object>>();
        public object ban_history { get; set; } = new object();
        public object in_app_purchases { get; set; } = new object();
        public List<Dictionary<string, object>> permissions { get; set; } = new List<Dictionary<string, object>>();
        public string undo_timeout { get; set; } = string.Empty;
        public object monthly_purchases { get; set; } = new object();
        public bool allowed_to_send_gifts { get; set; }
        public bool mfa_enabled { get; set; }
        public bool allowed_to_receive_gifts { get; set; }
        public object gift_history { get; set; } = new object[0];
    }

    public class MtxPurchaseHistory
    {
        public int refundsUsed { get; set; } = 0;

        public int refundCredits { get; set; } = 0;

        public List<object> purchases { get; set; } = new List<object>();

    }
}
