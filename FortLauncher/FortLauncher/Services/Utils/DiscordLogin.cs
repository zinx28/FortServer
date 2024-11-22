using FortLauncher.Pages;
using FortLauncher.Services.Globals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FortLauncher.Services.Utils
{
    public class DiscordLogin
    {
        private LoginPage loginPage;

        public DiscordLogin(LoginPage mainNav)
        {
            this.loginPage = mainNav;
        }
        public async void Init()
        {
            Process openbrowser = Process.Start(new ProcessStartInfo(LauncherConfig.DiscordURl) { UseShellExecute = true });

            await Task.Run(async () =>
            {
                loginPage.Dispatcher.Invoke(() =>
                {
                    loginPage.CallBackButton.IsEnabled = false;
                    loginPage.CallBackButton.Content = "Waiting for callback";
                });

                // THIS WILL NEED TO BE RECODED - TODO IG?!?!?!!?!?
                var listener = new HttpListener();
                // THIS IS FOR THE LAUNCHER DONT CHANGE THE PORT
                listener.Prefixes.Add("http://127.0.0.1:2158/callback/");
                listener.Start();


                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                response.Headers.Add("Access-Control-Allow-Origin", "*");

                byte[] Buffer = System.Text.Encoding.UTF8.GetBytes(@"
                        <!DOCTYPE html>
                        <head>
                        <title>CLOSE THIS PAGE</title>
                        </head>
                        <body>If you see this we couldnt close the page just close this! this is just for us to you into the launcher! 
                        <script> function test() { window.close(); } setInterval(function() { test(); }, 1000)</script>
                        </body>");
                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "text/html";
                response.ContentLength64 = Buffer.Length;
                await response.OutputStream.WriteAsync(Buffer, 0, Buffer.Length);
                response.OutputStream.Close();
                var code = request.QueryString.Get("code");
                response.Close();


                loginPage.Dispatcher.Invoke(() =>
                {
                    UserData.Token = code;
                    loginPage.NavigationService.Navigate(new Home());
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
