using FortLauncher.Services.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FortLauncher.Services.Utils.Launch.Helpers
{
    public class Inject
    {
        [DllImport("kernel32.dll")]
        public static extern nint CreateRemoteThread(
            nint hProcess,
            nint lpThreadAttributes,
            uint dwStackSize,
            nint lpStartAddress,
            nint lpParameter,
            uint dwCreationFlags,
            nint lpThreadId
        );

        [DllImport("kernel32.dll")]
        public static extern nint OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint VirtualAllocEx(nint hProcess, nint lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, nint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern nint GetProcAddress(nint hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(nint hObject);
        public static void InjectDll(int pid, string DLlPath)
        {
            try
            {
                if (!File.Exists(DLlPath))
                {
                    MessageBox.Show("Missing dll~ try turning your anti virus off " + DLlPath);
                    return;
                }
                nint process = OpenProcess(0x43A, false, pid);
                nint ProcessAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                if (ProcessAddress == 0)
                {
                    throw new Exception("error");
                }

                byte[] dllByte = Encoding.ASCII.GetBytes(DLlPath + "\0");
                nint dllAddress = VirtualAllocEx(process, 0, dllByte.Length, 0x3000, 0x4);

                bool WriteMemoryResult = WriteProcessMemory(process, dllAddress, dllByte, dllByte.Length, 0);
                if (!WriteMemoryResult)
                {
                    throw new Exception("WriteMemroy");
                }

                nint CreateThreadResult = CreateRemoteThread(process, 0, 0U, ProcessAddress, dllAddress, 0U, 0);
                if (CreateThreadResult == 0)
                {
                    throw new Exception("CreateThread is 0");
                }

                bool CloseResult = CloseHandle(process);
                if (!CloseResult)
                {
                    throw new Exception("CANT CLOSE");
                }

                Loggers.Log("Injected Redirect");
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
