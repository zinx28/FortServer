using FortHoster.src.Classes;
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
        public static List<Process> _FortniteProcesses = new();
        public static Process Start(string PATH, string args, string Email, string Password)
        {
            if (Email == null || Password == null)
            {
                Console.WriteLine("Email or password is empty");
                return null!;
            }
            if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64", "FortniteClient-Win64-Shipping.exe")))
            {
                var NewProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        Arguments = args + $" -AUTH_LOGIN={Email} -AUTH_PASSWORD={Password} -AUTH_TYPE=epic " + (Saved.ConfigC.Headless ? "-nullrhi -nosplash -nosound" : ""),
                        FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64", "FortniteClient-Win64-Shipping.exe"),
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    },
                    EnableRaisingEvents = true,
                };

                Console.WriteLine("STARTED PSBASIC");

                NewProcess.Exited += new EventHandler(OnFortniteExit);
                NewProcess.Start();
                return NewProcess;
            }
            else
            {
                Console.WriteLine("Build is missing.. please check if the build still exists");
            }
            return null!;
        }

        public static void OnFortniteExit(object sender, EventArgs e)
        {
            if (sender is Process exitedProcess)
            {
                _FortniteProcesses.Remove(exitedProcess);

                Console.WriteLine("Killing helpers");
                // Also gotta do the same
                FakeAC._FNLauncherProcess?.Kill();
                FakeAC._FNAntiCheatProcess?.Kill();
            }
        }
    }
}
