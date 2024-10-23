using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FortLauncher.Pages.Tabs;
using FortLauncher.Services.Classes;
using FortLauncher.Services.Globals;
using FortLauncher.Services.Utils;
using FortLauncher.Services.Utils.Helpers;
using FortLauncher.Services.Utils.Launch.Helpers;
using FortLauncher.Services.Utils.Launch;
using Wpf.Ui.Controls;

namespace FortLauncher.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public HomeTB HomeTB { get; set; } = new HomeTB();
        public BlankPage BlankTB { get; set; } = new BlankPage();
        public LibraryTB LibraryTB { get; set; }

        private CancellationTokenSource cancellationTokenSource;
        public Home()
        {
            InitializeComponent();
            TestBox.Text = UserData.Token;
            UserNameBox.Text = UserData.UserName;

            LibraryTB = new LibraryTB(this);
        }

        public object LastPageContent;

        public void Navigate(object content)
        {

            if (content != MainFrame.Content)
            {
                LastPageContent = MainFrame.Content;

                MainFrame.Navigate(content);

                if (content.ToString() != null)
                {
                    string MainContent = content?.ToString()?.ToLower()!;
                    //System.Windows.MessageBox.Show(content.ToString());
                    if (MainContent.Contains("home"))
                    {
                        SideBarHome.Background = new SolidColorBrush(Color.FromRgb(25, 23, 23));
                        SideBarLibrary.Background = Brushes.Transparent;
                    }
                    else if (MainContent.Contains("library"))
                    {
                        SideBarHome.Background = Brushes.Transparent;
                        SideBarLibrary.Background = new SolidColorBrush(Color.FromRgb(25, 23, 23));

                    }
                }


                DoubleAnimation slideAnimation = new DoubleAnimation();
                slideAnimation.From = +300;
                slideAnimation.To = 0;
                slideAnimation.Duration = TimeSpan.FromSeconds(0.11);

                TranslateTransform translation = new TranslateTransform();
                MainFrame.RenderTransform = translation;
                translation.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
            }
        }

        private void MainFrame_Initialized(object sender, EventArgs e)
        {
            Navigate(HomeTB);
        }

        private void SideBarHome_Click(object sender, RoutedEventArgs e)
        {
            Navigate(HomeTB);
        }

        private void SideBarLibrary_Click(object sender, RoutedEventArgs e)
        {
            Navigate(LibraryTB);
            //  MainFrame.Navigate(LibraryTB);
        }

        public async Task LaunchFortnite(BuildConfig config, EventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                Launch launch = new Launch(this);

                await launch.Start(config, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Loggers.Log("LaunchFortnite was cancelled.");

                PSBasics._FortniteProcess.Kill();
                //if (FakeAC._FNLauncherProcess != null && FakeAC._FNLauncherProcess.Id != 0)
                //{
                //    FakeAC._FNLauncherProcess.Close();
                //}
                //if (FakeAC._FNAntiCheatProcess != null && FakeAC._FNAntiCheatProcess.Id != 0)
                //{
                //    FakeAC._FNAntiCheatProcess.Close();
                //}
                //if(PSBasics._FortniteProcess != null)
                //{
                //    PSBasics._FortniteProcess.Kill();
                //}

            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message + "~LaunchFortnite~");
                System.Windows.MessageBox.Show("Please check launcher logs");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }


        private async void CustomContetnDialog_ButtonClicked(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (args.Button == ContentDialogButton.Close)
                {
                    CustomContetnDialog.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (File.Exists(System.IO.Path.Join(FortnitePath.Text, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                    {
                        this.IsEnabled = false;
                        bool AddNewBuild = await AddBuilds.AddBuild(FortnitePath.Text, System.IO.Path.Join(FortnitePath.Text, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"));
                        this.IsEnabled = true;
                        if (AddNewBuild)
                        {

                            CustomContetnDialog.Visibility = Visibility.Collapsed;
                            Navigate(BlankTB);
                            LibraryTB.AddBuild(FortnitePath.Text);
                            await Task.Delay(100);
                            Navigate(LibraryTB);
                        }
                        else
                        {
                            ErrorText.Visibility = Visibility.Visible;
                            ErrorText.Text = "Failed To Add Build";

                        }
                    }
                    else
                    {
                        ErrorText.Visibility = Visibility.Visible;
                        ErrorText.Text = "Make sure it contains *FortniteGame* and *Engine*";
                    }
                }
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message + "~CustomContetnDialog~");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string PathSearch = PathSearcher.Open();
            if (!string.IsNullOrEmpty(PathSearch) && PathSearch != "NotFound")
            {
                FortnitePath.Text = PathSearch;
                ErrorText.Visibility = Visibility.Collapsed;
            }
            else if (PathSearch == "NotFound")
            {
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = "Make sure it contains *FortniteGame* and *Engine*";
            }
        }

        private void LaunchingUI_ButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (PSBasics._FortniteProcess != null && !PSBasics._FortniteProcess.HasExited)
                PSBasics._FortniteProcess.Kill();
            if (FakeAC._FNLauncherProcess != null && !FakeAC._FNLauncherProcess.HasExited)
                FakeAC._FNLauncherProcess.Kill();
            if (FakeAC._FNAntiCheatProcess != null && !FakeAC._FNAntiCheatProcess.HasExited)
                FakeAC._FNAntiCheatProcess.Kill();
        }
    }
}
