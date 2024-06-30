using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp6.Services.Launch
{
    // Goofy Code Need To Improve On V2
    public class FakeAC
    {
		public static Process _FNLauncherProcess;
		public static Process _FNAntiCheatProcess;

		public static void Start(string Path69, string FileName, string args = "", string t = "r")
		{
			try {
				if (File.Exists(Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName)))
				{
					ProcessStartInfo ProcessIG = new ProcessStartInfo()
					{
						FileName = Path.Combine(Path69, "FortniteGame\\Binaries\\Win64\\", FileName),
						Arguments = args,
						CreateNoWindow = true,
					};

					if(t == "r")
					{
						_FNAntiCheatProcess = Process.Start(ProcessIG);
						if (_FNAntiCheatProcess.Id == 0)
						{
							MessageBox.Show("FAILED STARTING!?!?!");
						}
						_FNAntiCheatProcess.Freeze();
					}else
					{
						_FNLauncherProcess = Process.Start(ProcessIG);
						if (_FNLauncherProcess.Id == 0)
						{
							MessageBox.Show("FAILED STARTING!?!?!");
						}
						_FNLauncherProcess.Freeze();
					}
					
				}
			}catch (Exception ex)
			{
				MessageBox.Show("THERE BEEN A ERROR");
			}
		}
	}
}
