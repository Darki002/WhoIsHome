using WhoIsHome.Aggregates;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.WebApi.Models.Response;

public class OneTimeEventModelResponse
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required PresenceType PresenceType { get; set; }

    public TimeOnly? DinnerTime { get; set; } = null;

    public required UserModel User { get; set; }
    
    public static OneTimeEventModelResponse From(OneTimeEvent data, User user)
    {
        return new OneTimeEventModelResponse
        {
            Id = data.Id!.Value,
            Title = data.Title,
            Date = data.Date,
            StartTime = data.StartTime,
            EndTime = data.EndTime,
            PresenceType = data.DinnerTime.PresenceType,
            DinnerTime = data.DinnerTime.Time,
            User = UserModel.From(user)
        };
    }
}