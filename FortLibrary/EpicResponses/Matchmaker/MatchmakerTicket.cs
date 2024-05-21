using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Matchmaker
{
    public class MatchmakerTicket
    {
        public string accountId { get; set; } = string.Empty;
        public string BuildId { get; set; } = string.Empty;
        public string Playlist { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string CustomKey { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;

        public bool Priority { get; set; } = false;
        public string timestamp { get; set; } = string.Empty;
    }
}
