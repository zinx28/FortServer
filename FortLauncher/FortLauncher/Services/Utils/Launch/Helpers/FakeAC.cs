using FortLauncher.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils.Launch.Helpers
{
    public class FakeAC
    {
        public static Process _FNLauncherProcess;
        public static Process _FNAntiCheatProcess;

        public static void Start(string Path69, string FileName, string args = "", bool IsLauncher = false)
        {
            try
            {
                if (File.Exists(Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName)))
                {
                    ProcessStartInfo ProcessIG = new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName),
                        Arguments = args,
                        CreateNoWindow = true,
                    };

                    Process process = Process.Start(ProcessIG);

                    if (process == null || process.Id == 0)
                    {
                        Loggers.Log("Failed to start the process " + FileName);
                        MessageBox.Show("Failed to start the process " + FileName);
                    }
                    else
                    {
                        process.Freeze();

                        if (!IsLauncher)
                        {
                            Loggers.Log("Started FNANTICHEATPROCESS");
                            _FNAntiCheatProcess = process;
                        }
                        else
                        {
                            Loggers.Log("Started FNLAUNCHERPROCESS");
                            _FNLauncherProcess = process;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message + " ~FakeAC");
                MessageBox.Show("Please check launcher logs");
            }
        }
    }
}
