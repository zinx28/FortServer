using FortLibrary.Encoders;
using FortLibrary.EpicResponses.Matchmaker;
using FortMatchmaker.src.App.Utilities.Classes;
using System.Net.WebSockets;

namespace FortMatchmaker.src.App.WebSockets.Modules
{
    // Copied From a older matchmaker i had from github
    public class JsonSavedData
    {
        public static int ConnectingDelay = 200;
        public static int WaitingDelay = 1000;
        public static int QueuedDelay = 10000;
        public static int SessionAssignmentDelay = 3000;
        public static int JoinDelay = 2000;
        public static string ticket = Guid.NewGuid().ToString().Replace("-", "");

        public static string sessionAssignmentPayloadJson()
        {

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                payload = new
                {
                    matchId = RandomHash.CalculateMd5Hash("2" + DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                    state = "SessionAssignment"
                },
                name = "StatusUpdate"
            });

        }

        public static string connectingPayloadJson()
        {

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                payload = new
                {
                    state = "Connecting"
                },
                name = "StatusUpdate"
            });
        }
        public static string waitingPayloadJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                payload = new
                {
                    totalPlayers = MatchmakerData.connected.Count,
                    connectedPlayers = MatchmakerData.connected.Count,
                    state = "Waiting"
                },
                name = "StatusUpdate"
            });
        }


        public static string queuedPayloadJson(UserData matchmakerTicket, WebSocket websocket)
        {
            int YeahCount = MatchmakerData.SavedData.Values.Count(userData =>
                userData.Playlist == matchmakerTicket.Playlist &&
                userData.Region == matchmakerTicket.Region &&
                userData.CustomCode == matchmakerTicket.CustomCode &&
                userData.buildId == matchmakerTicket.buildId
            );

            if (string.IsNullOrEmpty(matchmakerTicket.CustomCode) || matchmakerTicket.CustomCode != "NONE")
            {
                if (!Servers.VaildCodes.Contains(matchmakerTicket.CustomCode))
                {
                    websocket.Dispose();
                    return "error";
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                payload = new
                {
                    ticketId = ticket,
                    queuedPlayers = YeahCount,
                    estimatedWaitSec = YeahCount * 2,
                    status = YeahCount == 0 ? 2 : 3,
                    state = "Queued"
                },
                name = "StatusUpdate"
            });
        }
    }
}
