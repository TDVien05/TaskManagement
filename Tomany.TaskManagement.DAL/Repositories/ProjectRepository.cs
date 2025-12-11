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
    }
}
