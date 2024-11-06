namespace WhoIsHome.WebApi.PersonOverviews;

public record UserOverviewModel
{
    public required int UserId { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> Today { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> ThisWeek { get; init; }
    
    public required IReadOnlyList<PersonOverviewEventModel> FutureEvents { get; init; }
}