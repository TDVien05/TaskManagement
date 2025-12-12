using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data; 
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;
using TaskManagementContext = Tomany.TaskManagement.DAL.Models.TaskManagementContext;

namespace Tomany.TaskManagement
{
    public partial class HomeWindow : Window
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        private readonly ProfileService _profileService;
        private readonly ProjectService _projectService; 
        private ProfileDto? _currentProfile;
        private bool _isSavingProfile;
        private bool _isChangingPassword;

        public HomeWindow(int accountId, string username)
        {
            InitializeComponent();
            AccountId = accountId;
            Username = username;
            WelcomeTextBlock.Text = $"Welcome back, {username}!";
            UserInfoTextBlock.Text = username;

            var dbContext = new TaskManagementContext();
            var accountRepo = new AccountRepository(dbContext);
            _profileService = new ProfileService(accountRepo);

            var projectRepo = new ProjectRepository(dbContext);
            _projectService = new ProjectService(projectRepo);

            // Initialize collections and view to satisfy non-nullable requirements
            Projects = new ObservableCollection<Project>();
            _projectsView = CollectionViewSource.GetDefaultView(Projects);
            _projectsView.Filter = ApplyProjectsFilter;
            ProjectsListView.ItemsSource = _projectsView;

            Loaded += HomeWindow_Loaded;
        }

        private async void HomeWindow_Loaded(object sender, RoutedEventArgs e)
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

        private void MyTasksButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
            ContentTitleTextBlock.Text = "My Tasks";
            ContentTextBlock.Text = "Your tasks functionality will be implemented here.\n\nYou can:\n- View your assigned tasks\n- Update task status\n- Submit task work\n- View task history";
        }

        private async void MyProjectsButton_Click(object sender, RoutedEventArgs e) 
        {
            ShowProjects();
            await LoadProjectsAsync(); 
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ShowProfile();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Settings";
            ContentTextBlock.Text = "Settings functionality will be implemented here.\n\nYou can:\n- Change account settings\n- Update preferences\n- Manage notifications";
        }

        private void ShowDashboard()
        {
            ContentTitleTextBlock.Text = "Dashboard";
            DashboardSection.Visibility = Visibility.Visible;
            ProjectsSection.Visibility = Visibility.Collapsed;
            ProfileSection.Visibility = Visibility.Collapsed;
        }

        private void ShowProjects()
        {
            ContentTitleTextBlock.Text = "My Projects";
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

        public ObservableCollection<Project> Projects { get; set; } // New Property
        private ICollectionView _projectsView; // New Field

        private async System.Threading.Tasks.Task LoadProjectsAsync()
        {
            try
            {
                var projectsList = await _projectService.GetProjectsByAccountIdAsync(AccountId);
                Projects.Clear();
                if (projectsList != null)
                {
                    foreach (var project in projectsList)
                    {
                        Projects.Add(project);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optionally log the error
            }
        }


        private async System.Threading.Tasks.Task LoadProfileAsync()
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

        // --- Filter Logic for Projects ---
        private void ProjectSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ProjectSearchTextBox.Text == "Search projects..." && ProjectSearchTextBox.FontStyle == FontStyles.Italic)
            {
                ProjectSearchTextBox.Text = "";
                ProjectSearchTextBox.FontStyle = FontStyles.Normal;
                ProjectSearchTextBox.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#52725b");
            }
        }

        private void ProjectSearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectSearchTextBox.Text))
            {
                ProjectSearchTextBox.Text = "Search projects...";
                ProjectSearchTextBox.FontStyle = FontStyles.Italic;
                ProjectSearchTextBox.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#999");
            }
        }

        private void ProjectSearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_projectsView != null)
            {
                _projectsView.Refresh();
            }
        }

        private bool ApplyProjectsFilter(object item)
        {
            if (item is Project project)
            {
                string searchText = ProjectSearchTextBox.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText) || searchText == "search projects...")
                {
                    return true; // No filter applied
                }

                return project.ProjectName.ToLower().Contains(searchText) ||
                       (project.ProjectDescription != null && project.ProjectDescription.ToLower().Contains(searchText));
            }
            return false;
        }

        // --- Project Detail View Logic ---
        private void ShowProjectDetailView()
        {
            ProjectsSection.Visibility = Visibility.Collapsed;
            ProjectDetailSection.Visibility = Visibility.Visible;
            ContentTitleTextBlock.Text = "Project Details";
        }

        private void HideProjectDetailView()
        {
            ProjectDetailSection.Visibility = Visibility.Collapsed;
            ShowProjects(); // Re-show the projects list
        }

        private void BackToProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            HideProjectDetailView();
        }

        private async void ProjectsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProjectsListView.SelectedItem is Project selectedProject)
            {
                var projectDetails = await _projectService.GetProjectByIdAsync(selectedProject.ProjectId);
                if (projectDetails != null)
                {
                    ProjectDetailNameTextBlock.Text = projectDetails.ProjectName;
                    ProjectDetailDescriptionTextBlock.Text = projectDetails.ProjectDescription;
                    TasksListView.ItemsSource = projectDetails.Tasks;
                    MembersItemsControl.ItemsSource = projectDetails.ProjectMembers;

                    ShowProjectDetailView();
                }
                else
                {
                    MessageBox.Show("Could not load project details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

