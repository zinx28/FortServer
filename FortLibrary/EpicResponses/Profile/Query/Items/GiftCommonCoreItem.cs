using FortLibrary.EpicResponses.Profile.Purchases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile.Query.Items
{
    public class GiftCommonCoreItem
    {
        [JsonProperty("templateId")]
        public string templateId { get; set; } = string.Empty;
        [JsonProperty("attributes")]
        public GiftCommonCoreItemAttributes attributes { get; set; } = new GiftCommonCoreItemAttributes();
        [JsonProperty("quantity")]
        public int quantity { get; set; } = 1;
    }

    public class GiftCommonCoreItemAttributes
    {

        [JsonProperty("max_level_bonus")]
        public int max_level_bonus { get; set; } = 1;

        [JsonProperty("fromAccountId")]
        public string fromAccountId { get; set; } = "Server"; // default

        [JsonProperty("lootList")]
        public List<NotificationsItemsClassOG> lootList { get; set; } = new List<NotificationsItemsClassOG>();
    }
}
