using FortLauncher.Services.Globals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FortLauncher.Pages.Tabs
{
    /// <summary>
    /// Interaction logic for SettingsTB.xaml
    /// </summary>
    public partial class SettingsTB : Page
    {
        public SettingsTB()
        {
            InitializeComponent();
            TestBox.Text = UserData.Token;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DllPath = System.IO.Path.Combine(BaseFolder, "FortLauncher");
            if (Directory.Exists(DllPath))
            {
                Process.Start("explorer.exe", DllPath);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string exePath = System.IO.Path.GetDirectoryName(Environment.ProcessPath!)!;
            if (Directory.Exists(exePath))
            {
                Process.Start("explorer.exe", exePath);
            }
        }
    }
}
