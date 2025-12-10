using System.Windows;

namespace Tomany.TaskManagement
{
    public partial class AdminWindow : Window
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;

        public AdminWindow(int accountId, string username)
        {
            InitializeComponent();
            AccountId = accountId;
            Username = username;
            WelcomeTextBlock.Text = $"Welcome, {username}";
            UserInfoTextBlock.Text = username;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Manage Users";
            ContentTextBlock.Text = "User management functionality will be implemented here.\n\nYou can:\n- View all users\n- Edit user information\n- Delete users\n- Change user roles";
        }

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Manage Projects";
            ContentTextBlock.Text = "Project management functionality will be implemented here.\n\nYou can:\n- View all projects\n- Create new projects\n- Edit projects\n- Delete projects";
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Manage Tasks";
            ContentTextBlock.Text = "Task management functionality will be implemented here.\n\nYou can:\n- View all tasks\n- Create new tasks\n- Assign tasks to users\n- Update task status";
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Reports";
            ContentTextBlock.Text = "Reports functionality will be implemented here.\n\nYou can:\n- View system statistics\n- Generate activity reports\n- Export data";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Settings";
            ContentTextBlock.Text = "System settings functionality will be implemented here.\n\nYou can:\n- Configure system settings\n- Manage email settings\n- Update preferences";
        }
    }
}

