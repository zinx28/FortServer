using FortLibrary.Shop;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

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

        public Languages title { get; set; } = new Languages();
        public Languages shortDescription { get; set; } = new Languages();
        public Languages description { get; set; } = new Languages();

        public string GetLanguage(Languages value, string acceptLanguage)
        {
            switch (acceptLanguage)
            {
                case "en":
                    return value.en;
                case "es":
                    return value.es;
                case "es-419":
                    return value.es_419;
                case "fr":
                    return value.fr;
                case "it":
                    return value.it;
                case "ja":
                    return value.ja;
                case "ko":
                    return value.ko;
                case "pl":
                    return value.pl;
                case "pt-BR":
                    return value.pt_BR;
                case "ru":
                    return value.ru;
                case "tr":
                    return value.tr;
                case "de":
                    return value.de;

                default:
                    return value.en;
            }
        }
    }

    public class Languages
    {
        public string en { get; set; } = string.Empty;
        public string es { get; set; } = string.Empty;
        [JsonPropertyName("es-419")]
        [JsonProperty("es-419")]
        public string es_419 { get; set; } = string.Empty;
        public string fr { get; set; } = string.Empty;
        public string it { get; set; } = string.Empty;
        public string ja { get; set; } = string.Empty;
        public string ko { get; set; } = string.Empty;
        public string pl { get; set; } = string.Empty;
        [JsonPropertyName("pt-BR")]
        [JsonProperty("pt-BR")]
        public string pt_BR { get; set; } = string.Empty;
        public string ru { get; set; } = string.Empty;
        public string tr { get; set; } = string.Empty;
        public string de { get; set; } = string.Empty;

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
