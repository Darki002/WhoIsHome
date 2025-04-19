using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.QueryHandler.DailyOverview;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.WeeklyReports;

public class WeeklyReportHandler
{
    public async Task<IReadOnlyList<WeeklyReport>> HandleAsync(
        DailyOverviewQueryHandler dailyOverviewQueryHandler, 
        IDbContextFactory<WhoIsHomeContext> contextFactory,
        IDateTimeProvider dateTimeProvider,
        CancellationToken cancellationToken)
    {
        var startOfWeek = DateOnly.FromDateTime(dateTimeProvider.Now.StartOfWeek());

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userIds = await context.Users.Select(u => u.Id).ToListAsync(cancellationToken);

        var result =
            userIds.ToDictionary(
                keySelector: k => k, 
                elementSelector: _ => new Dictionary<DateOnly, (bool IsAtHome, TimeOnly? DinnerTime)>());
        
        for (var i = 0; i < 7; i++)
        {
            var date = startOfWeek.AddDays(i);
            var overview = await dailyOverviewQueryHandler.HandleAsync(date, cancellationToken);
            
            foreach (var dailyOverview in overview)
            {
                var id = dailyOverview.User.Id!.Value;
                result[id][date] = (dailyOverview.IsAtHome, dailyOverview.DinnerTime);
            }
        }

        return result
            .Select(r => new WeeklyReport
            {
                UserId = r.Key,
                DailyOverviews = r.Value
            }).ToList();
    }
}