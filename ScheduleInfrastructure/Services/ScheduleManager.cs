using ScheduleDomain.Model;
using ScheduleInfrastructure.Services;

public class ScheduleManager
{
    private readonly IFinalScheduleService _scheduleService;

    public ScheduleManager(IFinalScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public void AddScheduleIfAvailable(FinalSchedule schedule)
    {
        if (_scheduleService.IsClassroomAvailable((int)schedule.ClassroomId, schedule.DayOfWeekId, schedule.PairNumberId))
        {
            _scheduleService.AddSchedule(schedule);
        }
        else
        {
            throw new InvalidOperationException("Classroom is not available.");
        }
    }

    public void RemoveSchedulesInOrder(List<int> scheduleIds)
    {
        foreach (var id in scheduleIds)
        {
            _scheduleService.DeleteSchedule(id);
        }
    }

    public bool TryAddSchedule(FinalSchedule schedule)
    {
        try
        {
            _scheduleService.AddSchedule(schedule);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
