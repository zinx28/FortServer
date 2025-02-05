using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortHoster 
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
                        Console.WriteLine("Failed to start the process " + FileName);
                    }
                    else
                    {
                        process.Freeze();

                        if (!IsLauncher)
                        {
                            Console.WriteLine("Started FNANTICHEATPROCESS");
                            _FNAntiCheatProcess = process;
                        }
                        else
                        {
                            Console.WriteLine("Started FNLAUNCHERPROCESS");
                            _FNLauncherProcess = process;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " ~FakeAC");
            }
        }
    }
}
