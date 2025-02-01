using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class MaxPairsPerDay
{
    public int Id { get; set; }

    [Display(Name = "Max pairs per day")]
    public int MaxPairs { get; set; }

    public virtual ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();
}
