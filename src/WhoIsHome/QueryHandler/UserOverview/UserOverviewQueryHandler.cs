using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.Shared.Helper;

namespace WhoIsHome.QueryHandler.UserOverview;

public class UserOverviewQueryHandler(IDbContextFactory<WhoIsHomeContext> contextFactory, IDateTimeProvider dateTimeProvider)
{
    public async Task<UserOverview> HandleAsync(int userId, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var today = dateTimeProvider.CurrentDate;

        var oneTimeEvents = (await context.OneTimeEvents
                .Where(e => e.Date >= today)
                .Where(e => e.User.Id == userId)
                .ToListAsync(cancellationToken))
            .Select(m => m.ToAggregate());

        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.LastOccurrence == null || e.LastOccurrence >= today)
                .Where(e => e.UserId == userId)
                .ToListAsync(cancellationToken))
            .Select(m => m.ToAggregate());

        var userEvents = new List<EventBase>();
        userEvents.AddRange(oneTimeEvents);
        userEvents.AddRange(repeatedEvents);

        var todaysEvents = userEvents.Where(e => e.IsEventAt(dateTimeProvider.CurrentDate))
            .Select(e => new UserOverviewEvent
            {
                Id = e.Id!.Value,
                Title = e.Title,
                Date = e.GetNextOccurrence(dateTimeProvider.CurrentDate),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                EventType = EventTypeHelper.FromType(e)
            })
            .ToList();

        var futureEvents = userEvents
            .Where(e => !e.IsEventAt(dateTimeProvider.CurrentDate))
            .Select(e => (Event: e, Next: e.GetNextOccurrence(dateTimeProvider.CurrentDate)))
            .Where(e => e.Next > today)
            .ToList();

        var thisWeeksEvents = futureEvents
            .Where(e => e.Next.IsSameWeek(dateTimeProvider.Now))
            .Select(e => new UserOverviewEvent
            {
                Id = e.Event.Id!.Value,
                Title = e.Event.Title,
                Date = e.Next,
                StartTime = e.Event.StartTime,
                EndTime = e.Event.EndTime,
                EventType = EventTypeHelper.FromType(e.Event)
            })
            .ToList();

        var eventsAfterThisWeek = futureEvents
            .Where(e => !e.Next.IsSameWeek(dateTimeProvider.Now))
            .Select(e => new UserOverviewEvent
            {
                Id = e.Event.Id!.Value,
                Title = e.Event.Title,
                Date = e.Next,
                StartTime = e.Event.StartTime,
                EndTime = e.Event.EndTime,
                EventType = EventTypeHelper.FromType(e.Event)
            })
            .ToList();

        var user = (await context.Users
                .SingleAsync(u => u.Id == userId, cancellationToken))
            .ToAggregate();

        return new UserOverview
        {
            User = user,
            Today = todaysEvents,
            ThisWeek = thisWeeksEvents,
            FutureEvents = eventsAfterThisWeek
        };
    }
}