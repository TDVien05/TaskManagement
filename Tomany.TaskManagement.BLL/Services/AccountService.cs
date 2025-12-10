using System.Text.RegularExpressions;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public RegisterResult ValidateRequest(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return Fail("Username is required.");
        }

        if (request.Username.Trim().Length < 4)
        {
            return Fail("Username must be at least 4 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Fail("Password is required.");
        }

        if (request.Password.Length < 6)
        {
            return Fail("Password must be at least 6 characters.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return Fail("Password confirmation does not match.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Fail("Email is required.");
        }

        if (!IsEmailLike(request.Email))
        {
            return Fail("Email format is invalid.");
        }

        return Success("Validated.");
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        var validation = ValidateRequest(request);
        if (!validation.Success)
        {
            return validation;
        }

        try
        {
            var normalizedUsername = request.Username.Trim();
            var exists = await _accountRepository.UsernameExistsAsync(normalizedUsername);
            if (exists)
            {
                return Fail("Username already exists.");
            }

            var account = new Account
            {
                Username = normalizedUsername,
                Password = request.Password,
                Role = "User"
            };

            var profile = new Profile
            {
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                DateOfBirth = request.DateOfBirth.HasValue
                    ? DateOnly.FromDateTime(request.DateOfBirth.Value)
                    : null
            };

            var accountId = await _accountRepository.CreateAccountWithProfileAsync(account, profile);

            return Success("Registration succeeded.", accountId);
        }
        catch (Exception ex)
        {
            return Fail($"System error: {ex.Message}");
        }
    }

    private static bool IsEmailLike(string email)
    {
        // Simple but stricter than Contains: letters/numbers + @ + domain
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    private static RegisterResult Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    private static RegisterResult Success(string message, int? accountId = null) => new()
    {
        Success = true,
        Message = message,
        AccountId = accountId
    };
}

