using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query.Attributes
{
    public class CommonCoreStatsAttributes
    {
        public object personal_offers { get; set; }
        public bool intro_game_played { get; set; }
        public object import_friends_played { get; set; }
        public string mtx_affiliate { get; set; }
        public List<string> undo_cooldowns { get; set; }
        public string mtx_affiliate_set_time { get; set; }
        public object import_friends_claimed { get; set; }
        public object mtx_purchase_history { get; set; }
        public int inventory_limit_bonus { get; set; }
        public string current_mtx_platform { get; set; }
        public List<Dictionary<string, object>> weekly_purchases { get; set; }
        public List<Dictionary<string, object>> daily_purchases { get; set; }
        public object ban_history { get; set; }
        public object in_app_purchases { get; set; }
        public List<Dictionary<string, object>> permissions { get; set; }
        public string undo_timeout { get; set; }
        public object monthly_purchases { get; set; }
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
