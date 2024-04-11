using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.XMPP
{
    public class DataSaved
    {
        public bool DidUserLoginNotSure = false;
        public string DiscordId = string.Empty;
        public string AccountId = string.Empty;
        public string DisplayName = string.Empty;

        public bool clientExists = false;
        public string Token = string.Empty;
        public string JID = string.Empty;
        public string Resource = string.Empty;
        public string[] Rooms = new string[] { };
    }
}
