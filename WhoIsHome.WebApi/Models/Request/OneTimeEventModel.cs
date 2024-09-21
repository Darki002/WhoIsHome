using WhoIsHome.Aggregates;

namespace WhoIsHome.WebApi.Models.Request;

public class OneTimeEventModel
{
    public required int Id { get; set; }

    public required string Title { get; set; }
    
    public required DateOnly Date { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required TimeOnly EndTime { get; set; }

    public required DinnerTime DinnerTime { get; set; }
}