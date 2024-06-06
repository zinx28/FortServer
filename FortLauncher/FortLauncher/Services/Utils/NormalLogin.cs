using FortLauncher.Pages;
using FortLauncher.Services.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using Wpf.Ui;
using System.Net;
using Newtonsoft.Json;
using Wpf.Ui.Extensions;

namespace FortLauncher.Services.Utils
{
    public class MainLogin
    {
        private LoginPage loginPage;

        public MainLogin(LoginPage mainNav)
        {
            this.loginPage = mainNav;
        }
        public async void Init(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(email), "email");
                formData.Add(new StringContent(password), "password");

                HttpResponseMessage response = await client.PostAsync(LauncherConfig.LoginOauthApi, formData);


                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginClass loginResponse = JsonConvert.DeserializeObject<LoginClass>(responseBody);

                    if(response.StatusCode == HttpStatusCode.BadRequest)
                    {

                        LoginPage.snackbarService.Show("Error Occurred", loginResponse.message, ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(loginResponse.token))
                        {
                            IniHelper.WriteToConfig("Auth", "Token", loginResponse.token);
                            Login(loginResponse.token);
                        }

                        LoginPage.snackbarService.Show("Error Occurred", "ERROR", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                        //string responseBody = await response.Content.ReadAsStringAsync();
                        // System.Windows.MessageBox.Show(responseBody);
                    }
                   
                }
                else
                {
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //System.Windows.MessageBox.Show(responseBody);
                    //System.Windows.MessageBox.Show($"Failed to call API. Status code: {response.StatusCode}");

                    LoginPage.snackbarService.Show("Error Occurred", "Server Down?", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                }
            }
        }

        public async void Login(string Token)
        {
            UserData.Token = Token;
            loginPage.NavigationService.Navigate(new Home());
        }
    }
}
