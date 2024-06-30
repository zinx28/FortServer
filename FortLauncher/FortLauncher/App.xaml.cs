using System.Windows;
using WpfApp6.Pages;

namespace WpfApp6
{
    public partial class App : Application
    {
        private bool mainAppOpened = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Set to dark so it doesn't break UI
            ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (loginWindow.LoginSuccessful && !mainAppOpened)
            {
                mainAppOpened = true;

                ShutdownMode = ShutdownMode.OnMainWindowClose;

                // Open the Home window here
                Home mainWindow = new Home();
               
            }
            else
            {
                Shutdown();
            }
        }
    }
}
