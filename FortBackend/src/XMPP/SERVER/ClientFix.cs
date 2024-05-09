using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;

namespace FortBackend.src.App.SERVER
{
    public class ClientFix
    {
        public static void Init(WebSocket webSocket, DataSaved dataSaved, string clientId)
        {
            if (!dataSaved.clientExists && webSocket.State == WebSocketState.Open && dataSaved.DidUserLoginNotSure)
            {
                //Console.WriteLine("I WANT TO ADD!!!");
                //Console.WriteLine(dataSaved.AccountId);
                //Console.WriteLine(dataSaved.DisplayName);
                //Console.WriteLine(dataSaved.Token);
                //Console.WriteLine(dataSaved.JID);
                //Console.WriteLine(clientId);
                //Console.WriteLine(dataSaved.Resource);
                if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "" && /*dataSaved.JID != "" &&*/ clientId != "" /*&& dataSaved.Resource != ""*/)
                {
                    dataSaved.clientExists = true;
                    Clients newClient = new Clients
                    {
                       // Launcher_Client = webSocket,
                        Game_Client = webSocket,
                        DataSaved = dataSaved,
                        DiscordId = dataSaved.DiscordId,
                        accountId = dataSaved.AccountId,
                        displayName = dataSaved.DisplayName,
                        token = dataSaved.Token,
                        jid = dataSaved.JID,
                        resource = dataSaved.Resource,
                        lastPresenceUpdate = new lastPresenceUpdate
                        {
                            away = false,
                            presence = "{}"
                        }
                    };
                    GlobalData.Clients.Add(newClient);
                   // Console.WriteLine("ADDED!");
                    return;
                }
            }
        }
    }
}
