using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories
{
    public interface IProjectRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Project>> GetProjectsByAccountIdAsync(int accountId);
        System.Threading.Tasks.Task<Project?> GetProjectByIdAsync(int projectId); 
        System.Threading.Tasks.Task<IEnumerable<Project>> GetAllWithCreatorAsync();
        System.Threading.Tasks.Task<List<Project>> GetAllProjectsAsync();
        System.Threading.Tasks.Task<bool> IsUserProjectMemberAsync(int projectId, int accountId);
        System.Threading.Tasks.Task<List<Account>> GetProjectMembersAsync(int projectId);
        System.Threading.Tasks.Task<bool> IsUserProjectManagerAsync(int projectId, int accountId);
    }
}
