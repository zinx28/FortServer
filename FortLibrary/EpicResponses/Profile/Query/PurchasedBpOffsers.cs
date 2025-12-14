using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile.Query
{
    public class PurchasedBpOffsers
    {
        [BsonElement("offerId")]
        [JsonProperty("offerId")]
        public string offerId { get; set; } = string.Empty;
        [BsonElement("bIsFreePassReward")]
        [JsonProperty("bIsFreePassReward")]
        public bool bIsFreePassReward { get; set; }
        [BsonElement("purchaseDate")]
        [JsonProperty("purchaseDate")]
        public string purchaseDate { get; set; } = string.Empty;

        [BsonElement("lootResult")]
        [JsonProperty("lootResult")]
        public List<LootResultMD> lootResult { get; set; } = new();

        [BsonElement("currencyType")]
        [JsonProperty("currencyType")]
        public string currencyType { get; set; } = string.Empty;

        [BsonElement("totalCurrencyPaid")]
        [JsonProperty("totalCurrencyPaid")]
        public int totalCurrencyPaid { get; set; } = -1;
    }

    public class LootResultMD
    {
        [BsonElement("itemType")]
        [JsonProperty("itemType")]
        public string itemType { get; set; } = string.Empty;
        [BsonElement("itemGuid")]
        [JsonProperty("itemGuid")]
        public string itemGuid { get; set; } = string.Empty;
        [BsonElement("itemProfile")]
        [JsonProperty("itemProfile")]
        public string itemProfile { get; set; } = string.Empty;
        [BsonElement("quantity")]
        [JsonProperty("quantity")]
        public int quantity { get; set; } = -1;

    }
}
