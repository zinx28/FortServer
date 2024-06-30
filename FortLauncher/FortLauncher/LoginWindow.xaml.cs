using DiscordRPC;
using DiscordRPC.Logging;
using System;
using System.IO;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media;
using System.CodeDom;
using Newtonsoft.Json.Linq;

namespace WpfApp6
{
    public partial class LoginWindow : Window
    {
        private bool rememberMe = false;
        public bool LoginSuccessful { get; private set; }
 

        public LoginWindow()
        {
            InitializeComponent();
            LoadRememberMeState();
            AutoFillIfRemembered();

        }

        private void Email_GotFocus(object sender, RoutedEventArgs e)
        {

            if (Email.Text == "Email")
            {
                Email.Text = "";
                Email.Foreground = Brushes.White;
            }
        }

        private void Email_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                Email.Text = "Email";
                Email.Foreground = Brushes.Gray; // Set the placeholder text color
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
            PasswordBoxControl.Visibility = Visibility.Visible;
            PasswordBoxControl.Focus();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(PasswordBoxControl.Password) && !PasswordBoxControl.IsFocused)
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
                PasswordBoxControl.Visibility = Visibility.Collapsed;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(PasswordBoxControl.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void PasswordPlaceholder_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
            PasswordBoxControl.Visibility = Visibility.Visible;
            PasswordBoxControl.Focus();
        }

        private void SaveCredentials(string email, string password, bool rememberMe)
        {
            try
            {

                dynamic credentials = new
                {
                    Email = email,
                    Password = password,
                    RememberMe = rememberMe
                };

                string json = JsonConvert.SerializeObject(credentials);


                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;


                string filePath = Path.Combine(directoryPath, "credentials.json");

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error saving credentials: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadRememberMeState()
        {
            try
            {

                if (File.Exists("credentials.json"))
                {

                    string json = File.ReadAllText("credentials.json");
                    dynamic credentials = JsonConvert.DeserializeObject(json);


                    bool? rememberMe = credentials.RememberMe;


                    if (rememberMe != null)
                    {
                        RememberMeCheckBox.IsChecked = rememberMe;


                        if (rememberMe == true)
                        {
                            Email.Text = credentials.Email;
                            PasswordBoxControl.Password = credentials.Password;


                            PasswordPlaceholder.Visibility = Visibility.Collapsed;
                            PasswordBoxControl.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Remember Me state: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AutoFillIfRemembered()
        {

            if (RememberMeCheckBox.IsChecked == true)
            {
                LoadCredentials(); 
            }
        }

        private void LoadCredentials()
        {
            try
            {

                if (File.Exists("credentials.json"))
                {

                    string json = File.ReadAllText("credentials.json");
                    dynamic credentials = JsonConvert.DeserializeObject(json);


                    string email = credentials.Email;
                    string password = credentials.Password;


                    Email.Text = email;
                    PasswordBoxControl.Password = password;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading credentials: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RememberMeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            rememberMe = true; 
            SaveCredentials(Email.Text, PasswordBoxControl.Password, true);
        }

        private void RememberMeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            rememberMe = false; 
            SaveCredentials("", "", false);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = Email.Text;
            string password = PasswordBoxControl.Password;
            bool rememberMe = RememberMeCheckBox.IsChecked ?? false;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage.Text = "Email and Password are required.";
                return;
            }

            SaveCredentials(email, password, rememberMe);


            LoginSuccessful = true;

            Close();
        }
    }
}
