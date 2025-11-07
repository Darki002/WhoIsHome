using Microsoft.EntityFrameworkCore;
using WhoIsHome.External;
using WhoIsHome.External.Database;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(
    IDbContextFactory<WhoIsHomeContext> contextFactory, 
    UserDayOverviewQueryHandler userDayOverviewQueryHandler)
{
    public async Task<IReadOnlyCollection<DailyOverview>> HandleAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userIds = await context.Users.Select(u => u.Id).ToListAsync(cancellationToken);

        var result = new List<DailyOverview>();
        
        foreach (var userId in userIds)
        {
            var overview = await userDayOverviewQueryHandler.HandleAsync(userId, date, cancellationToken);
            result.Add(overview);
        }

        return result;
    }
}