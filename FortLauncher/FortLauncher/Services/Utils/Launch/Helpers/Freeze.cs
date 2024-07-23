using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Utils.Launch.Helpers
{
    public static class FreezeYea
    {
        [DllImport("kernel32.dll")]
        private static extern nint OpenThread(int dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(nint hThread);

        public static void Freeze(this Process process)
        {

            foreach (object obj in process.Threads)
            {
                ProcessThread thread = (ProcessThread)obj;
                var Thread = OpenThread(2, false, (uint)thread.Id);
                if (Thread != 0)
                {
                    SuspendThread(Thread);
                }
            }
        }
    }
}
