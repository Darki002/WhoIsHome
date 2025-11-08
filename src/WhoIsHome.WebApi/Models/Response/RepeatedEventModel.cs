using WhoIsHome.Entities;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.Response;

public class EventGroupModel
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }

    public required TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    public required List<DayOfWeek> WeekDays { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;

    public required int UserId { get; set; }
    
    public static EventGroupModel From(EventGroup data)
    {
        return new EventGroupModel
        {
            Id = data.Id,
            Title = data.Title,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            WeekDays = data.WeekDays.ToDayOfWeekList(),
            PresenceType = data.PresenceType.ToString(),
            DinnerTime = data.DinnerTime,
            UserId = data.UserId
        };
    }
}