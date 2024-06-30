using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
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
using WpfApp6.Services;
using WpfApp6.Services.Launch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Threading;

namespace WpfApp6.Pages
{
    public partial class Home : System.Windows.Controls.UserControl
    {
        private DispatcherTimer clockTimer;
        public ObservableCollection<VersionItem> VersionItems { get; set; }
        private int versionNumber = 1;

        public Home()
        {
            InitializeComponent();

            // Initialize the digital clock timer
            InitializeClockTimer();

            // Initialize the VersionItems collection
            VersionItems = new ObservableCollection<VersionItem>();
            versionList.ItemsSource = VersionItems;

            // Load the selected folder paths from the configuration file
            LoadConfigFile();
        }

        private void InitializeClockTimer()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();

            // Initial update of the digital clock
            UpdateDigitalClock();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateDigitalClock();
        }

        private void UpdateDigitalClock()
        {
            // Update the TextBlock with the current time
            digitalClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void LoadConfigFile()
        {
            try
            {
                // Specify the path for the configuration file
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // Check if the configuration file exists
                if (File.Exists(configFilePath))
                {
                    // Read the content of the configuration file
                    string jsonText = File.ReadAllText(configFilePath);

                    // Parse the JSON content into a JObject
                    JObject json = JObject.Parse(jsonText);

                    // Get the array of folder paths from the JSON
                    JArray folderPathsArray = (JArray)json["FolderPaths"];

                    // Clear existing items
                    VersionItems.Clear();
                    versionNumber = 1;

                    // Loop through each folder path
                    foreach (var folderPathToken in folderPathsArray)
                    {
                        string folderPath = folderPathToken.ToString();

                        // Display builds for each folder path
                        DisplayBuilds(folderPath, versionNumber);
                        versionNumber++;
                    }
                }
                else
                {
                    // Configuration file does not exist
                    System.Windows.MessageBox.Show("Configuration file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                System.Windows.MessageBox.Show($"Error loading configuration file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBuilds(string selectedFolderPath, int versionNumber)
        {
            try
            {
                // Get all files in the selected folder path
                string[] buildFiles = Directory.GetFiles(selectedFolderPath, "Splash.bmp", SearchOption.AllDirectories);

                foreach (string buildFile in buildFiles)
                {
                    // Load the splash img from the game folder
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(buildFile, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    // Create a new VersionItem and set the properties
                    VersionItem versionItem = new VersionItem
                    {
                        FullImagePath = buildFile,
                        ImageSource = bitmap,
                        CornerRadius = new CornerRadius(10),
                        VersionNumber = versionNumber
                    };

                    // Add the version item to the collection
                    VersionItems.Add(versionItem);
                }

                versionList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                System.Windows.MessageBox.Show($"Error loading builds: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                versionList.Visibility = Visibility.Collapsed;
            }
        }

        private async void LaunchButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button launchButton && launchButton.Tag is VersionItem versionItem)
                {
                    string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                    if (File.Exists(configFilePath))
                    {
                        string jsonText = await File.ReadAllTextAsync(configFilePath);
                        JObject json = JObject.Parse(jsonText);
                        JArray folderPathsArray = (JArray)json["FolderPaths"];

                        // Retrieve the index of the version item
                        int index = VersionItems.IndexOf(versionItem);

                        if (index >= 0 && index < folderPathsArray.Count)
                        {
                            string selectedFolderPath = folderPathsArray[index].ToString();
                            string executablePath = Path.Combine(selectedFolderPath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe");

                            if (File.Exists(executablePath))
                            {
                                string credentialsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

                                if (File.Exists(credentialsFilePath))
                                {
                                    string credentialsText = await File.ReadAllTextAsync(credentialsFilePath);
                                    JObject credentialsJson = JObject.Parse(credentialsText);
                                    string email = credentialsJson["Email"].ToString();
                                    string password = credentialsJson["Password"].ToString();

                                    if (email == "NONE" || password == "NONE")
                                    {
                                        System.Windows.MessageBox.Show("Error code 1", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }

                                    // Reading the local DLL path from dlls.json
                                    string dllsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dlls.json");
                                    if (File.Exists(dllsFilePath))
                                    {
                                        string dllsText = await File.ReadAllTextAsync(dllsFilePath);
                                        JObject dllsJson = JObject.Parse(dllsText);
                                        string localDllPath = dllsJson["GFSDK_Aftermath_Lib.x64.dll"].ToString();

                                        if (File.Exists(localDllPath))
                                        {
                                            string destinationDllPath = Path.Combine(selectedFolderPath, "Engine\\Binaries\\ThirdParty\\NVIDIA\\NVaftermath\\Win64", "GFSDK_Aftermath_Lib.x64.dll");
                                            File.Copy(localDllPath, destinationDllPath, true);

                                            PSBasics.Start(selectedFolderPath, "-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", email, password);
                                            FakeAC.Start(selectedFolderPath, "FortniteClient-Win64-Shipping_BE.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "r");
                                            FakeAC.Start(selectedFolderPath, "FortniteLauncher.exe", $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -skippatchcheck", "dsf");

                                            PSBasics._FortniteProcess.WaitForExit();
                                            try
                                            {
                                                FakeAC._FNLauncherProcess?.Close();
                                                FakeAC._FNAntiCheatProcess?.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                System.Windows.MessageBox.Show("There was an error closing: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                        }
                                        else
                                        {
                                            System.Windows.MessageBox.Show("Local DLL file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                    else
                                    {
                                        System.Windows.MessageBox.Show("DLLs configuration file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show("Credentials file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Fortnite executable not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Invalid version item index.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Configuration file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error launching Fortnite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void FindBuild(object sender, RoutedEventArgs e)
        {
            try
            {
                // Specify the path for the configuration file
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // Check if the configuration file exists
                if (File.Exists(configFilePath))
                {
                    // Read the existing content of the configuration file
                    string jsonText = File.ReadAllText(configFilePath);

                    // Parse the JSON content into a JObject
                    JObject json = JObject.Parse(jsonText);

                    // Get the array of folder paths from the JSON
                    JArray folderPathsArray = (JArray)json["FolderPaths"];

                    // Check if the maximum number of versions has been reached
                    if (folderPathsArray.Count >= 4)
                    {
                        System.Windows.MessageBox.Show("You cannot add more than 4 versions please delete one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var dialog = new FolderBrowserDialog();

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string selectedFolderPath = dialog.SelectedPath;

                    // Save the selected folder path
                    SaveSelectedFolderPath(selectedFolderPath);

                    // Reload the configuration file to refresh the displayed versions
                    LoadConfigFile();
                }
                // No need to collapse the versionList when the user cancels
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                System.Windows.MessageBox.Show($"Error finding build: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveSelectedFolderPath(string selectedFolderPath)
        {
            try
            {
                // Specify the path for the configuration file
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                // Create a new JObject to store the folder paths
                JObject json = new JObject();

                // Check if the configuration file exists
                if (File.Exists(configFilePath))
                {
                    // Read the existing content of the configuration file
                    string jsonText = File.ReadAllText(configFilePath);

                    // Parse the JSON content into a JObject
                    json = JObject.Parse(jsonText);
                }

                // Get the array of folder paths from the JSON or create a new one if it doesn't exist
                JArray folderPathsArray = json.ContainsKey("FolderPaths") ? (JArray)json["FolderPaths"] : new JArray();

                // Add the new folder path to the array
                folderPathsArray.Add(selectedFolderPath);

                // Update the JSON with the new folder paths array
                json["FolderPaths"] = folderPathsArray;

                // Serialize JSON to a formatted string
                string output = json.ToString();

                // Write JSON to the configuration file
                File.WriteAllText(configFilePath, output);

                Console.WriteLine($"Selected Folder Path saved to config file: {configFilePath}");
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                System.Windows.MessageBox.Show($"Error saving selected folder path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteVersion(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is VersionItem version)
            {
                VersionItems.Remove(version);
                SaveConfigFile();
            }
        }
        private void SaveConfigFile()
        {
            try
            {
                string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                JObject json;
                JArray folderPathsArray;

                if (File.Exists(configFilePath))
                {
                    // Read existing JSON data from file
                    string existingJson = File.ReadAllText(configFilePath);
                    json = JObject.Parse(existingJson);

                    // Check if the "FolderPaths" array already exists
                    if (json["FolderPaths"] == null)
                    {
                        // If it doesn't exist, create a new one
                        folderPathsArray = new JArray();
                        json["FolderPaths"] = folderPathsArray;
                    }
                    else
                    {
                        // If it exists, get the existing array
                        folderPathsArray = (JArray)json["FolderPaths"];
                    }
                }
                else
                {
                    // If config file doesn't exist, create new JSON object
                    json = new JObject();
                    folderPathsArray = new JArray();
                    json["FolderPaths"] = folderPathsArray;
                }

                // Example: Remove the first version from the array
                if (folderPathsArray.Count > 0)
                {
                    folderPathsArray.RemoveAt(0); // Change this index as needed
                }

                // Convert JSON object to string and write to file
                string output = json.ToString();
                File.WriteAllText(configFilePath, output);
                Console.WriteLine($"Configuration file updated: {configFilePath}");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving configuration file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //class
        public class VersionItem
        {
            public string FullImagePath { get; set; }
            public BitmapImage ImageSource { get; set; }  // Add ImageSource property
            public CornerRadius CornerRadius { get; set; } = new CornerRadius(10);
            public int VersionNumber { get; set; } // Add VersionNumber property
        }
    }
}
