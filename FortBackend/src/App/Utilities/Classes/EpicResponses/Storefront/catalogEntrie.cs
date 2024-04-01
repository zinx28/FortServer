using FortBackend.src.App.Utilities.Shop.Helpers.Data;
using System.Net;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Storefront
{
    public class catalogEntrie
    {
        public string devName { get; set; } = string.Empty;
        public string offerId { get; set; } = string.Empty;
        public List<string> fulfillmentIds { get; set; } = new List<string>();
        public int dailyLimit { get; set; } = -1;
        public int weeklyLimit { get; set; } = -1;
        public int monthlyLimit { get; set; } = -1;
        public string[] categories { get; set; } = new string[0];
        public List<CatalogPrices> prices { get; set; } = new List<CatalogPrices>();
        public object meta { get; set; } = new object();
        public string matchFilter { get; set; } = string.Empty;
        public int filterWeight { get; set; } = 0;
        public List<string> appStoreId { get; set; } = new List<string>();
        public List<CatalogRequirements> requirements { get; set; } = new List<CatalogRequirements>();
        public string offerType { get; set; } = "StaticPrice";
        public object giftInfo { get; set; } = new object();
        public bool refundable { get; set; } = true;
        public List<MetaInfo> metaInfo { get; set; } = new List<MetaInfo>();
        public string displayAssetPath { get; set; } = string.Empty;
        public List<itemGrants> itemGrants { get; set; } = new List<itemGrants>();
        public int sortPriority { get; set; } = 0;
        public int catalogGroupPriority { get; set; } = 0;

    }

    public class CatalogPrices
    {
        public string currencyType { get; set; } = "BLANK";
        public string currencySubType { get; set; } = "";
        public int regularPrice { get; set; } = 9999999;
        public int finalPrice { get; set; } = 9999999;
        public string saleExpiration { get; set; } = "9999-12-31T23:59:59.999Z";
        public int basePrice = 9999999;
    }

    public class CatalogRequirements
    {
        public string requirementType { get; set; } = "DenyOnItemOwnership";
        public string requiredId { get; set; } = string.Empty;
        public int minQuantity { get; set; } = 1;
    }

    public class itemGrants
    {
        public string templateId { get; set; } = "";
        public int quantity { get; set; } = 1;
    }
}
