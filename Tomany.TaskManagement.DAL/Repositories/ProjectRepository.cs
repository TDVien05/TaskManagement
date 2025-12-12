using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskManagementContext _context;

        public ProjectRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllWithCreatorAsync()
        {
            return await _context.Projects
                .Include(p => p.CreateByNavigation)
                .ToListAsync();
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .OrderBy(p => p.ProjectName)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int projectId)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public async Task<bool> IsUserProjectMemberAsync(int projectId, int accountId)
        {
            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId && pm.AccountId == accountId);
        }

        public async Task<List<Account>> GetProjectMembersAsync(int projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.Account)
                .ThenInclude(a => a.Profile)
                .Select(pm => pm.Account)
                .ToListAsync();
        }

        public async Task<bool> IsUserProjectManagerAsync(int projectId, int accountId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
            
            if (project != null && project.CreateBy == accountId)
            {
                return true;
            }

            return await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == projectId 
                            && pm.AccountId == accountId 
                            && pm.RoleInProject != null 
                            && pm.RoleInProject.Equals("Manager", StringComparison.OrdinalIgnoreCase));
        }
    }
}
