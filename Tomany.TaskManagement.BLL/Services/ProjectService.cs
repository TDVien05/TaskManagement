using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var projects = await _projectRepository.GetAllWithCreatorAsync();
            return projects.Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectDescription = p.ProjectDescription,
                ProjectStatus = p.ProjectStatus,
                CreatedByUsername = p.CreateByNavigation.Username,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt
            }).ToList();
        }
    }
}
