using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class SchedulePreference
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    [Display(Name = "Subject")]
    public int SubjectId { get; set; }

    public int GroupId { get; set; }

    [Display(Name = "Day of week")]
    public int DayOfWeekId { get; set; }

    [Display(Name = "Pair number")]
    public int PairNumberId { get; set; }

    [Display(Name = "Max pairs per day")]
    public int MaxPairsPerDayId { get; set; }

    public int Priority { get; set; }

    [Display(Name = "Day of week")]
    public virtual DaysOfWeek DayOfWeek { get; set; } = null!;

    [Display(Name = "Max pairs per day")]
    public virtual MaxPairsPerDay MaxPairsPerDay { get; set; } = null!;

    [Display(Name = "Pair number")]
    public virtual PairNumber PairNumber { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
