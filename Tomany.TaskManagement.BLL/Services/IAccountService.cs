using System.Collections.Generic;
using System.Threading.Tasks;
using Tomany.TaskManagement.BLL.Models;

namespace Tomany.TaskManagement.BLL.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<ProfileDto>> GetAllAsync();
        Task<IEnumerable<ProfileDto>> GetUsersByRoleAsync(string role);
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
        System.Threading.Tasks.Task ToggleAccountStatusAsync(int accountId);
        Task<string> ResetPasswordAsync(int accountId);
        System.Threading.Tasks.Task ApproveRequestAsync(int accountId);
        System.Threading.Tasks.Task RejectRequestAsync(int accountId);
        System.Threading.Tasks.Task<OperationResult> RequestManagerRoleAsync(int accountId);
    }
}
