namespace Tomany.TaskManagement.BLL.Models;

public class TaskDto
{
    public int TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? TaskDescription { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? AssignedTo { get; set; }
    public string? AssigneeName { get; set; }
    public string? TaskStatus { get; set; }
    public DateOnly? DueDate { get; set; }
    public string FormattedDueDate => DueDate.HasValue ? DueDate.Value.ToString("MM/dd/yyyy") : "No deadline";
    public string? Priority { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public string? LinkSubmission { get; set; }
    
    public List<string> GetAllowedStatuses(string? userRole)
    {
        var isManagerOrAdmin = string.Equals(userRole, "Manager", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);
        
        if (isManagerOrAdmin)
        {
            return new List<string> { "Pending", "In Progress", "Review", "Completed" };
        }
        
        var currentStatus = TaskStatus ?? "Pending";
        if (currentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            return new List<string> { "Pending", "In Progress" };
        }
        else if (currentStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase))
        {
            return new List<string> { "In Progress", "Review" };
        }
        else
        {
            return new List<string> { currentStatus };
        }
    }
}

