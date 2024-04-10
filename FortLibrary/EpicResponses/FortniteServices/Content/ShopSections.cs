using System.Net;

namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class ShopSections
    {
        public string _title { get; set; } = "shop-sections";
        public ShopSectionsSectionList sectionList { get; set; } = new ShopSectionsSectionList();

        public bool _noIndex { get; set; } = false;
        public string _activeDate { get; set; } = "2023-09-27T21:00:00.000Z";
        public string lastModified { get; set; } = "2023-10-18T13:47:43.007Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameShopSections";
    }

    public class ShopSectionsSectionList
    {
        public string _type { get; set; } = "ShopSectionList";

        public List<ShopSectionsSectionsSEctions> sections { get; set; } = new List<ShopSectionsSectionsSEctions>();
    }

    public class ShopSectionsSectionsSEctions
    {
        public bool bSortOffersByOwnership { get; set; } = false;
        public bool bShowIneligibleOffersIfGiftable { get; set; } = false;
        public bool bEnableToastNotification { get; set; } = true;

        public ShopSectionsBackground background { get; set; } = new ShopSectionsBackground();
        public string _type { get; set; } = "ShopSection";
        public int landingPriority { get; set; } = 1;
        public bool bHidden { get; set; } = false;
        public string sectionId { get; set; } = "Unknown";
        public bool bShowTimer { get; set; } = true;
        public string sectionDisplayName { get; set; } = "Unknown";
        public bool bShowIneligibleOffers { get; set; } = true;
    }

    public class ShopSectionsBackground
    {
        public string stage { get; set; } = "default";
        public string _type { get; set; } = "DynamicBackground";
        public string key { get; set; } = "vault";
    }
}
