using System;
using System.Windows;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement
{
    public partial class AdminWindow : Window
    {
        private readonly AccountService _accountService;
        private readonly ProjectService _projectService;

        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;

        public AdminWindow(int accountId, string username)
        {
            InitializeComponent();
            
            var context = new TaskManagementContext();
            var accountRepository = new AccountRepository(context);
            _accountService = new AccountService(accountRepository);
            var projectRepository = new ProjectRepository(context);
            _projectService = new ProjectService(projectRepository);

            AccountId = accountId;
            Username = username;
            WelcomeTextBlock.Text = $"Welcome, {username}";
            UserInfoTextBlock.Text = username;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private async void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            UserManagementView.Visibility = Visibility.Visible;

            try
            {
                var users = await _accountService.GetAllAsync();
                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Visible;

            try
            {
                var projects = await _projectService.GetAllAsync();
                ProjectsDataGrid.ItemsSource = projects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            DashboardView.Visibility = Visibility.Visible;
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            DashboardView.Visibility = Visibility.Visible;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            DashboardView.Visibility = Visibility.Visible;
        }

        private void UserBlockUnblock_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is ProfileDto selectedUser)
            {
                MessageBox.Show($"Placeholder.", "Placeholder", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a user to block/unblock.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UserResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is ProfileDto selectedUser)
            {
                MessageBox.Show($"Placeholder.", "Placeholder", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a user to reset password.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

