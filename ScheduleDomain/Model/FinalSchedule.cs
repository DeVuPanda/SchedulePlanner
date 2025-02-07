﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class FinalSchedule
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    [Display(Name = "Classroom")]
    public int? ClassroomId { get; set; }

    [Display(Name = "Day of week")]
    public int DayOfWeekId { get; set; }

    [Display(Name = "Pair number")]
    public int PairNumberId { get; set; }

    [Display(Name = "Is classroom assigned")]
    public bool? IsClassroomAssigned { get; set; }

    public virtual Classroom? Classroom { get; set; } = null!;

    [Display(Name = "Day of week")]
    public virtual DaysOfWeek DayOfWeek { get; set; } = null!;

    [Display(Name = "Pair number")]
    public virtual PairNumber PairNumber { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;
}
