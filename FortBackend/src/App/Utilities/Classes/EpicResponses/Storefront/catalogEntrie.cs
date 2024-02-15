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
        public List<dynamic> prices { get; set; } = new List<dynamic>();
        public string matchFilter { get; set; } = string.Empty;
        public int filterWeight { get; set; } = 0;
        public List<string> appStoreId { get; set; } = new List<string>();
        public List<object> requirements { get; set; } = new List<object>();
        public string offerType { get; set; } = "StaticPrice";
        public object giftInfo { get; set; } = new object[0];
        public bool refundable { get; set; } = true;
        public List<object> metaInfo { get; set; } = new List<object>();
        public string displayAssetPath { get; set; } = string.Empty;
        public List<object> itemGrants { get; set; } = new List<object>();
        public int sortPriority { get; set; } = 0;
        public int catalogGroupPriority { get; set; } = 0;

    }
}
