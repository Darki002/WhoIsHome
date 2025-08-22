using WhoIsHome.External.Models;

namespace WhoIsHome.Aggregates.Mappers;

public static class SchoolScheduleMapper
{
    public static SchoolSchedule ToAggregate(this SchoolScheduleModel model)
    {
        var dinnerTime = new DinnerTime(model.PresenceType!.Value, model.DinnerTime);

        return new SchoolSchedule(
            model.Id,
            model.SchoolName,
            model.DayOfWeek,
            model.StartTime,
            model.EndTime,
            dinnerTime,
            model.UserId);
    }

    public static SchoolScheduleModel ToModel(this SchoolSchedule aggregate)
    {
        var model = new SchoolScheduleModel
        {
            SchoolName = aggregate.SchoolName,
            DayOfWeek = aggregate.DayOfWeek,
            StartTime = aggregate.StartTime,
            EndTime = aggregate.EndTime,
            PresenceType = aggregate.DinnerTime?.PresenceType,
            DinnerTime = aggregate.DinnerTime?.Time,
            UserId = aggregate.UserId
        };

        if (aggregate.Id.HasValue)
        {
            model.Id = aggregate.Id.Value;
        }

        return model;
    }
}