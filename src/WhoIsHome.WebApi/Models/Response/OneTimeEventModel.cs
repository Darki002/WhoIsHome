using WhoIsHome.Aggregates;

namespace WhoIsHome.WebApi.Models.Response;

public class OneTimeEventModel
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required string PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;

    public required int UserId { get; set; }
    
    public static OneTimeEventModel From(OneTimeEvent data, User user)
    {
        return new OneTimeEventModel
        {
            Id = data.Id!.Value,
            Title = data.Title,
            Date = data.Date,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            PresenceType = data.DinnerTime.PresenceType.ToString(),
            DinnerTime = data.DinnerTime.Time,
            UserId = user.Id!.Value
        };
    }
}