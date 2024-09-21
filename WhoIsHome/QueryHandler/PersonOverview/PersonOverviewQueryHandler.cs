using Microsoft.EntityFrameworkCore;
using WhoIsHome.Aggregates;
using WhoIsHome.DataAccess;
using WhoIsHome.DataAccess.Models;
using WhoIsHome.Shared;

namespace WhoIsHome.QueryHandler.PersonOverview;

public class PersonOverviewQueryHandler(WhoIsHomeContext context)
{
    public async Task<PersonOverview> HandleAsync(int userId, CancellationToken cancellationToken)
    {
        var today = DateOnlyHelper.Today;

        var oneTimeEvents = (await context.OneTimeEvents
                .Where(e => e.Date > today)
                .Where(e => e.UserModel.Id == userId)
                .ToListAsync(cancellationToken))
            .ToAggregateList<OneTimeEvent, OneTimeEventModel>();

        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.LastOccurrence > today)
                .Where(e => e.UserModel.Id == userId)
                .ToListAsync(cancellationToken))
            .ToAggregateList<RepeatedEvent, RepeatedEventModel>();


        var userEvents = new List<EventBase>();
        userEvents.AddRange(oneTimeEvents);
        userEvents.AddRange(repeatedEvents);

        var todaysEvents = userEvents.Where(e => e.IsToday)
            .Select(e => new PersonOverviewEvent
            {
                Id = e.Id!.Value,
                Title = e.Title,
                Date = e.GetNextOccurrence(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                EventType = EventTypeHelper.FromType(e)
            })
            .ToList();

        var futureEvents = userEvents
            .Where(e => !e.IsToday)
            .Select(e => (Event: e, Next: e.GetNextOccurrence()))
            .Where(e => e.Next > today)
            .ToList();

        var thisWeeksEvents = futureEvents
            .Where(e => e.Next.IsThisWeek())
            .Select(e => new PersonOverviewEvent
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
            .Where(e => !e.Next.IsThisWeek())
            .Select(e => new PersonOverviewEvent
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
            .ToAggregate<User>();

        return new PersonOverview
        {
            User = user,
            Today = todaysEvents,
            ThisWeek = thisWeeksEvents,
            FutureEvents = eventsAfterThisWeek
        };
    }
}