using System;
using System.Collections.Generic;

namespace Tomany.TaskManagement.DAL.Models;

public partial class UserActivityLog
{
    public int LogId { get; set; }

    public int AccountId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}
