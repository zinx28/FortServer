using FortBackend.src.App.Utilities.Helpers.Middleware;
using FortLibrary;
using System.Diagnostics;

namespace FortBackend.src.App.Utilities
{
    public class ProcessExitHandler
    {
        public static bool bIsRestarting;
        public static void ProcessExit(object? sender, EventArgs eventArgs)
        {
            //FortBackend.src.App.XMPP_Server.XMPP.XmppServer.STOP();
            //XmppServer.Join(); ~ i can't trust 

            CacheMiddleware.ShutDown().Wait();

            Logger.Close();

            if (bIsRestarting)
            {
                bIsRestarting = false;
                var exePath = Environment.ProcessPath;
                if (exePath is not null)
                {
                    Process.Start(exePath, Environment.GetCommandLineArgs().Skip(1));
                    return;
                }

                Console.WriteLine("Failed to restart :(");
            }
           
            Console.WriteLine("Press any key to close this window . . .");
            Console.ReadKey();
        }

        // Any weird issues happen after restarting tell us!
        public static void RestartServer(IHostApplicationLifetime lifetime)
        {
            bIsRestarting = true;
            lifetime.StopApplication();
        }
    }
}
