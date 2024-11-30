using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.Dynamics
{
    public class Languages
    {
        public string en { get; set; } = string.Empty; // sorry :3 i couldn't remove the org stuff
        public string es { get; set; } = string.Empty;
        [JsonPropertyName("es-419")]
        [JsonProperty("es-419")]
        public string es_419 { get; set; } = string.Empty;
        public string fr { get; set; } = string.Empty;
        public string it { get; set; } = string.Empty;
        public string ja { get; set; } = string.Empty;
        public string ko { get; set; } = string.Empty;
        public string pl { get; set; } = string.Empty;
        [JsonPropertyName("pt-BR")]
        [JsonProperty("pt-BR")]
        public string pt_BR { get; set; } = string.Empty;
        public string ru { get; set; } = string.Empty;
        public string tr { get; set; } = string.Empty;
        public string de { get; set; } = string.Empty;

    }
}
