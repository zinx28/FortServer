using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLibrary
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> _instance = new(() => new Logger());
        public static Logger Instance => _instance.Value;

        private static StreamWriter? writer;
        private static readonly object _lock = new();
        private static int LogLevel = 1; // 1 for the start only

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
        private Logger() { InitializeLogger(); }

        private static void InitializeLogger()
        {
            try
            {
                lock (_lock)
                {
                    if (writer == null)
                    {
                        writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FortBackend.log"), false) { AutoFlush = true };
                        writer.WriteLine("=== FortBackend Logs ===");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001B[31mFailed to open log file: {ex.Message}\u001B[0m");
            }
        }

        /// <summary>
        /// Set log level
        /// </summary>
        /// <param name="level">log level: 0 basic logs</param>
        public static void SetLogLevel(int level)
        {
            LogLevel = level;
        }

        /// <summary>
        /// Logs plain text
        /// </summary>
        /// <param name="message">Message that will be printed</param>
        public static void PlainLog(string message)
        {
            if(writer != null)
                writer.WriteLine(message);

            if (LogLevel > 0 && LogLevel != 3)
                Console.WriteLine(message);
        }

        /// <summary>
        /// Logs plain text
        /// </summary>
        /// <param name="message">Message that will be printed</param>
        public static void PlainLog(object message)
        {
            if (writer != null)
                writer.WriteLine(message);

            if (LogLevel > 0 && LogLevel != 3)
                Console.WriteLine(message);
        }

        /// <summary>
        /// Logs the default log
        /// </summary>
        /// <param name="message">Message that will be printed</param>
        /// <param name="custom">Custom log type (e.g., 'Log', 'FortServer')</param>
        public static void Log(string message, string custom = "Log")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {custom}] " + message);

            Console.WriteLine($"\u001B[32m[{custom}]: {message}\u001B[0m");
        }

        /// <summary>
        /// Logs the warn log
        /// </summary>
        /// <param name="message">Message that will be printed</param>
        /// <param name="custom">Custom log type (e.g., 'Warn', 'FortServer')</param>
        public static void Warn(string message, string custom = "Warn")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {custom}:Warn] " + message);

            Console.WriteLine($"\u001B[33m[{custom}]: {message}\u001B[0m");
        }

        /// <summary>
        /// Logs the error log
        /// </summary>
        /// <param name="message">Message that will be printed</param>
        /// <param name="custom">Custom log type (e.g., 'Error', 'FortServer')</param>
        public static void Error(string message, string custom = "Error")
        {
            if (writer != null)
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {custom}:Error] " + message);

            Console.WriteLine($"\u001B[31m[{custom}]: {message}\u001B[0m");
        }

        /// <summary>
        /// Closes the log writer
        /// </summary>
        public static void Close()
        {
            if (writer == null) return;

            lock (_lock)
            {
                // WRITER WONT WORK AFTER CLOSING!¬
                writer.WriteLine("=================================================================");
                writer.WriteLine($"END OF LOG: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                writer.Close();
                writer = null;
            }
        }
    }
}
