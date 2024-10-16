using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.DailyOverviews;

public record DailyOverviewModel
{
    public required UserModel User { get; set; }

    public required bool IsAtHome { get; set; }

    public required TimeOnly? DinnerTime { get; set; }

    public static DailyOverviewModel From(DailyOverview dailyOverview)
    {
        return new DailyOverviewModel
        {
            User = UserModel.From(dailyOverview.User),
            IsAtHome = dailyOverview.IsAtHome,
            DinnerTime = dailyOverview.DinnerTime
        };
    }
}