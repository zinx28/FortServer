using FortBackend.src.App;
using FortLibrary;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            await Service.Intiliazation(args);
        }
        catch (Exception ex) { Logger.Error(ex.Message, "Program"); }
    }
}