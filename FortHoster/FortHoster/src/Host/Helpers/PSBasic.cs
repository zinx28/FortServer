using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortHoster
{
    public static class PSBasics
    {
        public static Process _FortniteProcess;
        public static void Start(string PATH, string args, string Email, string Password)
        {
            if (Email == null || Password == null)
            {
                Console.WriteLine("Email or password is empty");
                return;
            }
            if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64", "FortniteClient-Win64-Shipping.exe")))
            {
                _FortniteProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = args + $" -AUTH_LOGIN={Email} -AUTH_PASSWORD={Password} -AUTH_TYPE=epic -nullrhi -nosplash -nosound -log", // pretty log fixes old seasons
                        FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64", "FortniteClient-Win64-Shipping.exe"),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    },
                    EnableRaisingEvents = true,
                };

                Console.WriteLine("STARTED PSBASIC");

                _FortniteProcess.Exited += new EventHandler(OnFortniteExit);
                _FortniteProcess.Start();
            }
            else
            {
                Console.WriteLine("Build is missing.. please check if the build still exists");
            }

        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            Process fortniteProcess = _FortniteProcess;
            if (fortniteProcess != null && fortniteProcess.HasExited)
            {
                _FortniteProcess = null;
            }
            Console.WriteLine("Killing helpers");
            FakeAC._FNLauncherProcess?.Kill();
            FakeAC._FNAntiCheatProcess?.Kill();
        }
    }
}
