using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary;

namespace FortBackend.src.App.Utilities
{
    public class ProcessExitHandler
    {
        public static void ProcessExit(object? sender, EventArgs eventArgs)
        {
            //FortBackend.src.App.XMPP_Server.XMPP.XmppServer.STOP();
            //XmppServer.Join(); ~ i can't trust 

            CacheMiddleware.ShutDown().Wait();

            Logger.Close();

            Console.WriteLine("Press any key to close this window . . .");
            Console.ReadKey();
        }
    }
}
