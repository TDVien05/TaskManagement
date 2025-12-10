using System;
using System.Collections.Generic;

namespace Tomany.TaskManagement.DAL.Models;

public partial class ProjectMember
{
    public int ProjectMemberId { get; set; }

    public int ProjectId { get; set; }

    public int AccountId { get; set; }

    public string? RoleInProject { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
