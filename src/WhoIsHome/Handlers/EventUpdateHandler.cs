using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.External.PushUp;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Handlers;

public class EventUpdateHandler(
    IDbContextFactory<WhoIsHomeContext> contextFactory, 
    IDateTimeProvider dateTimeProvider,
    IPushUpContext pushUpContext,
    ILogger<EventUpdateHandler> logger)
{
    // TODO: Unit Test
    
    public async Task HandleAsync(EventBase updatedEvent, CancellationToken cancellationToken)
    {
        var events = await GetUserEventsFromTodayAsync(updatedEvent, cancellationToken);

        var dinnerTimeEvent = events.MaxBy(e => e.DinnerTime.Time);
        if (dinnerTimeEvent?.Id != updatedEvent.Id)
        {
            logger.LogDebug("Skip Push Up Notification, since there is no change in the DinnerTime for today.");
            return;
        }

        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users.SingleAsync(u => u.Id == updatedEvent.UserId, cancellationToken);

        var users = await context.Users
            .Where(u => u.Id != updatedEvent.UserId)
            .ToListAsync(cancellationToken);

        var command = new PushUpEventUpdateCommand(
            Title: "Event Update",
            Body: $"{user.UserName} has entered a new Event for Today.",
            users.Select(u => u.Id).ToArray());
        pushUpContext.PushEventUpdate(command, cancellationToken);
    }

    private async Task<List<EventBase>> GetUserEventsFromTodayAsync(EventBase updatedEvent, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        
        var oneTimeEvents = (await context.OneTimeEvents
                .Where(e => e.UserId == updatedEvent.UserId)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.Date == dateTimeProvider.CurrentDate)
                .ToListAsync(cancellationToken))
            .Select(e => e.ToAggregate());

        var repeatedEvents = (await context.RepeatedEvents
                .Where(e => e.UserId == updatedEvent.UserId)
                .Where(e => e.PresenceType != PresenceType.Unknown)
                .Where(e => e.FirstOccurrence <= dateTimeProvider.CurrentDate)
                .Where(e => e.LastOccurrence >= dateTimeProvider.CurrentDate)
                .ToListAsync(cancellationToken))
            .Select(e => e.ToAggregate())
            .Where(e => e.IsEventAt(dateTimeProvider.CurrentDate));

         return [..oneTimeEvents, ..repeatedEvents];
    }
}