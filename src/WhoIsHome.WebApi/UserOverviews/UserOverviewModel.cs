namespace WhoIsHome.WebApi.UserOverviews;

public record UserOverviewModel
{
    public required UserModel User { get; init; }
    
    public required IReadOnlyList<UserOverviewEventModel> Today { get; init; }
    
    public required IReadOnlyList<UserOverviewEventModel> ThisWeek { get; init; }
    
    public required IReadOnlyList<UserOverviewEventModel> FutureEvents { get; init; }

    public record UserModel(int Id, string UserName);
}