using FortBackend.src.App.Utilities;
using FortBackend.src.App.Utilities.Saved;
using FortBackend.src.App.XMPP_Server.TCP;
using FortBackend.src.App.XMPP_Server.XMPP;
using FortLibrary;

namespace FortBackend.src.App.XMPP_V2
{
    public class Xmpp_Server
    {
        // Goal at some point is to just have tcp xmpp only
        public static void Intiliazation(string[] args, CancellationToken cancellationToken)
        {
            Logger.Log("Initializing Xmpp", "Xmpp");


            new Thread(() =>
            {
                XmppServer.Intiliazation(args, cancellationToken);
            }).Start();

            new Thread(() =>
            {
                TcpServer tcpServer = new TcpServer(Saved.DeserializeConfig.TCPXmppPort);
                Task tcpServerTask = tcpServer.Start();
            }).Start();

        }
    }
}
