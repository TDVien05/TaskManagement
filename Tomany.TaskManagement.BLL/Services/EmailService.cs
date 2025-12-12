using System.Net;
using System.Net.Mail;
using System.Text;

namespace Tomany.TaskManagement.BLL.Services;

public class EmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(SmtpSettings? settings = null)
    {
        _settings = settings ?? new SmtpSettings();
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        var subject = "Email Verification - OTP Code";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0f7f3c; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f7fff9; padding: 30px; border: 1px solid #d9f3e4; border-radius: 0 0 8px 8px; }}
        .otp-box {{ background-color: white; border: 2px solid #0f7f3c; border-radius: 6px; padding: 15px; text-align: center; margin: 20px 0; }}
        .otp-code {{ font-size: 32px; font-weight: bold; color: #0f7f3c; letter-spacing: 5px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #6b7280; font-size: 12px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>Task Management - Email Verification</h2>
        </div>
        <div class=""content"">
            <p>Hello,</p>
            <p>Thank you for registering with Task Management. Please use the OTP code below to verify your email address:</p>
            <div class=""otp-box"">
                <div class=""otp-code"">{otpCode}</div>
            </div>
            <p>This code will expire in 5 minutes.</p>
            <p>If you didn't request this code, please ignore this email.</p>
            <div class=""footer"">
                <p>This is an automated message. Please do not reply.</p>
            </div>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            throw new InvalidOperationException("SMTP sender email (FromEmail) is required.");
        }

        // Remove all whitespace from password (Gmail App Passwords may have spaces when copied)
        var originalPassword = _settings.Password ?? string.Empty;
        var cleanPassword = originalPassword.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();
        
        if (string.IsNullOrEmpty(cleanPassword))
        {
            throw new InvalidOperationException("SMTP password cannot be empty.");
        }

        if (cleanPassword.Length != 16)
        {
            throw new InvalidOperationException(
                $"App Password must be exactly 16 characters. Current length: {cleanPassword.Length}. " +
                $"Please check your App Password in Google Account settings.");
        }

        var primaryPort = _settings.Port > 0 ? _settings.Port : 587;
        var fallbackPort = primaryPort == 465 ? 587 : 465;

        // Try configured port first, then fallback (587/465) if it fails
        Exception? lastException = null;
        
        // Primary attempt
        try
        {
            using var client = new SmtpClient(_settings.Host, primaryPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, cleanPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 30000
            };

            using var message = new MailMessage(_settings.FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };
            
            await client.SendMailAsync(message);
            return;
        }
        catch (SmtpException smtpEx)
        {
            lastException = smtpEx;
            if (smtpEx.Message.Contains("5.7.0", StringComparison.OrdinalIgnoreCase) ||
                smtpEx.Message.Contains("Authentication", StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                throw new InvalidOperationException(
                    $"SMTP Error: {smtpEx.Message}. Status Code: {smtpEx.StatusCode}. " +
                    $"Please verify your App Password is correct.", smtpEx);
            }
        }
        catch (Exception ex)
        {
            lastException = ex;
            if (!ex.Message.Contains("Authentication", StringComparison.OrdinalIgnoreCase) &&
                !ex.Message.Contains("5.7.0", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }

        // Fallback attempt (alternate SSL port)
        try
        {
            using var client = new SmtpClient(_settings.Host, fallbackPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.Username, cleanPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Timeout = 30000
            };

            using var message = new MailMessage(_settings.FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };
            
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            var errorDetails = new StringBuilder();
            errorDetails.AppendLine($"Failed to send email after trying ports {primaryPort} and {fallbackPort}.");
            errorDetails.AppendLine($"Last error: {lastException?.Message ?? ex.Message}");
            errorDetails.AppendLine();
            errorDetails.AppendLine("Troubleshooting steps:");
            errorDetails.AppendLine("1. Verify App Password is exactly 16 characters (no spaces)");
            errorDetails.AppendLine("2. Make sure 2-Step Verification is enabled in Google Account");
            errorDetails.AppendLine("3. Create a NEW App Password:");
            errorDetails.AppendLine("   - Go to: https://myaccount.google.com/apppasswords");
            errorDetails.AppendLine("   - Select 'Mail' and 'Windows Computer'");
            errorDetails.AppendLine("   - Copy the 16-character password (without spaces)");
            errorDetails.AppendLine("4. Ensure username matches the email address exactly");
            
            throw new InvalidOperationException(errorDetails.ToString(), ex);
        }
    }
}

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;

    public SmtpSettings()
    {
    }

    public SmtpSettings(string username, string password, string fromEmail)
    {
        Username = username;
        Password = password;
        FromEmail = fromEmail;
    }
}

