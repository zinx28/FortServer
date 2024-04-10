using Newtonsoft.Json;

namespace FortLibrary.EpicResponses.Lightswitch
{
    public class launcherInfoDTO
    {
        public string appName { get; set; } = "Fortnite";
        public string catalogItemId { get; set; } = "4fe75bbc5a674f4f9b356b5c90567da5";

        [JsonProperty("namespace")]
        public string Namespace { get; set; } = "fn";
    }
}
