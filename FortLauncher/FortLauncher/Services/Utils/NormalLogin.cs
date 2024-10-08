﻿using FortLauncher.Pages;
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
using FortLauncher.Services.Utils.Helpers;

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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                Loggers.Log("Please Enter Your FortBackend Details");
                LoginPage.snackbarService.Show("Error Occurred", "Please Enter Your FortBackend Details", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                return;
            }

            try
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

                        if (response.StatusCode == HttpStatusCode.BadRequest)
                        {

                            Loggers.Log(loginResponse.message);
                            LoginPage.snackbarService.Show("Error Occurred", loginResponse.message, ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(loginResponse.token))
                            {
                                IniHelper.WriteToConfig("Auth", "Token", loginResponse.token);
                                Login(loginResponse.token);
                            }

                            Loggers.Log("ERROR");
                            LoginPage.snackbarService.Show("Error Occurred", "ERROR", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                            //string responseBody = await response.Content.ReadAsStringAsync();
                            // System.Windows.MessageBox.Show(responseBody);
                        }

                    }
                    else
                    {
                        Loggers.Log("Server Down?");
                        LoginPage.snackbarService.Show("Error Occurred", "Server Down?", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                    }
                }
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message);
                LoginPage.snackbarService.Show("Error Occurred", "Please Check Server Status.", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
            }
           
        }

        public async void Login(string Token)
        {
            UserData.Token = Token;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", Token);
                    HttpResponseMessage response = await client.GetAsync(LauncherConfig.LoginOauthApi);

                    if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        if (!string.IsNullOrEmpty(responseBody))
                        {
                            LoginResponse launcherJson = JsonConvert.DeserializeObject<LoginResponse>(responseBody)!;

                            if (launcherJson != null)
                            {
                                Loggers.Log($"Logged in as {launcherJson.username}");

                                if (launcherJson.banned)
                                {
                                    Loggers.Log("You are banned from FortBackend");
                                    LoginPage.snackbarService.Show("Error Occurred", "You are banned from FortBackend", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                                    return;
                                }
                                else
                                {                                 
                                    UserData.UserName = launcherJson.username;
                                    loginPage.NavigationService.Navigate(new Home());
                                    return;
                                }
                            }
                        }
                    }

                    Loggers.Log("Server Error");
                    LoginPage.snackbarService.Show("Error Occurred", "Server Error", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                Loggers.Log(ex.Message);
                LoginPage.snackbarService.Show("Error Occurred", "Server Error", ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
            }
          

        }
    }
}
