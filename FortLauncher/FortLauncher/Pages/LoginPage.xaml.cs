using FortLauncher.Services.Globals;
using FortLauncher.Services.Utils;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WpfApp1;

namespace FortLauncher.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public static ISnackbarService? snackbarService { get; set; }
        public LoginPage(/*ISnackbarService snackbarService*/)
        {
            InitializeComponent();
            // snackbarService1 = snackbarService;
            snackbarService = new SnackbarService();
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DiscordLogin discordLogin = new DiscordLogin(this);
            discordLogin.Init();
            //DiscordLogin.Init();
        }

        private void NormalLogin_Click(object sender, RoutedEventArgs e)
        {
            MainLogin mainLogin = new MainLogin(this);
            mainLogin.Init(EmailBox.Text, PasswordBox.Text);
           
           // MainLogin.Init(EmailBox.Text, PasswordBox.Text);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string Token = IniHelper.ReadValue("Auth", "Token");
            if (Token != "NONE")
            {
                MainLogin mainLogin = new MainLogin(this);
                mainLogin.Login(Token);
                //System.Windows.MessageBox.Show("FOUND A TOKEN SAVED" + Token);
            }
        }
    }
}
