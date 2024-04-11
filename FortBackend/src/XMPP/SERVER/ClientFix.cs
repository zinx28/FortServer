using FortBackend.src.XMPP.Data;
using FortLibrary.XMPP;
using System.Net.WebSockets;

namespace FortBackend.src.App.SERVER
{
    public class ClientFix
    {
        public static void Init(WebSocket webSocket, DataSaved dataSaved, string clientId)
        {
            if (!dataSaved.clientExists && webSocket.State == WebSocketState.Open)
            {
                if (dataSaved.AccountId != "" && dataSaved.DiscordId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "" && /*dataSaved.JID != "" && */clientId != "" && /*dataSaved.Resource != "" &&*/ dataSaved.DidUserLoginNotSure)
                {
                    dataSaved.clientExists = true;
                    Clients newClient = new Clients
                    {
                        Launcher_Client = webSocket,
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
                    return;
                }
            }
        }
    }
}
