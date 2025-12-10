namespace Tomany.TaskManagement.BLL.Models;

public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? AccountId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
}

