using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models; 
using Tomany.TaskManagement.DAL.Repositories;
using ProjectModel = Tomany.TaskManagement.DAL.Models.Project; 
using Tomany.TaskManagement.DAL.Models; 

namespace Tomany.TaskManagement.BLL.Services
{
    public class ProjectService : IProjectService 
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async System.Threading.Tasks.Task<IEnumerable<ProjectDto>> GetAllAsync()
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

        public async System.Threading.Tasks.Task<IEnumerable<Project>> GetProjectsByAccountIdAsync(int accountId)
        {
            return await _projectRepository.GetProjectsByAccountIdAsync(accountId);
        }

        public async System.Threading.Tasks.Task<Project?> GetProjectByIdAsync(int projectId)
        {
            return await _projectRepository.GetProjectByIdAsync(projectId);
        }
        
        public async System.Threading.Tasks.Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllProjectsAsync();
            
            return projects.Select(p => MapToDto(p)).ToList();
        }

        public async System.Threading.Tasks.Task<ProjectDto?> GetProjectDtoByIdAsync(int projectId) 
        {
            var project = await _projectRepository.GetProjectByIdAsync(projectId); 
            return project != null ? MapToDto(project) : null;
        }

        public async System.Threading.Tasks.Task<List<UserDto>> GetProjectMembersAsync(int projectId)
        {
            var members = await _projectRepository.GetProjectMembersAsync(projectId);
            
            return members.Select(m => new UserDto
            {
                AccountId = m.AccountId,
                Username = m.Username,
                FullName = $"{m.Profile?.FirstName ?? ""} {m.Profile?.LastName ?? ""}".Trim(),
                Email = m.Profile?.Email
            }).ToList();
        }

        public async System.Threading.Tasks.Task<bool> IsUserProjectManagerAsync(int projectId, int accountId)
        {
            return await _projectRepository.IsUserProjectManagerAsync(projectId, accountId);
        }

        private ProjectDto MapToDto(ProjectModel project)
        {
            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                ProjectDescription = project.ProjectDescription,
                ProjectStatus = project.ProjectStatus,
                CreateBy = project.CreateBy,
                CreateAt = project.CreateAt,
                UpdateAt = project.UpdateAt
            };
        }
    }
}
