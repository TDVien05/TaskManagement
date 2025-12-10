namespace Tomany.TaskManagement.BLL.Models;

public class RegisterResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? AccountId { get; set; }
}

