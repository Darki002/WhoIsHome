using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewMock
{
    public required User User { get; init; }
    
    public required IReadOnlyList<UserOverviewEvent> Today { get; init; }
    
    public required IReadOnlyList<UserOverviewEvent> ThisWeek { get; init; }
    
    public required IReadOnlyList<UserOverviewEvent> FutureEvents { get; init; }
}