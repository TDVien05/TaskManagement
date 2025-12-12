namespace Tomany.TaskManagement.BLL.Models;

public class UserDto
{
    public int AccountId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Email { get; set; }
}

