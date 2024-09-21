using WhoIsHome.Aggregates;

namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverview
{
    public required User User { get; init; }
    
    public required IReadOnlyList<PersonOverviewEvent> Today { get; init; }
    
    public required IReadOnlyList<PersonOverviewEvent> ThisWeek { get; init; }
    
    public required IReadOnlyList<PersonOverviewEvent> FutureEvents { get; init; }
}