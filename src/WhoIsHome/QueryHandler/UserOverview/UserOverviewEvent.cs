namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewEvent
{
    public required int Id { get; init; }
    
    public required string Title { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public required TimeOnly StartTime { get; init; }
    
    public required TimeOnly EndTime { get; init; }
    
    public required EventType EventType { get; init; }
}