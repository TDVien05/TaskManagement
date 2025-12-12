using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;

namespace Tomany.TaskManagement
{
    public partial class TaskBoardWindow : Window
    {
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;
        private List<TaskDto> _allTasks = new();
        private Window? _parentWindow;
        private int _currentUserId;
        private string? _currentUserRole;
        private int? _currentProjectId;
        private bool? _isProjectManager;

        public TaskBoardWindow(int accountId, string? role, Window? parentWindow = null)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUserId = accountId;
            _currentUserRole = role;
            
            _taskService = ServiceFactory.CreateTaskService();
            _projectService = ServiceFactory.CreateProjectService();
            
            LoadProjects();
        }

        private async void LoadProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                
                ProjectComboBox.ItemsSource = projects;
                
                if (projects.Count > 0)
                {
                    ProjectComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectComboBox.SelectedItem is ProjectDto selectedProject)
            {
                ProjectNameTextBlock.Text = selectedProject.ProjectName;
                _currentProjectId = selectedProject.ProjectId;
                _isProjectManager = null;
                await LoadTasks(selectedProject.ProjectId);
            }
        }

        private async Task LoadTasks(int projectId)
        {
            try
            {
                _allTasks = await _taskService.GetTasksByProjectIdAsync(projectId);
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            var statusFilter = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var deadlineFilter = DeadlineFilterDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DeadlineFilterDatePicker.SelectedDate.Value)
                : (DateOnly?)null;

            var filteredTasks = _taskService.FilterTasks(_allTasks, statusFilter, null, deadlineFilter);

            var pendingTasks = filteredTasks
                .Where(t => string.IsNullOrWhiteSpace(t.TaskStatus) || 
                           t.TaskStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            var inProgressTasks = filteredTasks
                .Where(t => t.TaskStatus?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            
            var reviewTasks = filteredTasks
                .Where(t => t.TaskStatus?.Equals("Review", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            
            var completedTasks = filteredTasks
                .Where(t => t.TaskStatus?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            PendingTasksItemsControl.ItemsSource = pendingTasks;
            InProgressTasksItemsControl.ItemsSource = inProgressTasks;
            ReviewTasksItemsControl.ItemsSource = reviewTasks;
            CompletedTasksItemsControl.ItemsSource = completedTasks;

            PendingCountTextBlock.Text = $"{pendingTasks.Count} task{(pendingTasks.Count != 1 ? "s" : "")}";
            InProgressCountTextBlock.Text = $"{inProgressTasks.Count} task{(inProgressTasks.Count != 1 ? "s" : "")}";
            ReviewCountTextBlock.Text = $"{reviewTasks.Count} task{(reviewTasks.Count != 1 ? "s" : "")}";
            CompletedCountTextBlock.Text = $"{completedTasks.Count} task{(completedTasks.Count != 1 ? "s" : "")}";

            TaskCountTextBlock.Text = $"{filteredTasks.Count} task(s)";
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DeadlineFilterDatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = 0;
            DeadlineFilterDatePicker.SelectedDate = null;
            ApplyFilters();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectComboBox.SelectedItem is ProjectDto selectedProject)
            {
                await LoadTasks(selectedProject.ProjectId);
            }
        }

        private async void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentProjectId.HasValue)
            {
                MessageBox.Show("Please select a project first.", "Validation", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new TaskDetailDialog(_currentProjectId.Value, _currentUserId, _currentUserRole);
            dialog.Owner = this;
            
            if (dialog.ShowDialog() == true)
            {
                await LoadTasks(_currentProjectId.Value);
            }
        }

        private async void TaskCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is FrameworkElement element && element.DataContext is TaskDto task)
            {
                if (!_currentProjectId.HasValue)
                {
                    return;
                }

                var dialog = new TaskDetailDialog(_currentProjectId.Value, _currentUserId, _currentUserRole, task.TaskId);
                dialog.Owner = this;
                
                if (dialog.ShowDialog() == true)
                {
                    await LoadTasks(_currentProjectId.Value);
                }
            }
        }

        private void StatusComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is TaskDto task)
            {
                var allowedStatuses = task.GetAllowedStatuses(_currentUserRole);
                
                comboBox.Items.Clear();
                
                foreach (var status in allowedStatuses)
                {
                    var item = new ComboBoxItem { Content = status };
                    comboBox.Items.Add(item);
                }
                
                comboBox.SelectedValue = task.TaskStatus ?? "Pending";
            }
        }

        private async void TaskStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is TaskDto task)
            {
                if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0)
                {
                    return;
                }

                var newStatus = (e.AddedItems[0] as ComboBoxItem)?.Content?.ToString();
                var oldStatus = task.TaskStatus ?? "Pending";

                if (string.IsNullOrWhiteSpace(newStatus) || newStatus == oldStatus)
                {
                    comboBox.SelectedValue = oldStatus;
                    return;
                }

                if (!_currentProjectId.HasValue)
                {
                    comboBox.SelectedValue = oldStatus;
                    return;
                }

                try
                {
                    var updatedTask = await _taskService.UpdateTaskStatusAsync(
                        task.TaskId, 
                        newStatus, 
                        _currentUserId, 
                        _currentUserRole, 
                        _currentProjectId.Value);

                    var index = _allTasks.FindIndex(t => t.TaskId == task.TaskId);
                    if (index >= 0)
                    {
                        _allTasks[index] = updatedTask;
                    }

                    ApplyFilters();
                    
                    if (newStatus.Equals("Review", StringComparison.OrdinalIgnoreCase))
                    {
                        var reviewTasks = _allTasks
                            .Where(t => t.TaskStatus?.Equals("Review", StringComparison.OrdinalIgnoreCase) == true)
                            .ToList();
                        ReviewTasksItemsControl.ItemsSource = null;
                        ReviewTasksItemsControl.ItemsSource = reviewTasks;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    comboBox.SelectedValue = oldStatus;
                    MessageBox.Show(ex.Message, "Permission Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    comboBox.SelectedValue = oldStatus;
                    MessageBox.Show($"Error updating status: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ApproveButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskDto task)
            {
                var isReviewStatus = task.TaskStatus?.Equals("Review", StringComparison.OrdinalIgnoreCase) == true;
                
                if (!isReviewStatus || !_currentProjectId.HasValue)
                {
                    button.Visibility = Visibility.Collapsed;
                    return;
                }

                var isSystemManagerOrAdmin = string.Equals(_currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase) ||
                                            string.Equals(_currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);
                
                if (!isSystemManagerOrAdmin)
                {
                    button.Visibility = Visibility.Collapsed;
                    return;
                }

                try
                {
                    if (!_isProjectManager.HasValue)
                    {
                        _isProjectManager = await _projectService.IsUserProjectManagerAsync(_currentProjectId.Value, _currentUserId);
                    }
                    
                    button.Visibility = _isProjectManager.Value ? Visibility.Visible : Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error checking project manager status: {ex.Message}");
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskDto task)
            {
                if (!_currentProjectId.HasValue)
                {
                    MessageBox.Show("Project not selected.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to approve task '{task.TaskName}'? This will mark it as Completed.",
                    "Confirm Approval",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                try
                {
                    button.IsEnabled = false;
                    button.Content = "Approving...";

                    var updatedTask = await _taskService.ApproveTaskAsync(
                        task.TaskId,
                        _currentUserId,
                        _currentProjectId.Value);

                    var index = _allTasks.FindIndex(t => t.TaskId == task.TaskId);
                    if (index >= 0)
                    {
                        _allTasks[index] = updatedTask;
                    }

                    ApplyFilters();
                    
                    _isProjectManager = null;

                    MessageBox.Show("Task approved successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Permission Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Invalid Operation", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error approving task: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    button.IsEnabled = true;
                    button.Content = "Approve";
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_parentWindow != null)
            {
                _parentWindow.Show();
            }
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
