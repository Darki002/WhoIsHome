using WhoIsHome.QueryHandler.UserOverview;

namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

public record UserOverviewEventModel
{
    public required int GroupId { get; init; }

    public required string Title { get; init; }

    public required DateOnly Date { get; init; }

    public required TimeOnly StartTime { get; init; }

    public TimeOnly? EndTime { get; init; }
    
    public bool HasRepetitions { get; set; }

    public static UserOverviewEventModel From(UserOverviewEvent userOverviewEvent)
    {
        return new UserOverviewEventModel
        {
            GroupId = userOverviewEvent.GroupId,
            Title = userOverviewEvent.Title,
            Date = userOverviewEvent.NextDate,
            StartTime = userOverviewEvent.StartTime,
            EndTime = userOverviewEvent.EndTime,
            HasRepetitions = userOverviewEvent.HasRepetitions
        };
    }
}