using FortLibrary.EpicResponses.Profile.Query.Items;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class Battlepass
    {
        public List<ItemInfo> Rewards = new List<ItemInfo>();
        public int Level { get; set; } = 0;
    }

    public class ItemInfo
    {
        public string TemplateId { get; set; } = string.Empty;
        public string connectedTemplate { get; set; } = string.Empty;
        //public int Level { get; set; } = 0;
        public List<NewAddedItemVariants> new_variants { get; set; } = new List<NewAddedItemVariants>();
        public List<AthenaItemVariants> variants { get; set; } = new List<AthenaItemVariants>();
        public int Quantity { get; set; } = 0;
    }

    public class NewAddedItemVariants
    {
        [JsonProperty("channel")]
        public string channel { get; set; } = string.Empty;

        [JsonProperty("added")]
        public List<string> added { get; set; } = new List<string>();
    }
}
