using System;
using System.Collections.Generic;

namespace ScheduleDomain.Model;

public partial class SchedulePreference
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public int DayOfWeekId { get; set; }

    public int PairNumberId { get; set; }

    public int MaxPairsPerDayId { get; set; }

    public int Priority { get; set; }

    public virtual DaysOfWeek DayOfWeek { get; set; } = null!;

    public virtual MaxPairsPerDay MaxPairsPerDay { get; set; } = null!;

    public virtual PairNumber PairNumber { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;
}
