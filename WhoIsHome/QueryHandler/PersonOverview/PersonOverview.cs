using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverview
{
    public required User User { get; init; }
    
    public required IReadOnlyList<PersonOverviewEvent> Events { get; init; }
}