using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.Utilities.Classes
{
    public class MatchmakerData
    {
        public static ConcurrentDictionary<string, WebSocket> connected = new ConcurrentDictionary<string, WebSocket>();
        public static ConcurrentDictionary<WebSocket, UserData> SavedData { get; } = new ConcurrentDictionary<WebSocket, UserData>();
    }


    public class UserData
    {
        public bool Queuing { get; set; }
        public string Playlist { get; set; }
        public string buildId { get; set; }
        public string Region { get; set; }
        public string Ticket { get; set; }
        public string AccountId { get; set; }
        public DateTime InsertionTime { get; set; }
        public string AccessToken { get; set; }
        public string CustomCode { get; set; }
    }
}
