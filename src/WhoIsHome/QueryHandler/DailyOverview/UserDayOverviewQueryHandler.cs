using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class UserDayOverviewQueryHandler(IDbContextFactory<WhoIsHomeContext> contextFactory)
{
    public async Task<DailyOverview> HandleAsync(int id, DateOnly date, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = (await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken));
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {id} was not found for UserDayOverview");
        }

        var oneTimeEvents = (await context.OneTimeEvents
                .Where(e => e.UserId == id)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.Date == date)
                .ToListAsync(cancellationToken))
            .Select(e => e.ToAggregate());

        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.UserId == id)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.FirstOccurrence <= date)
                .Where(e => e.LastOccurrence == null || e.LastOccurrence >= date)
                .ToListAsync(cancellationToken))
            .Select(e => e.ToAggregate())
            .Where(e => e.IsEventAt(date));

        List<EventBase> events = [..oneTimeEvents, ..repeatedEvents];
        
        if (events.Count == 0)
        {
            return DailyOverview.Empty(new User(user));
        }

        if (TryGetNotPresence(events, out var time))
        {
            return DailyOverview.From(new User(user), time!);
        }

        var nextEvent = events.MaxBy(e => e.DinnerTime.Time);
        return GetUserPresence(nextEvent, new User(user));
    }
    
    private static DailyOverview GetUserPresence(EventBase? eventBase, User user)
    {
        return eventBase == null ? DailyOverview.Empty(user) : DailyOverview.From(user, eventBase.DinnerTime);
    }
    
    private static bool TryGetNotPresence(IReadOnlyCollection<EventBase> events, out DinnerTime? dinnerTime)
    {
        var result = events.FirstOrDefault(e => e.DinnerTime.PresenceType == PresenceType.NotPresent);
        dinnerTime = result?.DinnerTime;
        return result is not null;
    }
}