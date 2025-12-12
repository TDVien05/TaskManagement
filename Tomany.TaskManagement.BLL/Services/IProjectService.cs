using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;

namespace Tomany.TaskManagement.BLL.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
    }
}
