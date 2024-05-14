using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.MongoDB
{
    public class CreateAccountArg
    {
        public string DiscordId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string[] UserIps { get; set; } = new string[0];
        public bool banned { get; set; } = false;
    }
}
