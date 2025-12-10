using System;
using System.Collections.Generic;

namespace Tomany.TaskManagement.DAL.Models;

public partial class TaskHistory
{
    public int HistoryId { get; set; }

    public int TaskId { get; set; }

    public int UpdatedBy { get; set; }

    public int? ProgressPercent { get; set; }

    public string? Note { get; set; }

    public string? StatusChange { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual Account UpdatedByNavigation { get; set; } = null!;
}
