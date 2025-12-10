using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;

namespace Tomany.TaskManagement
{
    public partial class TaskDetailDialog : Window
    {
        private readonly TaskService _taskService;
        private readonly ProjectService _projectService;
        private readonly int _projectId;
        private readonly int _currentUserId;
        private readonly string? _currentUserRole;
        private readonly int? _taskId;
        private bool _isManagerOrAdmin;

        public TaskDetailDialog(int projectId, int currentUserId, string? currentUserRole, int? taskId = null)
        {
            InitializeComponent();
            
            _projectId = projectId;
            _currentUserId = currentUserId;
            _currentUserRole = currentUserRole;
            _taskId = taskId;
            _isManagerOrAdmin = string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);

            _taskService = ServiceFactory.CreateTaskService();
            _projectService = ServiceFactory.CreateProjectService();

            TitleTextBlock.Text = taskId.HasValue ? "Edit Task" : "Create Task";
            SaveButton.Content = taskId.HasValue ? "Update" : "Create";

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var members = await _projectService.GetProjectMembersAsync(_projectId);
                
                var unassignedOption = new UserDto { AccountId = -1, Username = "Unassigned", FullName = "Unassigned" };
                var assigneeList = new List<UserDto> { unassignedOption };
                assigneeList.AddRange(members);
                
                AssigneeComboBox.ItemsSource = assigneeList;
                
                if (!_taskId.HasValue)
                {
                    AssigneeComboBox.SelectedValue = -1;
                }

                if (!_isManagerOrAdmin)
                {
                    AssigneeComboBox.IsEnabled = false;
                    AssigneeHintTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    AssigneeHintTextBlock.Visibility = Visibility.Collapsed;
                }

                if (_taskId.HasValue)
                {
                    var tasks = await _taskService.GetTasksByProjectIdAsync(_projectId);
                    var task = tasks.FirstOrDefault(t => t.TaskId == _taskId.Value);
                    
                    if (task != null)
                    {
                        TaskNameTextBox.Text = task.TaskName;
                        DescriptionTextBox.Text = task.TaskDescription ?? string.Empty;
                        
                        if (task.AssignedTo.HasValue)
                        {
                            var assignedUser = assigneeList.FirstOrDefault(u => u.AccountId == task.AssignedTo.Value);
                            if (assignedUser != null)
                            {
                                AssigneeComboBox.SelectedValue = task.AssignedTo.Value;
                            }
                            else
                            {
                                AssigneeComboBox.SelectedValue = -1;
                            }
                        }
                        else
                        {
                            AssigneeComboBox.SelectedValue = -1;
                        }

                        if (task.DueDate.HasValue)
                        {
                            DeadlineDatePicker.SelectedDate = task.DueDate.Value.ToDateTime(TimeOnly.MinValue);
                        }
                        
                        UpdatePriorityDisplay();

                        var statusItems = StatusComboBox.Items.Cast<System.Windows.Controls.ComboBoxItem>().ToList();
                        var statusItem = statusItems.FirstOrDefault(item => 
                            item.Content.ToString()?.Equals(task.TaskStatus, StringComparison.OrdinalIgnoreCase) == true);
                        if (statusItem != null)
                        {
                            StatusComboBox.SelectedItem = statusItem;
                        }

                        LinkSubmissionTextBox.Text = task.LinkSubmission ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(TaskNameTextBox.Text);
        }

        private void DeadlineDatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DeadlineDatePicker.SelectedDate.HasValue)
            {
                var selectedDate = DeadlineDatePicker.SelectedDate.Value.Date;
                var today = DateTime.Today;
                
                if (selectedDate < today)
                {
                }
            }
            
            UpdatePriorityDisplay();
        }

        private void UpdatePriorityDisplay()
        {
            if (DeadlineDatePicker.SelectedDate.HasValue)
            {
                var dueDate = DateOnly.FromDateTime(DeadlineDatePicker.SelectedDate.Value);
                var daysUntilDue = (dueDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;
                
                string priority;
                string priorityIcon;
                if (daysUntilDue < 0)
                {
                    priority = "Overdue";
                    priorityIcon = "🔴";
                }
                else if (daysUntilDue <= 3)
                {
                    priority = "High";
                    priorityIcon = "🔴";
                }
                else if (daysUntilDue <= 7)
                {
                    priority = "Medium";
                    priorityIcon = "🟡";
                }
                else
                {
                    priority = "Low";
                    priorityIcon = "🟢";
                }
                
                PriorityInfoTextBlock.Text = $"Priority: {priorityIcon} {priority} ({daysUntilDue} day{(daysUntilDue != 1 ? "s" : "")} until deadline)";
            }
            else
            {
                PriorityInfoTextBlock.Text = "Priority: 🟢 Low (no deadline set)";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskNameTextBox.Text))
            {
                MessageBox.Show("Task title is required.", "Validation Error", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Saving...";

                var request = new TaskRequest
                {
                    TaskName = TaskNameTextBox.Text.Trim(),
                    TaskDescription = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                    ProjectId = _projectId,
                    AssignedTo = AssigneeComboBox.SelectedValue is int assigneeId && assigneeId > 0 ? assigneeId : null,
                    TaskStatus = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Pending",
                    DueDate = DeadlineDatePicker.SelectedDate.HasValue 
                        ? DateOnly.FromDateTime(DeadlineDatePicker.SelectedDate.Value) 
                        : null,
                    LinkSubmission = string.IsNullOrWhiteSpace(LinkSubmissionTextBox.Text) ? null : LinkSubmissionTextBox.Text.Trim()
                };

                TaskDto result;
                if (_taskId.HasValue)
                {
                    result = await _taskService.UpdateTaskAsync(_taskId.Value, request, _currentUserId, _currentUserRole);
                    MessageBox.Show("Task updated successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    result = await _taskService.CreateTaskAsync(request, _currentUserId, _currentUserRole);
                    MessageBox.Show("Task created successfully!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Permission Denied", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving task: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = _taskId.HasValue ? "Update" : "Create";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

