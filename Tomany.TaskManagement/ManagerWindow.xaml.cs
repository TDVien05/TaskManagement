using System;
using System.Windows;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;
using SystemTask = System.Threading.Tasks.Task;

namespace Tomany.TaskManagement
{
    public partial class ManagerWindow : Window
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        private readonly ProfileService _profileService;
        private ProfileDto? _currentProfile;
        private bool _isSavingProfile;
        private bool _isChangingPassword;

        public ManagerWindow(int accountId, string username)
        {
            InitializeComponent();
            AccountId = accountId;
            Username = username;
            WelcomeTextBlock.Text = $"Welcome, {username}";
            UserInfoTextBlock.Text = username;

            var dbContext = new TaskManagementContext();
            var accountRepo = new AccountRepository(dbContext);
            _profileService = new ProfileService(accountRepo);

            Loaded += ManagerWindow_Loaded;
        }

        private async void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProfileAsync();
            ShowDashboard();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowProjects();
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Manage Tasks";
            ContentTextBlock.Text = "Task management functionality will be implemented here.\n\nYou can:\n- View all tasks in your projects\n- Create new tasks\n- Assign tasks to team members\n- Update task status\n- Set task priorities\n- Track task progress";
        }

        private void TeamButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "My Team";
            ContentTextBlock.Text = "Team management functionality will be implemented here.\n\nYou can:\n- View team members\n- See team member profiles\n- Assign team members to projects\n- View team performance\n- Manage team roles";
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Reports";
            ContentTextBlock.Text = "Reports functionality will be implemented here.\n\nYou can:\n- View project reports\n- Generate task reports\n- View team performance reports\n- Export data\n- View analytics";
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowProfile();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Settings";
            ContentTextBlock.Text = "Settings functionality will be implemented here.\n\nYou can:\n- Change account settings\n- Update preferences\n- Manage notifications\n- Configure project defaults";
        }

        private void ShowDashboard()
        {
            ContentTitleTextBlock.Text = "Dashboard Overview";
            DashboardSection.Visibility = Visibility.Visible;
            ProjectsSection.Visibility = Visibility.Collapsed;
            ProfileSection.Visibility = Visibility.Collapsed;
        }

        private void ShowProjects()
        {
            ContentTitleTextBlock.Text = "Manage Projects";
            DashboardSection.Visibility = Visibility.Collapsed;
            ProjectsSection.Visibility = Visibility.Visible;
            ProfileSection.Visibility = Visibility.Collapsed;
            UpdateProfileSummary();
        }

        private void ShowProfile()
        {
            ContentTitleTextBlock.Text = "My Profile";
            DashboardSection.Visibility = Visibility.Collapsed;
            ProjectsSection.Visibility = Visibility.Collapsed;
            ProfileSection.Visibility = Visibility.Visible;
            ApplyProfileToForm();
        }

        private async SystemTask LoadProfileAsync()
        {
            var result = await _profileService.GetProfileAsync(AccountId);
            if (result.Success && result.Data != null)
            {
                _currentProfile = result.Data;
                ApplyProfileToForm();
                UpdateProfileSummary();
            }
            else
            {
                StatusTextBlock.Text = result.Message;
            }
        }

        private void ApplyProfileToForm()
        {
            if (_currentProfile == null)
            {
                return;
            }

            FirstNameTextBox.Text = _currentProfile.FirstName ?? string.Empty;
            LastNameTextBox.Text = _currentProfile.LastName ?? string.Empty;
            EmailTextBox.Text = _currentProfile.Email ?? string.Empty;
            PhoneTextBox.Text = _currentProfile.PhoneNumber ?? string.Empty;
            DobPicker.SelectedDate = _currentProfile.DateOfBirth?.ToDateTime(TimeOnly.MinValue);
            StatusTextBlock.Text = string.Empty;
        }

        private void UpdateProfileSummary()
        {
            if (_currentProfile == null)
            {
                ProfileSummaryTextBlock.Text = "Chưa có dữ liệu hồ sơ.";
                return;
            }

            var fullName = BuildFullName(_currentProfile.FirstName, _currentProfile.LastName);
            ProfileSummaryTextBlock.Text =
                $"Họ tên: {fullName}\nEmail: {_currentProfile.Email ?? "N/A"}\nPhone: {_currentProfile.PhoneNumber ?? "N/A"}";
        }

        private static string BuildFullName(string? firstName, string? lastName)
        {
            var fn = firstName?.Trim() ?? string.Empty;
            var ln = lastName?.Trim() ?? string.Empty;
            var full = $"{fn} {ln}".Trim();
            return string.IsNullOrEmpty(full) ? "N/A" : full;
        }

        private async void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSavingProfile)
            {
                return;
            }

            _isSavingProfile = true;
            SaveProfileButton.IsEnabled = false;
            StatusTextBlock.Text = "Đang lưu hồ sơ...";

            var request = new UpdateProfileRequest
            {
                AccountId = AccountId,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneTextBox.Text,
                DateOfBirth = DobPicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(DobPicker.SelectedDate.Value)
                    : null
            };

            var result = await _profileService.UpdateProfileAsync(request);
            StatusTextBlock.Text = result.Message;

            if (result.Success)
            {
                await LoadProfileAsync();
            }

            SaveProfileButton.IsEnabled = true;
            _isSavingProfile = false;
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isChangingPassword)
            {
                return;
            }

            _isChangingPassword = true;
            ChangePasswordButton.IsEnabled = false;
            StatusTextBlock.Text = "Đang đổi mật khẩu...";

            var request = new ChangePasswordRequest
            {
                AccountId = AccountId,
                CurrentPassword = CurrentPasswordBox.Password,
                NewPassword = NewPasswordBox.Password,
                ConfirmPassword = ConfirmPasswordBox.Password
            };

            var result = await _profileService.ChangePasswordAsync(request);
            StatusTextBlock.Text = result.Message;

            if (result.Success)
            {
                CurrentPasswordBox.Clear();
                NewPasswordBox.Clear();
                ConfirmPasswordBox.Clear();
            }

            ChangePasswordButton.IsEnabled = true;
            _isChangingPassword = false;
        }
    }
}

