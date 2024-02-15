namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Profile
{
    public class PurchaseCatalogEntryRequest
    {

        public string offerId { get; set; } = string.Empty;
        public int purchaseQuantity { get; set; } = 0;
        public string currency { get; set; } = "MtxCurrency";
        public string currencySubType { get; set; } = string.Empty;
        public int expectedTotalPrice { get; set; } = 0;
        public string gameContext { get; set; } = string.Empty;
    }
}
