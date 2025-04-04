﻿using FortLibrary.EpicResponses.Profile.Purchases;
using System.Collections.Generic;

namespace FortLibrary.EpicResponses.Profile
{
    public class Mcp
    {
        public int profileRevision { get; set; } = 0;
        public string profileId { get; set; } = string.Empty;
        public int profileChangesBaseRevision { get; set; } = 1;
        public List<dynamic> profileChanges { get; set; } = new List<dynamic>();
     
        public List<dynamic>? notifications { get; set; } = new List<dynamic>();

        public int profileCommandRevision { get; set; } = 1;
        public string serverTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        public List<object> multiUpdate { get; set; } = new List<object>();
        public int responseVersion { get; set; } = 1;
    }

    public class McpNotifications
    {
        public string type { get; set; } = string.Empty;
        public bool primary { get; set; }
        public LootResultClass lootResult { get; set; } = new LootResultClass();
    }

    public class LootResultClass
    {
        public List<NotificationsItemsClass> items { get; set; } = new List<NotificationsItemsClass>();
    }
}
