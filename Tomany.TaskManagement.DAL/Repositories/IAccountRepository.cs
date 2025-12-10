using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories;

public interface IAccountRepository
{
    System.Threading.Tasks.Task<bool> UsernameExistsAsync(string username);
    System.Threading.Tasks.Task<int> CreateAccountWithProfileAsync(Account account, Profile profile);
    System.Threading.Tasks.Task<Account?> AuthenticateAsync(string username, string password);
    System.Threading.Tasks.Task<Profile?> GetProfileAsync(int accountId);
    System.Threading.Tasks.Task UpdateProfileAsync(Profile profile);
    System.Threading.Tasks.Task<bool> CheckPasswordAsync(int accountId, string currentPassword);
    System.Threading.Tasks.Task<bool> UpdatePasswordAsync(int accountId, string newPassword);
}

