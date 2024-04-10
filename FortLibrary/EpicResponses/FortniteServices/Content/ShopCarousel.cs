namespace FortLibrary.EpicResponses.FortniteServices.Content
{
    public class ShopCarousel
    {
        public ShopItemsList itemsList { get; set; } = new ShopItemsList();

        public string _title { get; set; } = "shop-carousel";

        public bool _noIndex { get; set; } = false;
        public string _activeDate { get; set; } = "2020-09-25T12:00:00.000Z";
        public string lastModified { get; set; } = "2021-03-23T00:45:30.609Z";
        public string _locale { get; set; } = "en-US";
        public string _templateName { get; set; } = "FortniteGameShopCarousel";
    }

    public class ShopItemsList
    {
        public string _type { get; set; } = "ShopCarouselItemList";
        public string[] items { get; set; } = new string[0];
    }
}
