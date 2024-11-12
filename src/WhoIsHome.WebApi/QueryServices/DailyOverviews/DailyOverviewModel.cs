using WhoIsHome.QueryHandler.DailyOverview;

namespace WhoIsHome.WebApi.DailyOverviews;

public record DailyOverviewModel
{
    public required DailyOverviewUser User { get; set; }

    public required bool IsAtHome { get; set; }

    public required TimeOnly? DinnerTime { get; set; }

    public static DailyOverviewModel From(DailyOverview dailyOverview)
    {
        return new DailyOverviewModel
        {
            User = new DailyOverviewUser(dailyOverview.User.Id!.Value, dailyOverview.User.UserName),
            IsAtHome = dailyOverview.IsAtHome,
            DinnerTime = dailyOverview.DinnerTime
        };
    }
}

public record DailyOverviewUser(int id, string Username);