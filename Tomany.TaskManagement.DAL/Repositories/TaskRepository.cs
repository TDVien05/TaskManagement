using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tomany.TaskManagement.DAL.Models;
using TaskModel = Tomany.TaskManagement.DAL.Models.Task;

namespace Tomany.TaskManagement.DAL.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementContext _context;

    public TaskRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task<List<TaskModel>> GetTasksByProjectIdAsync(int projectId)
    {
        return await _context.Tasks
            .Include(t => t.AssignedToNavigation)
            .ThenInclude(a => a!.Profile)
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<TaskModel?> GetTaskByIdAsync(int taskId)
    {
        return await _context.Tasks
            .Include(t => t.AssignedToNavigation)
            .ThenInclude(a => a!.Profile)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.TaskId == taskId);
    }

    public async Task<TaskModel> CreateTaskAsync(TaskModel task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskModel> UpdateTaskAsync(TaskModel task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskModel> UpdateTaskStatusAsync(int taskId, string status)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        task.TaskStatus = status;
        task.UpdateAt = DateTime.Now;
        await _context.SaveChangesAsync();
        
        return await GetTaskByIdAsync(taskId) ?? task;
    }

    public async Task<TaskModel> ApproveTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found.");
        }

        task.TaskStatus = "Completed";
        task.UpdateAt = DateTime.Now;
        await _context.SaveChangesAsync();
        
        return await GetTaskByIdAsync(taskId) ?? task;
    }
}

