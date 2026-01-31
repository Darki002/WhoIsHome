namespace WhoIsHome.WebApi.QueryServices.UserOverviews;

public record UserOverviewModel
{
    public required int UserId { get; init; }

    public required IReadOnlyList<UserOverviewEventModel> Events { get; init; }
}