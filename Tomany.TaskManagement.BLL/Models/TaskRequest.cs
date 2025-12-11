namespace Tomany.TaskManagement.BLL.Models;

public class TaskRequest
{
    public string TaskName { get; set; } = string.Empty;
    public string? TaskDescription { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedTo { get; set; }
    public string? TaskStatus { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? LinkSubmission { get; set; }
}

