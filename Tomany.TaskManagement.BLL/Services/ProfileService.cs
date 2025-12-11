using System.Text.RegularExpressions;
using Tomany.TaskManagement.BLL.Models;
using Tomany.TaskManagement.DAL.Repositories;

namespace Tomany.TaskManagement.BLL.Services;

public class ProfileService
{
    private readonly IAccountRepository _accountRepository;

    public ProfileService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<OperationResult<ProfileDto>> GetProfileAsync(int accountId)
    {
        var profile = await _accountRepository.GetProfileAsync(accountId);
        if (profile == null)
        {
            return Fail<ProfileDto>("Profile not found.");
        }

        var dto = new ProfileDto
        {
            AccountId = profile.AccountId,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            DateOfBirth = profile.DateOfBirth
        };

        return Success(dto, "Profile loaded.");
    }

    public async Task<OperationResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var validate = ValidateProfile(request);
        if (!validate.Success)
        {
            return validate;
        }

        var profile = await _accountRepository.GetProfileAsync(request.AccountId);
        if (profile == null)
        {
            return Fail("Profile not found.");
        }

        profile.FirstName = request.FirstName?.Trim();
        profile.LastName = request.LastName?.Trim();
        profile.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        profile.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        profile.DateOfBirth = request.DateOfBirth;

        await _accountRepository.UpdateProfileAsync(profile);
        return Success("Profile updated.");
    }

    public async Task<OperationResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            return Fail("Current password is required.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return Fail("New password must be at least 6 characters.");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return Fail("Password confirmation does not match.");
        }

        var match = await _accountRepository.CheckPasswordAsync(request.AccountId, request.CurrentPassword);
        if (!match)
        {
            return Fail("Current password is incorrect.");
        }

        var updated = await _accountRepository.UpdatePasswordAsync(request.AccountId, request.NewPassword);
        return updated ? Success("Password changed successfully.") : Fail("Unable to change password.");
    }

    private static OperationResult ValidateProfile(UpdateProfileRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Email) && !IsEmailLike(request.Email))
        {
            return Fail("Email format is invalid.");
        }

        return Success("Valid.");
    }

    private static bool IsEmailLike(string email)
    {
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    private static OperationResult Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    private static OperationResult<T> Fail<T>(string message) => new()
    {
        Success = false,
        Message = message
    };

    private static OperationResult Success(string message) => new()
    {
        Success = true,
        Message = message
    };

    private static OperationResult<T> Success<T>(T data, string message) => new()
    {
        Success = true,
        Message = message,
        Data = data
    };
}


