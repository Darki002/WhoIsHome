namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverviewEvent
{
    public required string Id { get; init; }
    
    public required string EventName { get; init; }
    
    public required DateTime Date { get; init; }
    
    public required DateTime StartTime { get; init; }
    
    public required DateTime EndTime { get; init; }
    
    public required EventType EventType { get; init; }
}