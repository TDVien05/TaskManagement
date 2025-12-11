using System.Collections.Concurrent;

namespace Tomany.TaskManagement.BLL.Services;

public class OtpService
{
    private class OtpEntry
    {
        public string Code { get; init; } = string.Empty;
        public DateTime ExpireAt { get; init; }
    }

    private static readonly ConcurrentDictionary<string, OtpEntry> Store = new();
    private readonly TimeSpan _ttl;
    private readonly Random _random = new();
    private readonly EmailService? _emailService;

    public OtpService(EmailService? emailService = null, int? ttlMinutes = null)
    {
        _emailService = emailService;
        var minutes = ttlMinutes.HasValue && ttlMinutes.Value > 0 ? ttlMinutes.Value : 5;
        _ttl = TimeSpan.FromMinutes(minutes);
    }

    public bool EmailSendingEnabled => _emailService != null;

    public string GenerateOtp(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required to generate OTP.", nameof(email));
        }

        var emailKey = email.Trim().ToLowerInvariant();
        var code = _random.Next(0, 999999).ToString("D6");
        var entry = new OtpEntry
        {
            Code = code,
            ExpireAt = DateTime.UtcNow.Add(_ttl)
        };
        Store[emailKey] = entry;
        return code;
    }

    public async Task<string> GenerateAndSendOtpAsync(string email)
    {
        var code = GenerateOtp(email);
        
        if (_emailService != null)
        {
            try
            {
                await _emailService.SendOtpEmailAsync(email, code);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send OTP email: {ex.Message}", ex);
            }
        }
        
        return code;
    }

    public bool ValidateOtp(string email, string code)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var emailKey = email.Trim().ToLowerInvariant();

        if (!Store.TryGetValue(emailKey, out var entry))
        {
            return false;
        }

        if (DateTime.UtcNow > entry.ExpireAt)
        {
            Store.TryRemove(emailKey, out _);
            return false;
        }

        var isMatch = string.Equals(entry.Code, code?.Trim(), StringComparison.Ordinal);
        if (isMatch)
        {
            Store.TryRemove(emailKey, out _);
        }

        return isMatch;
    }
}

