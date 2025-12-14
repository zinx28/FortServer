using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Profile
{
    public class PurchaseMultipleCatalogEntriesReq
    {
        public List<PurchaseInfoList> purchaseInfoList = new();
    }

    public class PurchaseInfoList
    {
        public string currency { get; set; } = "MtxCurrency";
        public string currencySubType { get; set; } = string.Empty;
        public int expectedTotalPrice { get; set; } = -1;
        public string gameContext { get; set; } = string.Empty;
        public string offerId { get; set; } = string.Empty;
        public int purchaseQuantity { get; set; } = -1;
    }
}
