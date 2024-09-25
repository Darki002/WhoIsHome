using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared;
using WhoIsHome.Shared.Framework;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.QueryHandler.DailyOverview;

public class DailyOverviewQueryHandler(WhoIsHomeContext context)
{
    public async Task<IReadOnlyCollection<DailyOverview>> HandleAsync(CancellationToken cancellationToken)
    {
        var users = (await context.Users.ToListAsync(cancellationToken))
            .ToAggregateList<User, UserModel>();

        var today = DateOnly.FromDateTime(DateTime.Today);

        var oneTimeEvents = (await context.OneTimeEvents
                .Where(e => e.DinnerTimeModel.PresentsType != PresentsType.Unknown)
                .Where(e => e.Date == today)
                .GroupBy(e => e.UserModel.Id)
                .ToListAsync(cancellationToken))
            .ToDictionary(
                g => g.Key,
                g => g.ToAggregateList<OneTimeEvent, OneTimeEventModel>());

        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.DinnerTimeModel.PresentsType != PresentsType.Unknown)
                .Where(e => e.FirstOccurrence > today)
                .Where(e => e.LastOccurrence <= today)
                .GroupBy(e => e.UserModel.Id)
                .ToListAsync(cancellationToken))
            .ToDictionary(
                g => g.Key,
                g => g.ToAggregateList<RepeatedEvent, RepeatedEventModel>());

        var eventsByUsers = new Dictionary<User, List<EventBase>>();

        foreach (var user in users)
        {
            var userEvents = new List<EventBase>();
            userEvents.AddRange(oneTimeEvents[user.Id!.Value]);
            userEvents.AddRange(repeatedEvents[user.Id.Value]);
            eventsByUsers.Add(user, userEvents);
        }

        var result = new List<DailyOverview>();
        
        foreach (var eventByUser in eventsByUsers)
        {
            var nextEvent = eventByUser.Value.Select(e => (Event: e, NextOccurrence: e.GetNextOccurrence()))
                .MaxBy(e => e.NextOccurrence)
                .Event;

            var personPresence = GetPersonPresence(nextEvent, eventByUser.Key);
            result.Add(personPresence);
        }

        return result;
    }

    private static DailyOverview GetPersonPresence(EventBase? eventBase, User user)
    {
        return eventBase == null ? DailyOverview.Empty(user) : DailyOverview.From(user, eventBase.DinnerTime);
    }
}