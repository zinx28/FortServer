using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using WpfApp1.Services.Saved;

namespace WpfApp1.Services.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process openbrowser = Process.Start(new ProcessStartInfo("PUT YOUR LOGIN LINK HERE") { UseShellExecute = true });

            Task.Run(async () =>
            {
                Dispatcher.Invoke(() =>
                {
                    CallBackButton.IsEnabled = false;
                    CallBackButton.Content = "Waiting for callback";
                });

                var listener = new HttpListener();
		// THIS IS FOR THE LAUNCHER DONT CHANGE THE PORT
                listener.Prefixes.Add("http://127.0.0.1:2158/callback/");
                listener.Start();


                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                
                byte[] Buffer = System.Text.Encoding.UTF8.GetBytes("<!DOCTYPE html><head><title>CLOSE THIS PAGE</title></head><body>If you see this we couldnt close the page just close this! this is just for us to you into the launcher! <script> function test() { window.close(); } setInterval(function() { test(); }, 1000)</script></body>");
                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "text/html";
                response.ContentLength64 = Buffer.Length;
                await response.OutputStream.WriteAsync(Buffer, 0, Buffer.Length);
                response.OutputStream.Close();
                var code = request.QueryString.Get("code");
                response.Close();
              

                Dispatcher.Invoke(() =>
                {
                    UserData.Token = code;
                    NavigationService.Navigate(new Home());
                });

               

                await Task.Delay(1000);

                listener.Stop();

                if (openbrowser != null && !openbrowser.HasExited)
                {
                    openbrowser.Kill();
                }
            });
        }
    }
}
