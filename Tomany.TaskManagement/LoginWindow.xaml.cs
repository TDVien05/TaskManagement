using System.Windows;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement
{
    public partial class LoginWindow : Window
    {
        private readonly LoginService _loginService;
        private bool _isSubmitting;

        public LoginWindow()
        {
            InitializeComponent();
            var dbContext = new TaskManagementContext();
            var accountRepo = new AccountRepository(dbContext);
            _loginService = new LoginService(accountRepo);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSubmitting)
            {
                return;
            }

            _isSubmitting = true;
            LoginButton.IsEnabled = false;
            UpdateStatus("Logging in...");

            var request = new LoginRequest
            {
                Username = UsernameTextBox.Text?.Trim() ?? string.Empty,
                Password = PasswordBox.Password
            };

            var result = await _loginService.LoginAsync(request);

            if (result.Success)
            {
                UpdateStatus($"Welcome, {result.Username}!");
                
                // Navigate based on role
                if (result.Role?.ToLower() == "admin")
                {
                    var adminWindow = new AdminWindow(result.AccountId ?? 0, result.Username ?? string.Empty);
                    adminWindow.Show();
                    this.Close();
                }
                else
                {
                    var homeWindow = new HomeWindow(result.AccountId ?? 0, result.Username ?? string.Empty);
                    homeWindow.Show();
                    this.Close();
                }
            }
            else
            {
                UpdateStatus(result.Message);
                MessageBox.Show(result.Message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoginButton.IsEnabled = true;
            _isSubmitting = false;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ClearForm()
        {
            UsernameTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
        }

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
    }
}

