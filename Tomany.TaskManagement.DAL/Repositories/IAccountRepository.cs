using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories;

public interface IAccountRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task<int> CreateAccountWithProfileAsync(Account account, Profile profile);
    Task<Account?> AuthenticateAsync(string username, string password);
}

