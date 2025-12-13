using System;
using System.Windows;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;

namespace Tomany.TaskManagement
{
    public partial class ProjectDialog : Window
    {
        private readonly IProjectService _projectService;
        private readonly int _currentUserId;
        private readonly int? _projectId;

        public ProjectDto? ProjectData { get; private set; }

        public ProjectDialog(int currentUserId, int? projectId = null)
        {
            InitializeComponent();

            _currentUserId = currentUserId;
            _projectId = projectId;
            _projectService = ServiceFactory.CreateProjectService();

            TitleTextBlock.Text = projectId.HasValue ? "Edit Project" : "Create Project";
            SaveButton.Content = projectId.HasValue ? "Update" : "Create";

            if (projectId.HasValue)
            {
                LoadProjectData();
            }
        }

        private async void LoadProjectData()
        {
            try
            {
                if (!_projectId.HasValue) return;

                var project = await _projectService.GetProjectByIdAsync(_projectId.Value);
                if (project != null)
                {
                    ProjectNameTextBox.Text = project.ProjectName ?? string.Empty;
                    DescriptionTextBox.Text = project.ProjectDescription ?? string.Empty;

                    // Set status
                    foreach (System.Windows.Controls.ComboBoxItem item in StatusComboBox.Items)
                    {
                        if (item.Content.ToString()?.Equals(project.ProjectStatus, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            StatusComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProjectNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(ProjectNameTextBox.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectNameTextBox.Text))
            {
                MessageBox.Show("Project name is required.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Saving...";

                // Create ProjectDto with entered data
                ProjectData = new ProjectDto
                {
                    ProjectId = _projectId ?? 0,
                    ProjectName = ProjectNameTextBox.Text.Trim(),
                    ProjectDescription = string.IsNullOrWhiteSpace(DescriptionTextBox.Text)
                        ? null
                        : DescriptionTextBox.Text.Trim(),
                    ProjectStatus = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Active",
                    CreateBy = _currentUserId,
                    CreateAt = _projectId.HasValue ? null : DateTime.Now,
                    UpdateAt = _projectId.HasValue ? DateTime.Now : null
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving project: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SaveButton.IsEnabled = true;
                SaveButton.Content = _projectId.HasValue ? "Update" : "Create";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
