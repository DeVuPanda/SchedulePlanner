using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class User
{
    public int Id { get; set; }

    [Display(Name = "Full name")]
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();

    public virtual UserRole Role { get; set; } = null!;

    public virtual ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
