using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.Encoders.JWTCLASS
{
    public class TokenPayload
    {
        [JsonProperty("app")]
        public string? App { get; set; }

        [JsonProperty("sub")]
        public string? Sub { get; set; }

        [JsonProperty("dvid")]
        public string? Dvid { get; set; }

        [JsonProperty("mver")]
        public string? Mver { get; set; }

        [JsonProperty("clid")]
        public string? Clid { get; set; }

        [JsonProperty("dn")]
        public string? Dn { get; set; }

        [JsonProperty("am")]
        public string? Am { get; set; }

        [JsonProperty("sec")]
        public string? Sec { get; set; }

        [JsonProperty("p")]
        public string? P { get; set; }

        [JsonProperty("iai")]
        public string? Iai { get; set; }

        [JsonProperty("clsvc")]
        public string? Clsvc { get; set; }

        [JsonProperty("t")]
        public string? T { get; set; }

        [JsonProperty("ic")]
        public string? Ic { get; set; }

        [JsonProperty("exp")]
        public long? Exp { get; set; }

        [JsonProperty("iat")]
        public long? Iat { get; set; }

        [JsonProperty("jti")]
        public string? Jti { get; set; }

        [JsonProperty("nbf")]
        public long? Nbf { get; set; }
    }
}
