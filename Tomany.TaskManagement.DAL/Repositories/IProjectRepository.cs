using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(int projectId);
    Task<bool> IsUserProjectMemberAsync(int projectId, int accountId);
    Task<List<Account>> GetProjectMembersAsync(int projectId);
    Task<bool> IsUserProjectManagerAsync(int projectId, int accountId);
}

