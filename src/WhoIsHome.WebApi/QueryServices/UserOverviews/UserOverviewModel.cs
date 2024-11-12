namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

public record UserOverviewModel
{
    public required int UserId { get; init; }

    public required IReadOnlyList<UserOverviewEventModel> Today { get; init; }

    public required IReadOnlyList<UserOverviewEventModel> ThisWeek { get; init; }

    public required IReadOnlyList<UserOverviewEventModel> FutureEvents { get; init; }
}