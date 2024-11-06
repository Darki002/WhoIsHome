using WhoIsHome.QueryHandler.DailyOverview;

namespace WhoIsHome.WebApi.DailyOverviews;

public record DailyOverviewModel
{
    public required int UserId { get; set; }

    public required bool IsAtHome { get; set; }

    public required TimeOnly? DinnerTime { get; set; }

    public static DailyOverviewModel From(DailyOverview dailyOverview)
    {
        return new DailyOverviewModel
        {
            UserId = dailyOverview.User.Id!.Value,
            IsAtHome = dailyOverview.IsAtHome,
            DinnerTime = dailyOverview.DinnerTime
        };
    }
}