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
        private readonly IAccountService _accountService;
        private readonly IProjectService _projectService;

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
            ManagerRequestsView.Visibility = Visibility.Collapsed;
            UserManagementView.Visibility = Visibility.Visible;

            await RefreshUsersGrid();
        }

        private async void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            UserManagementView.Visibility = Visibility.Collapsed;
            ManagerRequestsView.Visibility = Visibility.Collapsed;
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
        
        private async void ManagerRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardView.Visibility = Visibility.Collapsed;
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            ManagerRequestsView.Visibility = Visibility.Visible;

            await RefreshManagerRequestsGrid();
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            var taskBoardWindow = new TaskBoardWindow(AccountId, "Admin", this);
            taskBoardWindow.Show();
            this.Hide();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            ManagerRequestsView.Visibility = Visibility.Collapsed;
            DashboardView.Visibility = Visibility.Visible;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementView.Visibility = Visibility.Collapsed;
            ProjectManagementView.Visibility = Visibility.Collapsed;
            ManagerRequestsView.Visibility = Visibility.Collapsed;
            DashboardView.Visibility = Visibility.Visible;
        }

        private async void UserBlockUnblock_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is not ProfileDto selectedUser)
            {
                MessageBox.Show("Please select a user to block/unblock.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string action = selectedUser.IsActive ? "block" : "unblock";
            var result = MessageBox.Show($"Are you sure you want to {action} the user '{selectedUser.Username}'?", "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _accountService.ToggleAccountStatusAsync(selectedUser.AccountId);
                    MessageBox.Show($"User '{selectedUser.Username}' has been successfully {action}ed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshUsersGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void UserResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is not ProfileDto selectedUser)
            {
                MessageBox.Show("Please select a user to reset their password.", "No User Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to reset the password for '{selectedUser.Username}'?", "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string newPassword = await _accountService.ResetPasswordAsync(selectedUser.AccountId);
                    MessageBox.Show($"Password for '{selectedUser.Username}' has been reset to:\n\n{newPassword}\n\nPlease provide this to the user.", "Password Reset Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while resetting the password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private async void ApproveRequest_Click(object sender, RoutedEventArgs e)
        {
            if (ManagerRequestsDataGrid.SelectedItem is not ProfileDto selectedUser)
            {
                MessageBox.Show("Please select a request to approve.", "No Request Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to approve '{selectedUser.Username}' as a Manager?", "Confirm Approval", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _accountService.ApproveRequestAsync(selectedUser.AccountId);
                    MessageBox.Show($"User '{selectedUser.Username}' has been promoted to Manager.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshManagerRequestsGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void RejectRequest_Click(object sender, RoutedEventArgs e)
        {
            if (ManagerRequestsDataGrid.SelectedItem is not ProfileDto selectedUser)
            {
                MessageBox.Show("Please select a request to reject.", "No Request Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to reject the manager request for '{selectedUser.Username}'?", "Confirm Rejection", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _accountService.RejectRequestAsync(selectedUser.AccountId);
                    MessageBox.Show($"The manager request for '{selectedUser.Username}' has been rejected. The user remains a 'User'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await RefreshManagerRequestsGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async System.Threading.Tasks.Task RefreshUsersGrid()
        {
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
        
        private async System.Threading.Tasks.Task RefreshManagerRequestsGrid()
        {
            try
            {
                var applicants = await _accountService.GetUsersByRoleAsync("Applicant");
                ManagerRequestsDataGrid.ItemsSource = applicants;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading manager requests: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
