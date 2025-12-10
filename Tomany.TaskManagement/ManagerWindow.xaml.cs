using System.Windows;

namespace Tomany.TaskManagement
{
    public partial class ManagerWindow : Window
    {
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;

        public ManagerWindow(int accountId, string username)
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

        private void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Manage Projects";
            ContentTextBlock.Text = "Project management functionality will be implemented here.\n\nYou can:\n- View all your projects\n- Create new projects\n- Edit project details\n- Assign team members to projects\n- Track project progress\n- Close projects";
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
            ContentTitleTextBlock.Text = "My Profile";
            ContentTextBlock.Text = "Profile management functionality will be implemented here.\n\nYou can:\n- View your profile information\n- Edit your profile\n- Change password\n- Update contact details";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTitleTextBlock.Text = "Settings";
            ContentTextBlock.Text = "Settings functionality will be implemented here.\n\nYou can:\n- Change account settings\n- Update preferences\n- Manage notifications\n- Configure project defaults";
        }
    }
}

