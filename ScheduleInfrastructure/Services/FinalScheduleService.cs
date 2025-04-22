using ScheduleDomain.Model;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleInfrastructure.Services
{
    public class FinalScheduleService : IFinalScheduleService
    {
        private readonly UniversityPracticeContext _context;

        public FinalScheduleService(UniversityPracticeContext context)
        {
            _context = context;
        }

        public void AddSchedule(FinalSchedule schedule)
        {
            _context.FinalSchedules.Add(schedule);
            _context.SaveChanges();
        }

        public void DeleteSchedule(int id)
        {
            var schedule = _context.FinalSchedules.FirstOrDefault(s => s.Id == id);
            if (schedule != null)
            {
                _context.FinalSchedules.Remove(schedule);
                _context.SaveChanges();
            }
        }

        public IEnumerable<FinalSchedule> GetSchedulesByTeacher(int teacherId)
        {
            return _context.FinalSchedules.Where(s => s.TeacherId == teacherId).ToList();
        }

        public bool IsClassroomAvailable(int classroomId, int dayOfWeekId, int pairNumberId)
        {
            return !_context.FinalSchedules.Any(s =>
                s.ClassroomId == classroomId &&
                s.DayOfWeekId == dayOfWeekId &&
                s.PairNumberId == pairNumberId);
        }
    }
}
