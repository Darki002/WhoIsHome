using WhoIsHome.Services.Persons;

namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverview
{
    public required Person Person { get; init; }
    
    public required IReadOnlyList<PersonOverviewEvent> Events { get; init; }
}