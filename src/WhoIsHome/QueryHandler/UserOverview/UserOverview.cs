using WhoIsHome.Entities;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverview
{
    public required User User { get; init; }
    
    public required IReadOnlyList<UserOverviewEvent> Events { get; init; }
}