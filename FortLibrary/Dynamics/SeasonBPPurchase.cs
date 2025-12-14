using FortLibrary.EpicResponses.Profile.Query.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class SeasonBPPurchase
    {
        public bool bRequireBp { get; set; } = true;
        public string OfferId { get; set; } = string.Empty;
        public string templateId { get; set; } = string.Empty;
        public List<NewAddedItemVariants_V2> new_variants { get; set; } = new List<NewAddedItemVariants_V2>();
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();

        public int Price { get; set; } = -1;
        public int Quantity { get; set;  } = -1;
    }

    public class NewAddedItemVariants_V2
    {
        [JsonProperty("connectedItem")]
        public string connectedItem { get; set; } = string.Empty;
        [JsonProperty("channel")]
        public string channel { get; set; } = string.Empty;

        [JsonProperty("added")]
        public List<string> added { get; set; } = new List<string>();
    }
}
