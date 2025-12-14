using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.ConfigHelpers
{
    public class FortGameConfig
    {
        [JsonPropertyName("//")]
        [JsonProperty("//")]
        public string FortBackendGame { get; set; } = "";

        public string SeasonEndDate { get; set; } = "9999-12-31T23:59:59.999Z";

        public bool ForceSeason { get; set; } = false; // force season~ means that no matter version you login to will be that season
        public float Season { get; set; } = 0; // eg. 12.41, 11.30, 10.40.... get it?!?
        public int WeeklyQuest { get; set; } = 1; // This is the max weekly quests is added to the user profile if they have the battlepass

        public bool MfaClaim { get; set; } = false; // Change this to true for auto claim! -- idk how your going to claim without having it enabled!

        public bool ShopRotation { get; set; } = false; // THIS IS STILL WIP AND MIGHT GIVE WEIRD SHOPS

        public bool SeasonalShopRotation { get; set; } = true;
    }
}
