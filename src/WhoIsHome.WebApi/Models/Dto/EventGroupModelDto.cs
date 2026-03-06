using WhoIsHome.Entities;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.Dto;

public class EventGroupModelDto
{
    public string Title { get; set; } = null!;
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    public List<DayOfWeek> WeekDays { get; set; } = [];

    public string PresenceType { get; set; } = null!;

    public TimeOnly? DinnerTime { get; set; } = null;
    
    
    public static EventGroupModelDto FromEntity(EventGroup eventGroup)
    {
        return new EventGroupModelDto
        {
            Title = eventGroup.Title,
            StartDate = eventGroup.StartDate,
            EndDate = eventGroup.EndDate,
            WeekDays = eventGroup.WeekDays.ToDayOfWeekList(),
            StartTime = eventGroup.StartTime,
            EndTime = eventGroup.EndTime,
            PresenceType = eventGroup.PresenceType.ToEnumString(),
            DinnerTime = eventGroup.DinnerTime
        };
    }

    public void ApplyUpdate(EventGroup eventGroup)
    {
        eventGroup.Title = Title;
        eventGroup.StartDate = StartDate;
        eventGroup.EndDate = EndDate;
        eventGroup.WeekDays = WeekDays.ToWeekDays();
        eventGroup.StartTime = StartTime;
        eventGroup.EndTime = EndTime;
        eventGroup.PresenceType = PresenceTypeHelper.FromString(PresenceType);
        eventGroup.DinnerTime = DinnerTime;
    }
}