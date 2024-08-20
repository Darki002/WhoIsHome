namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverviewEvent
{
    public required string Id { get; init; }
    
    public required string EventName { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public required TimeOnly StartTime { get; init; }
    
    public required TimeOnly EndTime { get; init; }
    
    public required EventType EventType { get; init; }
}