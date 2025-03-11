using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.Response;

public class RepeatedEventModel
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly FirstOccurrence { get; set; }
    
    public required DateOnly LastOccurrence { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;

    public required int UserId { get; set; }
    
    public static RepeatedEventModel From(RepeatedEvent data)
    {
        return new RepeatedEventModel
        {
            Id = data.Id!.Value,
            Title = data.Title,
            FirstOccurrence = data.FirstOccurrence,
            LastOccurrence = data.LastOccurrence,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            PresenceType = data.DinnerTime.PresenceType.ToString(),
            DinnerTime = data.DinnerTime.Time,
            UserId = data.UserId
        };
    }
}