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
    public static class PSBasics
    {
        public static Process _FortniteProcess;
        public static UIElement test;
        public static void Start(string PATH, string args, string Email, string Password)
        {
            if (Email == null || Password == null)
            {
                System.Windows.MessageBox.Show("Your Token Was Detected Wrong Try Restarting The Launcher!");
                return;
            }
            if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
            {
                _FortniteProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = args + $"-AUTH_LOGIN={Email} -AUTH_PASSWORD={Password} -AUTH_TYPE=exchangecode ",
                        FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")
                    },
                    EnableRaisingEvents = true
                };

                _FortniteProcess.Exited += new EventHandler(OnFortniteExit);
                _FortniteProcess.Start();
            }
            else
            {
                Loggers.Log("Build is missing.. please check if the build still exists");
                System.Windows.MessageBox.Show("Build is missing.. please check if the build still exists");
            }

        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            Process fortniteProcess = _FortniteProcess;
            if (fortniteProcess != null && fortniteProcess.HasExited)
            {
                _FortniteProcess = null;
            }
            Loggers.Log("Killing helpers");
            FakeAC._FNLauncherProcess?.Kill();
            FakeAC._FNAntiCheatProcess?.Kill();
        }
    }
}
