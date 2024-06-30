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
	public static class PSBasics
	{
		public static Process _FortniteProcess;
		public static void Start(string PATH, string args, string Email, string Password)
		{
			if(Email == null || Password == null)
			{
				MessageBox.Show("Sorry Make Sure You Put Your Eon Logins");
				return;
			}
			if (File.Exists(Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
			{
				PSBasics._FortniteProcess = new Process()
				{
					StartInfo = new ProcessStartInfo()
					{
						Arguments = $"-AUTH_LOGIN={Email} -AUTH_PASSWORD={Password} -AUTH_TYPE=epic " + args,
						FileName = Path.Combine(PATH, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")
					},
					EnableRaisingEvents = true
				};
				PSBasics._FortniteProcess.Exited += new EventHandler(PSBasics.OnFortniteExit);
				PSBasics._FortniteProcess.Start();


			}

		}

		public static void OnFortniteExit(object sender, EventArgs e)
		{
			Process fortniteProcess = PSBasics._FortniteProcess;
			if (fortniteProcess != null && fortniteProcess.HasExited)
			{
				PSBasics._FortniteProcess = (Process)null;
			}
			FakeAC._FNLauncherProcess?.Kill();
			FakeAC._FNAntiCheatProcess?.Kill();
		}
	}
}
