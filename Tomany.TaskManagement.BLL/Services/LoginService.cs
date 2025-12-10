using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services;

public class LoginService
{
    private readonly IAccountRepository _accountRepository;

    public LoginService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return Fail("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Fail("Password is required.");
        }

        try
        {
            var account = await _accountRepository.AuthenticateAsync(
                request.Username.Trim(), 
                request.Password);

            if (account == null)
            {
                return Fail("Invalid username or password.");
            }

            return Success("Login successful.", account.AccountId, account.Username, account.Role);
        }
        catch (Exception ex)
        {
            return Fail($"System error: {ex.Message}");
        }
    }

    private static LoginResult Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    private static LoginResult Success(string message, int accountId, string username, string? role) => new()
    {
        Success = true,
        Message = message,
        AccountId = accountId,
        Username = username,
        Role = role
    };
}

