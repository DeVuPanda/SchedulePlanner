using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDomain.Model;

public partial class Classroom
{
    public int Id { get; set; }

    [Display(Name= "Room Number")]
    public string? RoomNumber { get; set; }

    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();
}
