using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary
{
    public static class Logger
    {
        private static StreamWriter? writer;

        static Logger()
        {
            try
            {
                InitializeLogger();
                if (writer == null)
                    throw new InvalidOperationException("Logger StreamWriter is not initialized.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Logger failed to initialize: {ex.Message}");
            }
        }


        private static void InitializeLogger()
        {
            try
            {
                writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FortBackend.log"), false) { AutoFlush = true };
                writer.WriteLine("FortBackend Logs");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001B[31mFailed to open log file: {ex.Message}\u001B[0m");
            }
        }

        public static void PlainLog(string Message)
        {
            if(writer != null)
                writer.WriteLine(Message);

            Console.WriteLine(Message);
        }

        public static void Log(string Message, string Custom = "Log")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {Custom}] " + Message);

            Console.WriteLine($"\u001B[32m[{Custom}]: {Message}\u001B[0m");
        }

        public static void Warn(string Message, string Custom = "Warn")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {Custom}:Warn] " + Message);

            Console.WriteLine($"\u001B[33m[{Custom}]: {Message}\u001B[0m");
        }

        public static void Error(string Message, string Custom = "Error")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {Custom}] " + Message);

            Console.WriteLine($"\u001B[31m[{Custom}]: {Message}\u001B[0m");
        }

        public static void Close()
        {
            if (writer == null) return;

            // WRITER WONT WORK AFTER CLOSING!¬
            writer.WriteLine("=================================================================");
            writer.WriteLine($"END OF LOG: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            writer.Close(); 
        }
    }
}
