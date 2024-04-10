using Newtonsoft.Json;

namespace FortXmpp.src.App.Globals
{
    public class Members
    {
        public string account_id { get; set; } = string.Empty;
        public Dictionary<string, object> meta { get; set; } = new Dictionary<string, object>();
        public List<Dictionary<string, object>> connections { get; set; } = new List<Dictionary<string, object>>();
        public int revision { get; set; } = 0;
        public string updated_at { get; set; } = string.Empty;
        public string joined_at { get; set; } = string.Empty;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string role { get; set; } = string.Empty;
    }

}
