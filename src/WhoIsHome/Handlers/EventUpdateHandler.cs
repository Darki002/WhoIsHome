using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.Aggregates;
using WhoIsHome.Aggregates.Mappers;
using WhoIsHome.External;
using WhoIsHome.External.PushUp;
using WhoIsHome.External.Translation;
using WhoIsHome.Shared.BackgroundTasks;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Handlers;

public class EventUpdateHandler(
    IDbContextFactory<WhoIsHomeContext> contextFactory, 
    IPushUpContext pushUpContext,
    IDateTimeProvider dateTimeProvider,
    IBackgroundTaskQueue backgroundTaskQueue,
    ILogger<EventUpdateHandler> logger) : IEventUpdateHandler
{
    public async Task HandleAsync(EventBase updatedEvent, UpdateAction updateAction)
    {
        await backgroundTaskQueue.QueueBackgroundWorkItemAsync(RunAsync);
        return;

        async ValueTask RunAsync(CancellationToken cancellationToken)
        {
            var events = await GetUserEventsFromTodayAsync(updatedEvent, cancellationToken);

            var shouldSend = updateAction switch
            {
                UpdateAction.Create => CheckUpdate(updatedEvent, events),
                UpdateAction.Update => CheckUpdate(updatedEvent, events),
                UpdateAction.Delete => CheckDelete(updatedEvent, events),
                _ => throw new ArgumentOutOfRangeException(nameof(updateAction), "No command for this Action.")
            };

            if (!shouldSend)
            {
                logger.LogDebug("Skip Push Up Notification, since there is no change in the DinnerTime for today.");
                return;
            }

            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            
            var user = await context.Users.SingleAsync(u => u.Id == updatedEvent.UserId, cancellationToken);
            var users = await context.Users
                .Where(u => u.Id != updatedEvent.UserId)
                .ToListAsync(cancellationToken);
            
            var command = new PushUpCommand(
                Title: TranslationKeys.DinnerTimeChange,
                Body: new TranslatableString(TranslationKeys.UserHasUpdated, user.UserName),
                users.Select(u => u.Id).ToArray());
            await pushUpContext.PushEventUpdateAsync(command);
        }
    }

    private static bool CheckDelete(EventBase updatedEvent, List<EventBase> events)
    {
        var dinnerTimeEvent = events.MaxBy(e => e.DinnerTime.Time);
        return dinnerTimeEvent is null || dinnerTimeEvent.DinnerTime.Time < updatedEvent.DinnerTime.Time;
    }

    private static bool CheckUpdate(EventBase updatedEvent, List<EventBase> events)
    {
        var dinnerTimeEvent = events.MaxBy(e => e.DinnerTime.Time);
        return dinnerTimeEvent?.Id == updatedEvent.Id;
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
    
    public enum UpdateAction
    {
        Create,
        Update,
        Delete
    }
}

public interface IEventUpdateHandler
{
    Task HandleAsync(EventBase updatedEvent, EventUpdateHandler.UpdateAction updateAction);
}