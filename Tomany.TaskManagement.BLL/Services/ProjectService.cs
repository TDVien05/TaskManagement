using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async System.Threading.Tasks.Task<IEnumerable<Project>> GetProjectsByAccountIdAsync(int accountId)
        {
            return await _projectRepository.GetProjectsByAccountIdAsync(accountId);
        }

        public async System.Threading.Tasks.Task<Project?> GetProjectByIdAsync(int projectId)
        {
            return await _projectRepository.GetProjectByIdAsync(projectId);
        }
    }
}
