using FortLauncher.Services.Utils.Helpers;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        const int SW_RESTORE = 9;
#if DEV
        public static Mutex mutex = new Mutex(true, Guid.NewGuid().ToString().Replace("-", ""));
#else
        public static Mutex mutex = new Mutex(true, "{D9C5B56E-F98A-45D8-9A41-8C8917E2F35D}");
#endif
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Loggers.InitializeLogger();
               // RPC.Init();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Process currentProcess = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                foreach (Process process in processes)
                {
                    if (process.Id != currentProcess.Id)
                    {
                        ShowWindow(process.MainWindowHandle, SW_RESTORE);
                        SetForegroundWindow(process.MainWindowHandle);
                        Application.Current.Shutdown();
                        return;
                    }
                }
                Application.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e) { }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Loggers.Log(e.Exception.Message + " ~Application_DispatcherUnhandledException");
        }


    }

}
