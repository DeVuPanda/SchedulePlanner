using ScheduleDomain.Model;
using System.Collections.Generic;

namespace ScheduleInfrastructure.Services
{
    public interface IFinalScheduleService
    {
        void AddSchedule(FinalSchedule schedule);
        void DeleteSchedule(int id);
        IEnumerable<FinalSchedule> GetSchedulesByTeacher(int teacherId);
        bool IsClassroomAvailable(int classroomId, int dayOfWeekId, int pairNumberId);
    }
}
