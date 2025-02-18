using FortLibrary.EpicResponses.Matchmaker;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.Utilities.Classes
{
    public class MatchmakerData
    {
        public static ConcurrentDictionary<string, WebSocket> connected = new();
        public static ConcurrentDictionary<WebSocket, UserData> SavedData { get; } = new();



        // gs stuff
        public static List<HosterJ> matchmakerData { get; set; } = new();
    }


    public class UserData
    {
        public bool Queuing { get; set; }
        public string Playlist { get; set; } = string.Empty;
        public string buildId { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public DateTime InsertionTime { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string CustomCode { get; set; } = string.Empty;
    }
}
