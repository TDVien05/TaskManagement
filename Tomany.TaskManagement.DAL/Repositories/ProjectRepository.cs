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

        public async System.Threading.Tasks.Task<IEnumerable<Project>> GetProjectsByAccountIdAsync(int accountId)
        {
            // First, find all ProjectMember entries for the given accountId
            var projectMembers = await _context.ProjectMembers
                                            .Where(pm => pm.AccountId == accountId)
                                            .ToListAsync();

            // Extract ProjectIds from the projectMembers
            var projectIds = projectMembers.Select(pm => pm.ProjectId).Distinct();

            // Now, get the projects based on these ProjectIds
            var projects = await _context.Projects
                                        .Where(p => projectIds.Contains(p.ProjectId))
                                        .ToListAsync();

            return projects;
        }

        public async System.Threading.Tasks.Task<Project?> GetProjectByIdAsync(int projectId)
        {
            return await _context.Projects
                                 .Include(p => p.Tasks)
                                 .Include(p => p.ProjectMembers)
                                     .ThenInclude(pm => pm.Account) // Include Account details for each member
                                 .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }
    }
}
