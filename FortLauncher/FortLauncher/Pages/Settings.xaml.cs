using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace WpfApp6.Pages
{
    public partial class Settings : UserControl
    {
        private const string DllsFileName = "dlls.json";
        private const string DllKeyName = "GFSDK_Aftermath_Lib.x64.dll";
        private DispatcherTimer timer;
        public Settings()
        {
            InitializeComponent();
            LoadSettings();
            LoadDllPath();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += Timer_Tick;
            timer.Start();

            this.Visibility = Visibility.Visible;
        }

        private async void LoadDllPath()
        {
            string dllsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DllsFileName);

            if (File.Exists(dllsFilePath))
            {
                try
                {
                    string dllsText = await File.ReadAllTextAsync(dllsFilePath);
                    JObject dllsJson = JObject.Parse(dllsText);

                    if (dllsJson.ContainsKey(DllKeyName))
                    {
                        DllPathTextBox.Text = dllsJson[DllKeyName].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading DLL path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                DllPathTextBox.Text = openFileDialog.FileName;
                await SaveDllPathAsync(openFileDialog.FileName);
            }
        }

        private async Task SaveDllPathAsync(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("Selected file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string dllsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DllsFileName);
            JObject dllsJson;

            if (File.Exists(dllsFilePath))
            {
                string dllsText = await File.ReadAllTextAsync(dllsFilePath);
                dllsJson = JObject.Parse(dllsText);
            }
            else
            {
                dllsJson = new JObject();
            }

            dllsJson[DllKeyName] = dllPath;
            string updatedDllsText = dllsJson.ToString();
            await File.WriteAllTextAsync(dllsFilePath, updatedDllsText);

            MessageBox.Show("DLL path saved successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
      private void Timer_Tick(object sender, EventArgs e)
        {
            clock.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void DiscordLinkCard_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "https://github.com/zinx28/FortServer";
            process.Start();
        }

        private void DiscordLinkCard_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void DiscordLinkCard_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists("credentials.json"))
                {
                    string json = File.ReadAllText("credentials.json");
                    dynamic credentials = JsonConvert.DeserializeObject(json);

                    string email = credentials.Email;
                    string password = credentials.Password;
                    bool rememberMe = credentials.RememberMe;

                    EmailTextBox.Text = email;
                    PasswordBox.Password = password;
                    RememberMeCheckBox.IsChecked = rememberMe;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;
                string password = PasswordBox.Password;
                bool rememberMe = RememberMeCheckBox.IsChecked ?? false;

                dynamic credentials = new
                {
                    Email = email,
                    Password = password,
                    RememberMe = rememberMe
                };

                string json = JsonConvert.SerializeObject(credentials);
                File.WriteAllText("credentials.json", json);

                MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailTextBox.Text == "Email")
            {
                EmailTextBox.Text = "";
                EmailTextBox.Foreground = Brushes.White;
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                EmailTextBox.Text = "Email";
                EmailTextBox.Foreground = Brushes.Gray;
            }
        }
    }
}