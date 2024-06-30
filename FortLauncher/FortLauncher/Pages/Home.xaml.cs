using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using FortLauncher.Services.Globals;

namespace FortLauncher.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public HomeTB HomeTB { get; set; } = new HomeTB();
        public LibraryTB  LibraryTB { get; set; } = new LibraryTB();
        public Home()
        {
            InitializeComponent();
            TestBox.Text = UserData.Token;
            UserNameBox.Text = UserData.UserName;
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

        private void CustomContetnDialog_ButtonClicked(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
