using Microsoft.EntityFrameworkCore;
using WhoIsHome.External.Database;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public class WeeklyReportQueryHandler(
    DailyOverviewQueryHandler dailyOverviewQueryHandler, 
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<IReadOnlyList<WeeklyReportMock>> HandleAsync(CancellationToken cancellationToken)
    {
        var startOfWeek = DateOnly.FromDateTime(dateTimeProvider.Now.StartOfWeek());

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
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
                    result[dailyOverview.User].Report[date] = (dailyOverview.IsAtHome, dailyOverview.DinnerTime);
                }
            }
        }

        return result
            .Select(r => new WeeklyReportMock
            {
                User = r.Key,
                Report = r.Value
            }).ToList();
    }
}

public record WeeklyReportResult
{
    public readonly Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)> Report = [];
    public string? ErrorMessage { get; set; }
}