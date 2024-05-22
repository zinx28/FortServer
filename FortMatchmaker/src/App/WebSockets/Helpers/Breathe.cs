using FortLibrary;
using FortMatchmaker.src.App.Utilities.Classes;
using FortMatchmaker.src.App.WebSockets.Modules;
using System.Net.WebSockets;
using System.Text;

namespace FortMatchmaker.src.App.WebSockets.Helpers
{
    public class Breathe
    {
        public async static Task Start()
        {
            Logger.Log("Loaded!", "QueueChecker");
            while (true)
            {
                int PeopleInQueue = 0;
                foreach (var Websocket in MatchmakerData.SavedData.Keys)
                {
                    //Console.WriteLine(Websocket);
                    try
                    {
                        if (MatchmakerData.SavedData.TryGetValue(Websocket, out var userData))
                        {

                            // if they are not queingf it wont do nothing
                            if (userData.Queuing)
                            {
                                PeopleInQueue++;
                                if (Websocket.State == WebSocketState.Open)
                                {
                                    await Websocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSavedData.queuedPayloadJson(userData, Websocket))), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                Logger.Log("Logging queue metrics...", "MatchMetrics");
                Logger.Log($"Number of players in queue: {PeopleInQueue}", "MatchMetrics");
                Logger.Log("Number of active matches: N/A", "MatchMetrics");
                Logger.Log("Average match duration: N/A", "MatchMetrics");

                await Task.Delay(18000);
            }
        }
    }
}
