using System;
using System.Collections.Generic;

namespace ScheduleDomain.Model;

public partial class FinalSchedule
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }

    public int ClassroomId { get; set; }

    public int DayOfWeekId { get; set; }

    public int PairNumberId { get; set; }

    public bool? IsClassroomAssigned { get; set; }

    public virtual Classroom Classroom { get; set; } = null!;

    public virtual DaysOfWeek DayOfWeek { get; set; } = null!;

    public virtual PairNumber PairNumber { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;
}
