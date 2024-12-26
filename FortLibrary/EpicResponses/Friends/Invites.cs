using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Friends
{
    public class Invite
    {
        [JsonPropertyName("urn:epic:invite:platformdata_s")]
        public string? PlatformData { get; set; }

        [JsonPropertyName("urn:epic:member:dn_s")]
        public string? MemberDisplayName { get; set; }

        [JsonPropertyName("urn:epic:conn:platform_s")]
        public string? ConnectionPlatform { get; set; }

        [JsonPropertyName("urn:epic:conn:type_s")]
        public string? ConnectionType { get; set; }

        [JsonPropertyName("urn:epic:cfg:build-id_s")]
        public string? BuildId { get; set; }
    }
}
