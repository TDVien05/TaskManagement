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

    public async System.Threading.Tasks.Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Accounts.AnyAsync(a => a.Username == username);
    }

    public async System.Threading.Tasks.Task<int> CreateAccountWithProfileAsync(Account account, Profile profile)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        profile.AccountId = account.AccountId;
        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        return account.AccountId;
    }

    public async System.Threading.Tasks.Task<Account?> AuthenticateAsync(string username, string password)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
        return account;
    }

    public async System.Threading.Tasks.Task<Profile?> GetProfileAsync(int accountId)
    {
        return await _context.Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.AccountId == accountId);
    }

    public async System.Threading.Tasks.Task UpdateProfileAsync(Profile profile)
    {
        _context.Profiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<bool> CheckPasswordAsync(int accountId, string currentPassword)
    {
        return await _context.Accounts.AnyAsync(a =>
            a.AccountId == accountId && a.Password == currentPassword);
    }

    public async System.Threading.Tasks.Task<bool> UpdatePasswordAsync(int accountId, string newPassword)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (account == null)
        {
            return false;
        }

        account.Password = newPassword;
        await _context.SaveChangesAsync();
        return true;
    }
}

