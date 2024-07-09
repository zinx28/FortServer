using FortLauncher.Services.Classes;
using FortLauncher.Services.Utils;
using FortLauncher.Services.Utils.Helpers;
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
                                        Border border = AddBorderBuild(BuildString, VersionBuild, config.buildPath);

                                       // border.MouseUp += (sender, e) => ButtonClicked(border, sender, e, config);

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

        public Border AddBorderBuild(string BuildString, string VersionBuild, string buildPath)
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

            string SplashPath = Path.Combine(buildPath, "FortniteGame", "Content", "Splash", "Splash.bmp");

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
                        new Wpf.Ui.Controls.Button
                        {
                            Margin = new Thickness(152, 242, 0, 0),
                            VerticalAlignment = VerticalAlignment.Top,
                            Height = 43,
                            Width = 53,
                            Foreground = Brushes.White,
                            Content = symbolIcon

                        },
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

                                Border border = AddBorderBuild(BuildString, VersionBuild, config.buildPath);

                              //  border.MouseUp += (sender, e) => ButtonClicked(border, sender, e, config);

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
