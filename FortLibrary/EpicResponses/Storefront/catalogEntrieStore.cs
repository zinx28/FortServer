using FortLibrary.Shop;
using Newtonsoft.Json;
using System.Net;

namespace FortLibrary.EpicResponses.Storefront
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class catalogEntrieStore
    {
        public string devName { get; set; } = string.Empty;
        public string offerId { get; set; } = string.Empty;
        public List<CatalogPricesStore> prices { get; set; } = new List<CatalogPricesStore>();
        public string[] categories { get; set; } = new string[0];

        public int dailyLimit { get; set; } = -1;
        public int weeklyLimit { get; set; } = -1;
        public int monthlyLimit { get; set; } = -1;
        public bool refundable { get; set; } = false;
        public List<string> appStoreId { get; set; } = new List<string>();
        public List<CatalogRequirements> requirements { get; set; } = new List<CatalogRequirements>();
        public string offerType { get; set; } = "StaticPrice";
        public object giftInfo { get; set; } = new object();
        public List<MetaInfo> metaInfo { get; set; } = new List<MetaInfo>();
        public string displayAssetPath { get; set; } = string.Empty;
        public List<itemGrants> itemGrants { get; set; } = new List<itemGrants>();
        public int sortPriority { get; set; } = 0;
        public int catalogGroupPriority { get; set; } = 0;

        public string title { get; set; } = string.Empty;
        public string shortDescription { get; set; } = string.Empty;    
        public string description { get; set; } = string.Empty;
    }

    /*saleType*/
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class CatalogPricesStore
    {
        public string currencyType { get; set; } = "BLANK";
        public string currencySubType { get; set; } = "";
        public int regularPrice { get; set; } = 9999999;
        public int finalPrice { get; set; } = 9999999;

        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string saleType { get; set; } = null;
        public string saleExpiration { get; set; } = "9999-12-31T23:59:59.999Z";
        public int basePrice { get; set; } = 9999999;
    }

}
