using WhoIsHome.QueryHandler.UserOverview;

namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

public record UserOverviewEventModel
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public required DateOnly Date { get; init; }

    public required TimeOnly StartTime { get; init; }

    public TimeOnly? EndTime { get; init; }
    
    public bool HasRepetitions { get; set; }

    public required int TemplateId { get; init; }

    public static UserOverviewEventModel From(UserOverviewEvent userOverviewEvent)
    {
        return new UserOverviewEventModel
        {
            Id = userOverviewEvent.Id,
            Title = userOverviewEvent.Title,
            Date = userOverviewEvent.NextDate,
            StartTime = userOverviewEvent.StartTime,
            EndTime = userOverviewEvent.EndTime,
            TemplateId = userOverviewEvent.TemplateId,
            HasRepetitions = userOverviewEvent.HasRepetitions
        };
    }
}