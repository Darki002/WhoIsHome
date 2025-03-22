using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(
    IDbContextFactory<WhoIsHomeContext> contextFactory, 
    UserDayOverviewQueryHandler userDayOverviewQueryHandler)
{
    public async Task<IReadOnlyCollection<DailyOverview>> HandleAsync(CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var userIds = await context.Users.Select(u => u.Id).ToListAsync(cancellationToken);

        var result = new List<DailyOverview>();
        
        foreach (var userId in userIds)
        {
            var overview = await userDayOverviewQueryHandler.HandleAsync(userId, cancellationToken);
            result.Add(overview);
        }

        return result;
    }
}