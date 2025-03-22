using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(IDbContextFactory<WhoIsHomeContext> contextFactory, IDateTimeProvider dateTimeProvider)
{
    public async Task<IReadOnlyCollection<DailyOverview>> HandleAsync(CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var users = (await context.Users.ToListAsync(cancellationToken))
            .Select(m => m.ToAggregate());

        var today = dateTimeProvider.CurrentDate;

        var oneTimeEvents = (await context.OneTimeEvents
                .Include(e => e.User)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.Date == today)
                .GroupBy(e => e.User.Id)
                .Select(g => new
                {
                    g.Key,
                    Data = g.Select(e => e)
                })
                .ToListAsync(cancellationToken))
            .ToDictionary(
                g => g.Key,
                g => g.Data.Select(m => m.ToAggregate()));

        var repeatedEvents = (await context.RepeatedEvents
                .Include(e => e.User)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.FirstOccurrence <= today)
                .Where(e => e.LastOccurrence >= today)
                .GroupBy(e => e.UserId)
                .Select(g => new
                {
                    g.Key,
                    Data = g.Select(e => e)
                })
                .ToListAsync(cancellationToken))
            .ToDictionary(
                g => g.Key,
                g => g.Data.Select(m => m.ToAggregate()));

        var eventsByUsers = new Dictionary<User, List<EventBase>>();

        foreach (var user in users)
        {
            var userEvents = new List<EventBase>();

            if (oneTimeEvents.TryGetValue(user.Id!.Value, out var userOneTimeEvents))
            {
                userEvents.AddRange(userOneTimeEvents);
            }
            if (repeatedEvents.TryGetValue(user.Id!.Value, out var userRepeatedEvents))
            {
                
                userEvents.AddRange(userRepeatedEvents
                    .Where(r => r.IsEventAt(today)));
            }
            
            eventsByUsers.Add(user, userEvents);
        }

        var result = new List<DailyOverview>();
        
        foreach (var eventByUser in eventsByUsers)
        {
            if (eventByUser.Value.Count == 0)
            {
                var presence = DailyOverview.Empty(eventByUser.Key);
                result.Add(presence);
                continue;
            }

            if (TryGetNotPresence(eventByUser.Value, out var time))
            {
                result.Add(DailyOverview.From(eventByUser.Key, time!));
                continue;
            }

            var nextEvent = eventByUser.Value.MaxBy(e => e.DinnerTime.Time);
            var userPresence = GetUserPresence(nextEvent, eventByUser.Key);
            result.Add(userPresence);
        }

        return result;
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