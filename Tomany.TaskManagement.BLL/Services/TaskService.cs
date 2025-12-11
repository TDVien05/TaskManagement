using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Repositories;
using TaskModel = Tomany.TaskManagement.DAL.Models.Task;

namespace Tomany.TaskManagement.BLL.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<List<TaskDto>> GetTasksByProjectIdAsync(int projectId)
    {
        var tasks = await _taskRepository.GetTasksByProjectIdAsync(projectId);
        
        return tasks.Select(t => MapToDto(t)).ToList();
    }

    public List<TaskDto> FilterTasks(List<TaskDto> tasks, string? statusFilter, int? creatorFilter, DateOnly? deadlineFilter)
    {
        var filtered = tasks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
        {
            filtered = filtered.Where(t => t.TaskStatus?.Equals(statusFilter, StringComparison.OrdinalIgnoreCase) == true);
        }

        if (creatorFilter.HasValue)
        {
        }

        if (deadlineFilter.HasValue)
        {
            filtered = filtered.Where(t => t.DueDate.HasValue && t.DueDate.Value <= deadlineFilter.Value);
        }

        return filtered.ToList();
    }

    private TaskDto MapToDto(TaskModel task)
    {
        var assigneeName = task.AssignedToNavigation != null
            ? $"{task.AssignedToNavigation.Profile?.FirstName ?? ""} {task.AssignedToNavigation.Profile?.LastName ?? ""}".Trim()
            : "Unassigned";

        if (string.IsNullOrWhiteSpace(assigneeName))
        {
            assigneeName = task.AssignedToNavigation?.Username ?? "Unassigned";
        }

        var priority = CalculatePriority(task.DueDate);

        return new TaskDto
        {
            TaskId = task.TaskId,
            TaskName = task.TaskName,
            TaskDescription = task.TaskDescription,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.ProjectName,
            AssignedTo = task.AssignedTo,
            AssigneeName = assigneeName,
            TaskStatus = task.TaskStatus ?? "Pending",
            DueDate = task.DueDate,
            Priority = priority,
            CreateAt = task.CreateAt,
            UpdateAt = task.UpdateAt,
            LinkSubmission = task.LinkSubmission
        };
    }

    public async Task<TaskDto> CreateTaskAsync(TaskRequest request, int currentUserId, string? currentUserRole)
    {
        var isMember = await _projectRepository.IsUserProjectMemberAsync(request.ProjectId, currentUserId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this project.");
        }

        if (request.AssignedTo.HasValue && 
            !string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Only Managers and Admins can assign tasks.");
        }

        var task = new TaskModel
        {
            TaskName = request.TaskName,
            TaskDescription = request.TaskDescription,
            ProjectId = request.ProjectId,
            AssignedTo = request.AssignedTo,
            TaskStatus = request.TaskStatus ?? "Pending",
            DueDate = request.DueDate,
            LinkSubmission = request.LinkSubmission,
            CreateAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };

        var createdTask = await _taskRepository.CreateTaskAsync(task);
        
        var taskWithDetails = await _taskRepository.GetTaskByIdAsync(createdTask.TaskId);
        return MapToDto(taskWithDetails!);
    }

    public async Task<TaskDto> UpdateTaskAsync(int taskId, TaskRequest request, int currentUserId, string? currentUserRole)
    {
        var existingTask = await _taskRepository.GetTaskByIdAsync(taskId);
        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var isMember = await _projectRepository.IsUserProjectMemberAsync(request.ProjectId, currentUserId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this project.");
        }

        if (request.AssignedTo.HasValue && 
            !string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Only Managers and Admins can assign tasks.");
        }

        existingTask.TaskName = request.TaskName;
        existingTask.TaskDescription = request.TaskDescription;
        existingTask.ProjectId = request.ProjectId;
        existingTask.AssignedTo = request.AssignedTo;
        existingTask.TaskStatus = request.TaskStatus ?? existingTask.TaskStatus ?? "Pending";
        existingTask.DueDate = request.DueDate;
        existingTask.LinkSubmission = request.LinkSubmission;
        existingTask.UpdateAt = DateTime.Now;

        var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask);
        
        var taskWithDetails = await _taskRepository.GetTaskByIdAsync(updatedTask.TaskId);
        return MapToDto(taskWithDetails!);
    }

    public async Task<TaskDto> UpdateTaskStatusAsync(int taskId, string newStatus, int currentUserId, string? currentUserRole, int projectId)
    {
        var existingTask = await _taskRepository.GetTaskByIdAsync(taskId);
        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var isMember = await _projectRepository.IsUserProjectMemberAsync(projectId, currentUserId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this project.");
        }

        var currentStatus = existingTask.TaskStatus ?? "Pending";
        var isManagerOrAdmin = string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase);

        if (!isManagerOrAdmin)
        {
            if (currentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                if (!newStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Members can only change status from Pending to In Progress.");
                }
            }
            else if (currentStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
            {
                if (!newStatus.Equals("Review", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Members can only change status from In Progress to Review.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Members can only change status from Pending to In Progress, or from In Progress to Review.");
            }
        }

        await _taskRepository.UpdateTaskStatusAsync(taskId, newStatus);
        
        var taskWithDetails = await _taskRepository.GetTaskByIdAsync(taskId);
        return MapToDto(taskWithDetails!);
    }

    public async Task<TaskDto> ApproveTaskAsync(int taskId, int currentUserId, int projectId)
    {
        var existingTask = await _taskRepository.GetTaskByIdAsync(taskId);
        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        var currentStatus = existingTask.TaskStatus ?? "Pending";
        if (!currentStatus.Equals("Review", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only tasks with 'Review' status can be approved.");
        }

        var isManager = await _projectRepository.IsUserProjectManagerAsync(projectId, currentUserId);
        if (!isManager)
        {
            throw new UnauthorizedAccessException("Only the Project Manager can approve tasks.");
        }

        await _taskRepository.ApproveTaskAsync(taskId);
        
        var taskWithDetails = await _taskRepository.GetTaskByIdAsync(taskId);
        return MapToDto(taskWithDetails!);
    }

    private string CalculatePriority(DateOnly? dueDate)
    {
        if (!dueDate.HasValue)
        {
            return "Low";
        }

        var daysUntilDue = (dueDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

        if (daysUntilDue < 0)
        {
            return "Overdue";
        }
        else if (daysUntilDue <= 3)
        {
            return "High";
        }
        else if (daysUntilDue <= 7)
        {
            return "Medium";
        }
        else
        {
            return "Low";
        }
    }
}

