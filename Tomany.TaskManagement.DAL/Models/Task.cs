using System;
using System.Collections.Generic;

namespace Tomany.TaskManagement.DAL.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? TaskDescription { get; set; }

    public int ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    public string? TaskStatus { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? LinkSubmission { get; set; }

    public virtual Account? AssignedToNavigation { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}
