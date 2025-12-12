using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories
{
    public interface IProjectRepository
    {
        System.Threading.Tasks.Task<IEnumerable<Project>> GetProjectsByAccountIdAsync(int accountId);
        System.Threading.Tasks.Task<Project?> GetProjectByIdAsync(int projectId); 
    }
}
