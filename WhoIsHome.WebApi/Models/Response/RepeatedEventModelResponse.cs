using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.Response;

public class RepeatedEventModelResponse
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly FirstOccurrence { get; set; }
    
    public required DateOnly LastOccurrence { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required PresenceType PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;

    public required UserModel User { get; set; }
    
    public static RepeatedEventModelResponse From(RepeatedEvent data, User user)
    {
        return new RepeatedEventModelResponse
        {
            Id = data.Id!.Value,
            Title = data.Title,
            FirstOccurrence = data.FirstOccurrence,
            LastOccurrence = data.LastOccurrence,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            PresenceType = data.DinnerTime.PresenceType,
            DinnerTime = data.DinnerTime.Time,
            User = UserModel.From(user)
        };
    }
}