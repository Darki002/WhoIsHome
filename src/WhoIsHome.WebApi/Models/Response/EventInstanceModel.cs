using WhoIsHome.Entities;

namespace WhoIsHome.WebApi.Models.Response;

public class EventInstanceModel
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }

    public required TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;
    
    public required int EventGroupId { get; set; }

    public required int UserId { get; set; }
    
    public static EventInstanceModel From(EventInstance data)
    {
        return new EventInstanceModel
        {
            Id = data.Id,
            Title = data.Title,
            Date = data.Date,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            PresenceType = data.PresenceType.ToString(),
            DinnerTime = data.DinnerTime,
            EventGroupId = data.EventGroupId,
            UserId = data.UserId
        };
    }
}