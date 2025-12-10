using System.Windows;

namespace Tomany.TaskManagement
{
    public partial class HomeWindow : Window
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;

        public HomeWindow(int accountId, string username)
        {
            InitializeComponent();
            AccountId = accountId;
            Username = username;
            WelcomeTextBlock.Text = $"Welcome back, {username}!";
            UserInfoTextBlock.Text = username;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void MyTasksButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "My Tasks";
            ContentTextBlock.Text = "Your tasks functionality will be implemented here.\n\nYou can:\n- View your assigned tasks\n- Update task status\n- Submit task work\n- View task history";
        }

        private void MyProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "My Projects";
            ContentTextBlock.Text = "Your projects functionality will be implemented here.\n\nYou can:\n- View projects you're part of\n- See project details\n- View project members\n- Track project progress";
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "My Profile";
            ContentTextBlock.Text = "Profile management functionality will be implemented here.\n\nYou can:\n- View your profile information\n- Edit your profile\n- Change password\n- Update contact details";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Settings";
            ContentTextBlock.Text = "Settings functionality will be implemented here.\n\nYou can:\n- Change account settings\n- Update preferences\n- Manage notifications";
        }
    }
}

