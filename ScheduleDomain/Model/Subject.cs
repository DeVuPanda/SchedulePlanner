using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ScheduleDomain.Model;
public partial class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [Display(Name = "Teacher")]
    public int TeacherId { get; set; }
    public int Hours { get; set; }
    [Display(Name = "Group")]
    public int GroupId { get; set; }
    [Display(Name = "Education program")]
    public string? EducationProgram { get; set; }
    public virtual ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();
    public virtual ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();
    public virtual User Teacher { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}