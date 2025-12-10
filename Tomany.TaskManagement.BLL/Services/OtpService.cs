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
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);
    private readonly Random _random = new();
    private readonly EmailService? _emailService;

    public OtpService(EmailService? emailService = null)
    {
        _emailService = emailService;
    }

    public string GenerateOtp(string email)
    {
        var code = _random.Next(0, 999999).ToString("D6");
        var entry = new OtpEntry
        {
            Code = code,
            ExpireAt = DateTime.UtcNow.Add(_ttl)
        };
        Store[email] = entry;
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
        if (!Store.TryGetValue(email, out var entry))
        {
            return false;
        }

        if (DateTime.UtcNow > entry.ExpireAt)
        {
            Store.TryRemove(email, out _);
            return false;
        }

        var isMatch = string.Equals(entry.Code, code?.Trim(), StringComparison.Ordinal);
        if (isMatch)
        {
            Store.TryRemove(email, out _);
        }

        return isMatch;
    }
}

