using FortLauncher.Services.Classes;
using FortLauncher.Services.Utils;
using FortLauncher.Services.Utils.Helpers;
using FortLauncher.Services.Utils.Launch.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Wpf.Ui.Controls;

namespace FortLauncher.Pages.Tabs
{
    /// <summary>
    /// Interaction logic for HomeTB.xaml
    /// </summary>
    public partial class LibraryTB : Page
    {
        MouseButtonEventHandler buttonEventHandler = (sender, e) => { e.Handled = true; };
        public class BorderInfo
        {
            public Border Border { get; set; }
            public int PlacementId { get; set; }
            public string BuildId { get; set; }
        }

        List<BorderInfo> borderInfoList = new List<BorderInfo>();
        public Home MainFrame { get; set; }
        public LibraryTB(Home MainFrame)
        {
            this.MainFrame = MainFrame;


            InitializeComponent();
        }

        public async Task LoadBuilds()
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            string FilePath = Path.Combine(DataFolder, "builds.json");

            try
            {
                if (!(borderInfoList.Count > 0))
                {
                    while (LaunchBuilds.Children.Count > 1)
                    {
                        LaunchBuilds.Children.RemoveAt(0);
                    }

                    await Task.Run(async () =>
                    {

                        if (File.Exists(FilePath))
                        {
                            string jsonData = await File.ReadAllTextAsync(FilePath);
                            List<BuildConfig> buildConfig = JsonConvert.DeserializeObject<List<BuildConfig>>(jsonData)!;

                            if (buildConfig != null)
                            {
                                foreach (BuildConfig config in buildConfig)
                                {
                                    var VersionBuild = "";
                                    string prefix = "++Fortnite+Release-";
                                    if (config.VersionID.Length == 0)
                                    {
                                        VersionBuild = "ERROR";
                                    }
                                    else
                                    {
                                        int startIndex = config.VersionID.IndexOf(prefix);
                                        if (startIndex != -1)
                                        {
                                            startIndex += prefix.Length;
                                            VersionBuild = config.VersionID.Substring(startIndex);
                                        }
                                    }
                                    var BuildString = FortniteDetect.Init(config.VersionID);
                                    //VersionSorter.BuildInfo buildInfo = VersionSorter.SortOutMyVersion(config.buildID);
                                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
                                    {
                                        Border border = AddBorderBuild(BuildString, VersionBuild, config);

                                       // border.MouseUp += (sender, e) => ButtonClicked(sender, e, config);

                                        borderInfoList.Add(new BorderInfo
                                        {
                                            Border = border,
                                            PlacementId = 0,
                                            BuildId = config.buildID
                                        });
                                        LaunchBuilds.Children.Insert(0, border);
                                    });

                                    Loggers.Log("Added " + config.VersionID);
                                }

                            }
                        }

                    });
                }
            }
            catch { }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBuilds(); // Yes!
        }

        public void UpdateAllBuilds(BuildConfig config, bool Launched = false)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 10,
                KernelType = KernelType.Gaussian
            };
            foreach (BorderInfo a in borderInfoList)
            {
                //System.Windows.MessageBox.Show(Launched.ToString());
                if (Launched)
                {
                    var mainGrid = a.Border.Child as Grid;
                    if (mainGrid != null)
                    {
                        foreach (var child in mainGrid.Children)
                        {
                            if (child is System.Windows.Controls.Button button)
                            {
                                button.IsEnabled = false; 
                            }
                        }
                    }

                    a.Border.PreviewMouseDown += buttonEventHandler;
                    a.Border.PreviewMouseUp += buttonEventHandler;
                }
                else
                {
                    var mainGrid = a.Border.Child as Grid;
                    if (mainGrid != null)
                    {
                        foreach (var child in mainGrid.Children)
                        {
                            if (child is System.Windows.Controls.Button button)
                            {
                                button.IsEnabled = true;
                            }
                        }
                    }
                    a.Border.PreviewMouseDown -= buttonEventHandler;
                    a.Border.PreviewMouseUp -= buttonEventHandler;
                }

                if (a.BuildId == config.buildID)
                {
                    //((Wpf.Ui.Controls.TextBlock)((Grid)a.Border.Child).Children[0]).Text = Launched ? "Launched" : "Launch";
                }

            }
        }


        public async void ButtonClicked(/*Wpf.Ui.Controls.Button button, */object s, RoutedEventArgs e, BuildConfig config)
        {
           // System.Windows.MessageBox.Show("PENIS");
           // System.Windows.MessageBox.Show(config.buildPath);
            //if (e.ChangedButton == MouseButton.Left)
           // {
                try
                {
                    if (PSBasics._FortniteProcess == null)
                    {
                      //  System.Windows.MessageBox.Show(config.buildPath);
                        if (File.Exists(System.IO.Path.Join(config.buildPath, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                        {
                            UpdateAllBuilds(config, true);
                            await MainFrame.LaunchFortnite(config, e);
                            UpdateAllBuilds(config, false);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Path is wrong?");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Loggers.Log(ex.Message + " ~L2091~");
                    System.Windows.MessageBox.Show("Please Check Logs");
                }
          //  }
        }

        public Border AddBorderBuild(string BuildString, string VersionBuild, BuildConfig config)
        {
            //MessageBox.Show(BuildString);

            Wpf.Ui.Controls.SymbolIcon symbolIcon = new SymbolIcon
            {
                Foreground = Brushes.White,
                Symbol = SymbolRegular.Play16,
                Width = 29,
                Height = 29,
                FontSize = 25
            };

            Border borderte = new Border
            {
                CornerRadius = new CornerRadius(15),
                Width = 186,
                Background = new SolidColorBrush(Color.FromArgb(255, 27, 26, 26)),
                Margin = new Thickness(14, 4, 15, 53)

            };

            string SplashPath = Path.Combine(config.buildPath, "FortniteGame", "Content", "Splash", "Splash.bmp");

            if (File.Exists(SplashPath))
            {
                borderte.Background = new ImageBrush
                {

                    Stretch = Stretch.UniformToFill,
                    ImageSource = new BitmapImage(new Uri(SplashPath))
                };
            }
            else
            {
                borderte.Background = new ImageBrush
                {
                    Stretch = Stretch.UniformToFill,
                    ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Resources/Season1.jpg")),

                };
            }

            Wpf.Ui.Controls.Button LaunchButton = new Wpf.Ui.Controls.Button
            {
                Margin = new Thickness(152, 242, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Height = 43,
                Width = 53,
                Foreground = Brushes.White,
                Content = symbolIcon
            };

            LaunchButton.Click += (sender, e) => ButtonClicked(sender, e, config);

            if (PSBasics._FortniteProcess != null && PSBasics._FortniteProcess.Id != 0)
            {
            //   ((Border)((Grid)border.Child).Children[4]).Child.Effect = blurEffect;
            //   ((Border)((Border)((Grid)border.Child).Children[4]).Child).Child = new Border
            //    {
            //    Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 50))
            // };
            // border.PreviewMouseDown += buttonEventHandler;
            //  border.PreviewMouseUp += buttonEventHandler;
            }

            Border mainBorder = new Border
            {
                CornerRadius = new CornerRadius(15),
                Width = 215,
                Height = 290,
                Child = new Grid
                {
                    Children =
                    {
                        borderte,
                        LaunchButton,
                        new Wpf.Ui.Controls.TextBlock
                        {
                            Text = BuildString,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(14, 242, 52, 5),
                            Foreground = Brushes.White,
                            FontSize = 16
                        }
                    }
                }
            };





            return mainBorder;

        }

        public async Task AddBuild(string fortnitepath)
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            string FilePath = Path.Combine(DataFolder, "builds.json");

            try
            {
                BlurEffect blurEffect = new BlurEffect
                {
                    Radius = 10,
                    KernelType = KernelType.Gaussian
                };
                if (File.Exists(FilePath))
                {
                    string jsonData = await File.ReadAllTextAsync(FilePath);
                    List<BuildConfig> buildConfig = JsonConvert.DeserializeObject<List<BuildConfig>>(jsonData)!;
                    if (buildConfig.Count > 0)
                    {
                        BuildConfig config = buildConfig.FirstOrDefault(e => e.buildPath == fortnitepath)!;
                        if (config != null)
                        {
                            var VersionBuild = "";
                            string prefix = "++Fortnite+Release-";
                            if (config.VersionID.Length == 0)
                            {
                                VersionBuild = "ERROR";
                            }
                            else
                            {
                                int startIndex = config.VersionID.IndexOf(prefix);
                                if (startIndex != -1)
                                {
                                    startIndex += prefix.Length;
                                    VersionBuild = config.VersionID.Substring(startIndex);
                                }
                            }

                            var BuildString = FortniteDetect.Init(config.VersionID);

                            Border border = AddBorderBuild(BuildString, VersionBuild, config);
                          
                            //border. += (sender, e) => ButtonClicked(sender, e, config);

                            // if (PSBasics._FortniteProcess != null && PSBasics._FortniteProcess.Id != 0)
                            // {
                            //   ((Border)((Grid)border.Child).Children[4]).Child.Effect = blurEffect;
                            //   ((Border)((Border)((Grid)border.Child).Children[4]).Child).Child = new Border
                            //    {
                            //    Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 50))
                            // };
                            // border.PreviewMouseDown += buttonEventHandler;
                            //  border.PreviewMouseUp += buttonEventHandler;
                            // }

                            borderInfoList.Add(new BorderInfo
                            {
                                Border = border,
                                PlacementId = 0,
                                BuildId = config.buildID
                            });

                            LaunchBuilds.Children.Insert(0, border);
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show($"Couldn't Find {FilePath}");
                }
            }
            catch (Exception ex)
            {
                // Loggers.Log(ex.Message + "AddBuild");
                System.Windows.MessageBox.Show("Please Check Logs!");
            }
        } 


        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainFrame.CustomContetnDialog.Visibility = Visibility.Visible;
        }
    }
}
