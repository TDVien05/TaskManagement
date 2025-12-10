namespace Tomany.TaskManagement.BLL.Models;

public class ProfileDto
{
    public int AccountId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}


