using Newtonsoft.Json;

namespace FortBackend.src.App.Utilities.Classes.EpicResponses.Lightswitch
{
    public class launcherInfoDTO
    {
        public string appName { get; set; } = "Fortnite";
        public string catalogItemId { get; set; } = "";

        [JsonProperty("namespace")]
        public string Namespace { get; set; } = "fn";
    }
}
