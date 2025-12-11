using System.Threading.Tasks;
using Tomany.TaskManagement.DAL.Models;
using TaskModel = Tomany.TaskManagement.DAL.Models.Task;

namespace Tomany.TaskManagement.DAL.Repositories;

public interface ITaskRepository
{
    Task<List<TaskModel>> GetTasksByProjectIdAsync(int projectId);
    Task<TaskModel?> GetTaskByIdAsync(int taskId);
    Task<TaskModel> CreateTaskAsync(TaskModel task);
    Task<TaskModel> UpdateTaskAsync(TaskModel task);
    Task<TaskModel> UpdateTaskStatusAsync(int taskId, string status);
    Task<TaskModel> ApproveTaskAsync(int taskId);
}

