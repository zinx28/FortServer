using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils.Helpers
{
    public class Loggers
    {
        private static string appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FortLauncher", "launcherlog.txt");

        public static void InitializeLogger()
        {
            InitializeLogFile();
            Log("Initializing");
        }

        public static void Log(string message)
        {
            try
            {

                using (StreamWriter writer = File.AppendText(appDataFolderPath))
                {
                    writer.WriteLine($"FortLauncher->{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static void InitializeLogFile()
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Luna"));
                if (!File.Exists(appDataFolderPath))
                {
                    File.Create(appDataFolderPath).Close();
                }
                else
                {
                    File.WriteAllText(appDataFolderPath, "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Launcher Logs File Failed To Initialize");
            }
        }
    }
}
