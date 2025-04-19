using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public class WeeklyReportQueryHandler(
    DailyOverviewQueryHandler dailyOverviewQueryHandler, 
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<IReadOnlyList<WeeklyReport>> HandleAsync(CancellationToken cancellationToken)
    {
        var startOfWeek = DateOnly.FromDateTime(dateTimeProvider.Now.StartOfWeek());

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userIds = await context.Users.ToListAsync(cancellationToken);

        var result =
            userIds.ToDictionary(
                keySelector: k => k.ToAggregate(), 
                elementSelector: _ => new Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)>());
        
        for (var i = 0; i < 7; i++)
        {
            var date = startOfWeek.AddDays(i);
            var overview = await dailyOverviewQueryHandler.HandleAsync(date, cancellationToken);
            
            foreach (var dailyOverview in overview)
            {
                result[dailyOverview.User][date] = (dailyOverview.IsAtHome, dailyOverview.DinnerTime);
            }
        }

        return result
            .Select(r => new WeeklyReport
            {
                User = r.Key,
                DailyOverviews = r.Value
            }).ToList();
    }
}