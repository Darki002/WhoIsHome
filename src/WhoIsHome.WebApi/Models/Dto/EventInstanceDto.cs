using WhoIsHome.Entities;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.WebApi.Models.Dto;

public class EventInstanceDto
{
    public DateOnly Date { get; set; }
    
    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }
    
    public string PresenceType { get; set; } = null!;

    public TimeOnly? DinnerTime { get; set; }

    public static EventInstanceDto FromEntity(EventInstance eventInstance)
    {
        return new EventInstanceDto
        {
            Date = eventInstance.Date,
            StartTime = eventInstance.StartTime,
            EndTime = eventInstance.EndTime,
            PresenceType = eventInstance.PresenceType.ToEnumString(), 
            DinnerTime = eventInstance.DinnerTime
        };
    }

    public void ApplyUpdate(EventInstance entity)
    {
        entity.Date = Date;
        entity.StartTime = StartTime;
        entity.EndTime = EndTime;
        entity.PresenceType = PresenceTypeHelper.FromString(PresenceType);
        entity.DinnerTime = DinnerTime;
    }
}