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
        public string FortBackendGame { get; set; } = "";

        public string SeasonEndDate { get; set; } = "9999-12-31T23:59:59.9999999";
    }
}
