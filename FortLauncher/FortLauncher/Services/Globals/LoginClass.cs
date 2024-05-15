using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Globals
{
    public class LoginClass
    {
        [JsonProperty("status-code")]
        public int StatusCode = 404;

        public string message = "Error!";
        public string token = string.Empty;
    }
}
