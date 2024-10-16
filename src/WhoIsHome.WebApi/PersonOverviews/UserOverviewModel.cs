using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.PersonOverviews;

public record UserOverviewModel
{
    public required UserModel User { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> Today { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> ThisWeek { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> FutureEvents { get; init; }
}