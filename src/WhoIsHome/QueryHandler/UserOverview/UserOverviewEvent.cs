namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewEvent
{
    public required int Id { get; init; }
    
    public required string Title { get; init; }
    
    public required DateOnly NextDate { get; init; }
    
    public required TimeOnly StartTime { get; init; }
    
    public required TimeOnly? EndTime { get; init; }
    
    public bool HasRepetitions { get; set; }
    
    public required int TemplateId { get; init; }
}