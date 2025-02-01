using System;
using System.Collections.Generic;

namespace ScheduleDomain.Model;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TeacherId { get; set; }

    public int Hours { get; set; }

    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();

    public virtual ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();

    public virtual User Teacher { get; set; } = null!;
}
