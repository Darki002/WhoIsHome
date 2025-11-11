using WhoIsHome.QueryHandler.WeeklyReports;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.QueryServices.WeeklyReports;

public record WeeklyReportModel
{
    public required SimpleUserModel User { get; set; }
    
    public required List<DailyOverviewReport> DailyOverviews { get; init; }
    
    public static WeeklyReportModel From(WeeklyReport dailyOverview)
    {
        return new WeeklyReportModel
        {
            User = new SimpleUserModel(dailyOverview.User.Id, dailyOverview.User.UserName),
            DailyOverviews = dailyOverview.Report.Value
                .Select(o => new DailyOverviewReport(o.Key, o.Value.IsAtHome, o.Value.DinnerTime))
                .ToList(),
        };
    }
}

public record DailyOverviewReport(DateOnly Date, bool IsAtHome, TimeOnly? DinnerTime);