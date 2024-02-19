using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Purchases;
using FortBackend.src.App.Utilities.Classes.EpicResponses.Profile.Query;
using System.Collections.Generic;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile
{
    public class Mcp
    {
        public int profileRevision { get; set; } = 0;
        public string profileId { get; set; }
        public int profileChangesBaseRevision { get; set; } = 1;
        public List<dynamic> profileChanges { get; set; } = new List<dynamic>();
        public List<object> multiUpdate { get; set; } = new List<object>();
        public List<McpNotifications> notifications { get; set; } = new List<McpNotifications>();
        public DateTime serverTime { get; set; }

        public int profileCommandRevision { get; set; } = 1;
        public int responseVersion { get; set; } = 1;
    }

    public class McpNotifications
    {
        public string type { get; set; }
        public bool primary { get; set; }
        public LootResultClass lootResult { get; set; } = new LootResultClass();
    }

    public class LootResultClass
    {
        public List<NotificationsItemsClass> items { get; set; } = new List<NotificationsItemsClass>();
    }
}
