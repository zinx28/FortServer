using DiscordRPC;
using System;
using System.Windows;
using System.Windows.Navigation;
using ModernWpf.Controls;
using System.Net;
using WpfApp6.Pages;

namespace WpfApp6
{
    public partial class MainWindow : Window
    {
        Home home = new Home();
        Settings settings = new Settings();
        faq FAQ = new faq();

        DiscordRpcClient client;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            HomeItem.IsSelected = true;
        }

        private void NavView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(settings);
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                if (item.Tag.ToString() == "Home")
                {
                    ContentFrame.Navigate(home);
                }
                else if (item.Tag.ToString() == "faq")
                {
                    ContentFrame.Navigate(FAQ);
                }
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page.");
        }
    }
}