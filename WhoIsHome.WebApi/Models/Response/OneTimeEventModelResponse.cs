using WhoIsHome.Aggregates;

namespace WhoIsHome.WebApi.Models.Response;

public class OneTimeEventModelResponse
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required DinnerTime DinnerTime { get; set; }

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
            DinnerTime = data.DinnerTime,
            User = UserModel.From(user)
        };
    }
}