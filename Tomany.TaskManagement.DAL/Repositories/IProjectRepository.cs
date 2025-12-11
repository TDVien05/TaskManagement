using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllWithCreatorAsync();
    }
}
