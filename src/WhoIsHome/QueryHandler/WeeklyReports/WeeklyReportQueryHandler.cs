using Microsoft.EntityFrameworkCore;
using WhoIsHome.External.Database;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public class WeeklyReportQueryHandler(
    DailyOverviewQueryHandler dailyOverviewQueryHandler, 
    WhoIsHomeContext context,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<IReadOnlyList<WeeklyReport>> HandleAsync(CancellationToken cancellationToken)
    {
        var startOfWeek = DateOnly.FromDateTime(dateTimeProvider.Now.StartOfWeek());

        var userIds = await context.Users.ToListAsync(cancellationToken);

        var result =
            userIds.ToDictionary(
                keySelector: k => k, 
                elementSelector: _ => new WeeklyReportResult());
        
        for (var i = 0; i < 7; i++)
        {
            var date = startOfWeek.AddDays(i);
            var overview = await dailyOverviewQueryHandler.HandleAsync(date, cancellationToken);
            
            foreach (var dailyOverview in overview)
            {
                if (dailyOverview.HasError)
                {
                    result[dailyOverview.User].ErrorMessage = dailyOverview.ErrorMessage;
                }
                else
                {
                    result[dailyOverview.User].Value[date] = (dailyOverview.IsAtHome, dailyOverview.DinnerTime);
                }
            }
        }

        return result
            .Select(r => new WeeklyReport
            {
                User = r.Key,
                Report = r.Value
            }).ToList();
    }
}

public record WeeklyReportResult
{
    public readonly Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)> Value = [];
    public string? ErrorMessage { get; set; }
}