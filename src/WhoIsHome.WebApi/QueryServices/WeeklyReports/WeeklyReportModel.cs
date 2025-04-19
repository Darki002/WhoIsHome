using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.QueryServices.WeeklyReports;

public record WeeklyReportModel
{
    public required SimpleUserModel User { get; set; }
    
    public required Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)> DailyOverviews { get; init; }
    
    public static WeeklyReportModel From(WeeklyReport dailyOverview)
    {
        return new WeeklyReportModel
        {
            User = new SimpleUserModel(dailyOverview.User.Id!.Value, dailyOverview.User.UserName),
            DailyOverviews = dailyOverview.DailyOverviews,
        };
    }
}