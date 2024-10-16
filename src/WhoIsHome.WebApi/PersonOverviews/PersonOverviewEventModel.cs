using WhoIsHome.QueryHandler.PersonOverview;

namespace WhoIsHome.WebApi.PersonOverviews;

public record PersonOverviewEventModel
{
    public required int Id { get; init; }
    
    public required string Title { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public required TimeOnly StartTime { get; init; }
    
    public required TimeOnly EndTime { get; init; }
    
    public required string EventType { get; init; }

    public static PersonOverviewEventModel From(PersonOverviewEvent personOverviewEvent)
    {
        return new PersonOverviewEventModel
        {
            Id = personOverviewEvent.Id,
            Title = personOverviewEvent.Title,
            Date = personOverviewEvent.Date,
            StartTime = personOverviewEvent.StartTime,
            EndTime = personOverviewEvent.EndTime,
            EventType = personOverviewEvent.EventType.ToString()
        };
    }
}