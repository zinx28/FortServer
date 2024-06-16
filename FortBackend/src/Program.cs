using FortBackend.src.App;
using FortLibrary;

try
{
    Service.Intiliazation(args);
}
catch (Exception ex) { Logger.Error(ex.Message, "Program"); }

Console.ReadLine();