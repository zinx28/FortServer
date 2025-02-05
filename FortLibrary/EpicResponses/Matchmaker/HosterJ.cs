using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary.EpicResponses.Matchmaker
{
    public class HosterJ
    {
        public string Region { get; set; } = string.Empty;
        public string Playlist { get; set; } = string.Empty;
        public WebSocket? webSocket { get; set; }
    }
}
