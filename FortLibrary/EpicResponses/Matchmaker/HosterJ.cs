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

        public string IP { get; set; } = "127.0.0.1"; // just incase its removed
        public int Port { get; set; } = 7777;
        public WebSocket? webSocket { get; set; }
    }
}
