using FortBackend.src.App.SERVER.Send;
using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace FortBackend.src.XMPP.SERVER
{
    public class KillGame
    {
        public static async Task Init(Clients client, DataSaved dataSaved)
        {
            await XmppFriend.UpdatePresenceForFriends(client.Game_Client, "{}", false, true);

            int ClientIndex = GlobalData.Clients.FindIndex(e => e.Game_Client == client.Game_Client);
            if(ClientIndex == -1)
            {
                Console.WriteLine("OK WTFFFF");
                return;
            }
            var ClientData = GlobalData.Clients[ClientIndex];
            Console.WriteLine(ClientData);
            if (ClientIndex != -1)
            {
                GlobalData.Clients[ClientIndex].Game_Client = null; // pooper
                object ParsedPresence = "";
                try
                {
                    ParsedPresence = JsonConvert.DeserializeObject(ClientData.lastPresenceUpdate.presence)!;

               

                    foreach (var woah in dataSaved.Rooms)
                    {
                        if (Array.IndexOf(dataSaved.Rooms, woah) != -1)
                        {
                            var MemberIndex = GlobalData.Rooms[woah].members.FindIndex(i => i.accountId == client.accountId);

                            if (MemberIndex != -1)
                            {
                                GlobalData.Rooms[woah]?.members.RemoveAt(MemberIndex);
                            }
                        }
                    }

                    try
                    {
                        if (ParsedPresence != null)
                        {
                            //var ParsedPresence = JsonConvert.DeserializeObject(client.lastPresenceUpdate.presence);

                            Console.WriteLine($"YOO TEST TEST {ParsedPresence}"); // onmly for testing
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message); // Average Erro! 
                    }

                    // what?
                }
                catch
                {
                    
                   // GlobalData.Clients.Remove(client);
                    //return; // wow
                }


            }
            else
            {
                // return;
            }
        }
    }
}
