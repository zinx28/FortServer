namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Storefront
{
    public class Catalog
    {
        public int refreshIntervalHrs { get; set; } = 1;
        public int dailyPurchaseHrs { get; set; } = 24;
        public string expiration { get; set; } = string.Empty;
        public List<dynamic> storefronts  { get; set;} = new List<dynamic>();

    }

    public class StoreFront
    {
        public string name { get; set; } = string.Empty;
        public List<dynamic> catalogEntries { get; set; } = new List<dynamic>();
    }
}
