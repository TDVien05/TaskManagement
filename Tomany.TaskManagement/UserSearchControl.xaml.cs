using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.BLL.Services;

namespace Tomany.TaskManagement
{
    public partial class UserSearchControl : UserControl
    {
        private readonly IProjectService _projectService;
        private List<UserDto> _allUsers;

        // ObservableCollection for team members - automatically updates UI
        public ObservableCollection<UserDto> TeamMembers { get; private set; }

        public UserSearchControl()
        {
            InitializeComponent();

            _projectService = ServiceFactory.CreateProjectService();
            _allUsers = new List<UserDto>();
            TeamMembers = new ObservableCollection<UserDto>();

            // Bind ObservableCollection to ListBox
            TeamMembersListBox.ItemsSource = TeamMembers;

            // Subscribe to collection changes to update count
            TeamMembers.CollectionChanged += TeamMembers_CollectionChanged;

            UpdatePlaceholders();
        }

        private void TeamMembers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTeamMemberCount();
            UpdatePlaceholders();
        }

        private void UpdateTeamMemberCount()
        {
            TeamMemberCountText.Text = $"{TeamMembers.Count} member{(TeamMembers.Count != 1 ? "s" : "")}";
        }

        private void UpdatePlaceholders()
        {
            // Show/hide placeholder text
            SearchPlaceholderText.Visibility = SearchResultsListBox.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TeamPlaceholderText.Visibility = TeamMembers.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Auto-search as user types (debounced search could be added)
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchResultsListBox.ItemsSource = null;
                UpdatePlaceholders();
                return;
            }

            // Simple client-side filtering
            if (_allUsers.Count == 0)
            {
                await LoadAllUsers();
            }

            PerformSearch();
        }

        private async System.Threading.Tasks.Task LoadAllUsers()
        {
            try
            {
                // Load all users from service
                // Note: In a real application, you'd have a dedicated user service
                // For now, we'll use a placeholder that returns empty list
                _allUsers = new List<UserDto>();

                // TODO: Replace with actual user service when available
                // var users = await _userService.GetAllUsersAsync();
                // _allUsers = users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                MessageBox.Show("Please enter a search term.", "Search Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_allUsers.Count == 0)
            {
                await LoadAllUsers();
            }

            PerformSearch();
        }

        private void PerformSearch()
        {
            var searchTerm = SearchTextBox.Text.Trim().ToLower();

            var results = _allUsers.Where(u =>
                (u.FullName?.ToLower().Contains(searchTerm) ?? false) ||
                (u.Email?.ToLower().Contains(searchTerm) ?? false) ||
                (u.Username?.ToLower().Contains(searchTerm) ?? false)
            )
            .Where(u => !TeamMembers.Any(tm => tm.AccountId == u.AccountId)) // Exclude already added users
            .ToList();

            SearchResultsListBox.ItemsSource = results;
            UpdatePlaceholders();

            if (results.Count == 0 && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                MessageBox.Show("No users found matching your search.", "No Results",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is UserDto user)
            {
                // Check if user is already in team
                if (TeamMembers.Any(tm => tm.AccountId == user.AccountId))
                {
                    MessageBox.Show($"{user.FullName} is already in the team.", "Already Added",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Add to ObservableCollection - UI updates automatically
                TeamMembers.Add(user);

                MessageBox.Show($"{user.FullName} has been added to the team.", "User Added",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh search results to remove added user
                PerformSearch();
            }
        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is UserDto user)
            {
                var result = MessageBox.Show(
                    $"Remove {user.FullName} from the team?",
                    "Confirm Remove",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove from ObservableCollection - UI updates automatically
                    TeamMembers.Remove(user);

                    // Refresh search results
                    PerformSearch();
                }
            }
        }

        // Public method to get team member IDs for saving
        public List<int> GetTeamMemberIds()
        {
            return TeamMembers.Select(u => u.AccountId).ToList();
        }

        // Public method to load existing team members
        public async System.Threading.Tasks.Task LoadProjectMembers(int projectId)
        {
            try
            {
                var members = await _projectService.GetProjectMembersAsync(projectId);

                TeamMembers.Clear();
                foreach (var member in members)
                {
                    TeamMembers.Add(member);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project members: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Public method to clear all team members
        public void ClearTeamMembers()
        {
            TeamMembers.Clear();
        }
    }
}
