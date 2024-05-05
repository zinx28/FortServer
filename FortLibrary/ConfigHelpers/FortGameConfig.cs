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

        public string SeasonEndDate { get; set; } = "9999-12-31T23:59:59.9999999";

        public bool ForceSeason { get; set; } = false; // force season~ means that no matter version you login to will be that season
        public int Season { get; set; } = 0;
    }
}
