using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Repositories;
using ProjectModel = Tomany.TaskManagement.DAL.Models.Project;

namespace Tomany.TaskManagement.BLL.Services;

public class ProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<List<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllProjectsAsync();
        
        return projects.Select(p => MapToDto(p)).ToList();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int projectId)
    {
        var project = await _projectRepository.GetProjectByIdAsync(projectId);
        
        return project != null ? MapToDto(project) : null;
    }

    public async Task<List<UserDto>> GetProjectMembersAsync(int projectId)
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

    public async Task<bool> IsUserProjectManagerAsync(int projectId, int accountId)
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

