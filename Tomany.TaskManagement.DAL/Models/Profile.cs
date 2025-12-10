using System;
using System.Collections.Generic;

namespace Tomany.TaskManagement.DAL.Models;

public partial class Profile
{
    public int AccountId { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}
