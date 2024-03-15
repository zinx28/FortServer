namespace FortMatchmaker.src.App.Utilities
{
    public class Logger
    {
        public static void Log(string Message, string Custom = "Log")
        {
            Console.WriteLine($"\u001B[32m[{Custom}]: {Message}\u001B[0m");
        }

        public static void Warn(string Message)
        {
            Console.WriteLine($"\u001B[33m[Warn]: {Message}\u001B[0m");
        }

        public static void Error(string Message, string Custom = "Error")
        {
            Console.WriteLine($"\u001B[31m[{Custom}]: {Message}\u001B[0m");
        }
    }
}
