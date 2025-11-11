using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External;
using WhoIsHome.External.Database;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class UserDayOverviewQueryHandler(WhoIsHomeContext context)
{
    public async Task<DailyOverview> HandleAsync(int id, DateOnly date, CancellationToken cancellationToken)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        
        if (user is null)
        {
            return DailyOverview.Error($"User with ID {id} was not found for UserDayOverview");
        }

        var eventList = await context.EventInstances
            .Where(e => e.UserId == id)
            .Where(e => e.PresenceType != PresenceType.Unknown)
            .Where(e => e.Date == date)
            .ToListAsync(cancellationToken);
        
        if (eventList.Count == 0)
        {
            return DailyOverview.Empty(user);
        }

        if (TryGetNotPresence(eventList, out var result))
        {
            return DailyOverview.From(result!);
        }

        var nextEvent = eventList.MaxBy(e => e.DinnerTime);
        return GetUserPresence(nextEvent, user);
    }
    
    private static DailyOverview GetUserPresence(EventInstance? eventInstance, User user)
    {
        return eventInstance is null ? DailyOverview.Empty(user) : DailyOverview.From(eventInstance);
    }
    
    private static bool TryGetNotPresence(IReadOnlyCollection<EventInstance> events, out EventInstance? result)
    {
        result = events.FirstOrDefault(e => e.PresenceType is PresenceType.NotPresent);
        return result is not null;
    }
}