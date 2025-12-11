using System.Collections.Generic;
using System.Threading.Tasks;
using Account = Tomany.TaskManagement.DAL.Models.Account;
using Profile = Tomany.TaskManagement.DAL.Models.Profile;

namespace Tomany.TaskManagement.DAL.Repositories;

public interface IAccountRepository
{
    Task<bool> UsernameExistsAsync(string username);
    Task<int> CreateAccountWithProfileAsync(Account account, Profile profile);
    Task<Account?> AuthenticateAsync(string username, string password);
    Task<Profile?> GetProfileAsync(int accountId);
    Task UpdateProfileAsync(Profile profile);
    Task<bool> CheckPasswordAsync(int accountId, string currentPassword);
    Task<bool> UpdatePasswordAsync(int accountId, string newPassword);
    Task<IEnumerable<Account>> GetAllWithProfilesAsync();
}

