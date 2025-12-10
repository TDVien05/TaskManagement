using Microsoft.EntityFrameworkCore;
using Tomany.TaskManagement.DAL.Models;

namespace Tomany.TaskManagement.DAL.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly TaskManagementContext _context;

    public AccountRepository(TaskManagementContext context)
    {
        _context = context;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Accounts.AnyAsync(a => a.Username == username);
    }

    public async Task<int> CreateAccountWithProfileAsync(Account account, Profile profile)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        profile.AccountId = account.AccountId;
        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        return account.AccountId;
    }

    public async Task<Account?> AuthenticateAsync(string username, string password)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
        return account;
    }
}

