using FortBackend.src.App.Utilities;
using FortBackend.src.App.XMPP_Server.XMPP;

namespace FortBackend.src.App.XMPP_V2
{
    public class Xmpp_Server
    {
        // Goal at some point is to just have tcp xmpp only
        public static void Intiliazation(string[] args)
        {
            Logger.Log("Initializing Xmpp", "Xmpp");


            new Thread(() =>
            {
                XmppServer.Intiliazation(args);
            }).Start();
            /*
             * var TCPXmppServer = new Thread(() =>
            { 
                TcpServer testserver = new TcpServer(Saved.DeserializeConfig.TCPXmppPort);
                Task tcpServerTask = testserver.Start();
            });
            TCPXmppServer.Start();
            */
        }
    }
}
