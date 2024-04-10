using FortXmpp.src.App.Globals;
using FortXmpp.src.App.Globals.Data;
using System.Net.WebSockets;

namespace FortXmpp.src.App.SERVER
{
    public class ClientFix
    {
        public static void Init(WebSocket webSocket, DataSaved_XMPP dataSaved, string clientId)
        {
            if (!dataSaved.clientExists && webSocket.State == WebSocketState.Open)
            {
                if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "" && dataSaved.JID != "" && clientId != "" && dataSaved.Resource != "" && dataSaved.DidUserLoginNotSure)
                {
                    dataSaved.clientExists = true;
                    Clients newClient = new Clients
                    {
                        Client = webSocket,
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
