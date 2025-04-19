using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.QueryServices.DailyOverviews;

public record DailyOverviewModel
{
    public required SimpleUserModel UserModel { get; set; }

    public required bool IsAtHome { get; set; }

    public required TimeOnly? DinnerTime { get; set; }

    public static DailyOverviewModel From(DailyOverview dailyOverview)
    {
        return new DailyOverviewModel
        {
            UserModel = new SimpleUserModel(dailyOverview.User.Id!.Value, dailyOverview.User.UserName),
            IsAtHome = dailyOverview.IsAtHome,
            DinnerTime = dailyOverview.DinnerTime
        };
    }
}