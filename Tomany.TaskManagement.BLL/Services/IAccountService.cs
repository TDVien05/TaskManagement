using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;

namespace Tomany.TaskManagement.BLL.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<ProfileDto>> GetAllAsync();
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
        System.Threading.Tasks.Task ToggleAccountStatusAsync(int accountId);
    }
}
