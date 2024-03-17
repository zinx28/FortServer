using FortBackend.src.App.TCP_XMPP.Helpers.Resources;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace FortBackend.src.App.TCP_XMPP.Helpers
{
    public class ClientFix
    {
        public static void Init(TcpClient client, DataSaved dataSaved, string clientId)
        {
            if (!dataSaved.clientExists)
            {
                if (dataSaved.AccountId != "" && dataSaved.DisplayName != "" && dataSaved.Token != "" && dataSaved.JID != "" && clientId != "" && dataSaved.Resource != "" && dataSaved.DidUserLoginNotSure)
                {
                    dataSaved.clientExists = true;
                    Clients newClient = new Clients
                    {
                        Client = client.Client,
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
