using System;
using System.Collections.Generic;

namespace ScheduleDomain.Model;

public partial class PairNumber
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();

    public virtual ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();
}
