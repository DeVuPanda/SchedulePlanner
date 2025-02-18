using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleDomain.Model
{
    public partial class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = null!;
        public string Course { get; set; } = null!;
        public string Speciality { get; set; } = null!;

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<SchedulePreference> SchedulePreferences { get; set; } = new List<SchedulePreference>();
        public ICollection<FinalSchedule> FinalSchedules { get; set; } = new List<FinalSchedule>();
    }
}
