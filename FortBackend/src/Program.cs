using FortBackend.src.App;
using FortBackend.src.App.Utilities;
using FortLibrary;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler.ProcessExit;

            await Service.Intiliazation(args);
        }
        catch (Exception ex) { Logger.Error(ex.Message, "Program"); }
    }
}