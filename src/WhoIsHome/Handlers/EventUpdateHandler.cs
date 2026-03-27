using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.External.PushUp;
using WhoIsHome.External.Translation;
using WhoIsHome.Shared.BackgroundTasks;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Handlers;

public class EventUpdateHandler(
    WhoIsHomeContext context, 
    IPushUpContext pushUpContext,
    IDateTimeProvider dateTimeProvider,
    IBackgroundTaskQueue backgroundTaskQueue,
    ILogger<EventUpdateHandler> logger) : IEventUpdateHandler
{
    public async Task HandleAsync(int userId, IEnumerable<EventInstance> updatedEvents, UpdateAction updateAction)
    {
        var today = updatedEvents
            .Where(e => e.Date == dateTimeProvider.CurrentDate)
            .ToList();
        
        if (today.Count > 0)
        {
            await backgroundTaskQueue.QueueBackgroundWorkItemAsync(RunAsync);
        }
        
        return;

        async ValueTask RunAsync(CancellationToken cancellationToken)
        {
            var events = await GetUserEventsFromTodayAsync(userId, cancellationToken);

            var shouldSend = updateAction switch
            {
                UpdateAction.Create => CheckUpdate(today, events),
                UpdateAction.Update => CheckUpdate(today, events),
                UpdateAction.Delete => CheckDelete(today, events),
                _ => throw new ArgumentOutOfRangeException(nameof(updateAction), "No command for this Action.")
            };

            if (!shouldSend)
            {
                logger.LogDebug("Skip Push Up Notification, since there is no change in the DinnerTime for today.");
                return;
            }
            
            var user = await context.Users.SingleAsync(u => u.Id == userId, cancellationToken);
            var users = await context.Users
                .Where(u => u.Id != userId)
                .ToListAsync(cancellationToken);
            
            var command = new PushUpCommand(
                Title: TranslationKeys.DinnerTimeChange,
                Body: new TranslatableString(TranslationKeys.UserHasUpdated, user.UserName),
                users.Select(u => u.Id).ToArray());
            await pushUpContext.PushEventUpdateAsync(command);
        }
    }

    private static bool CheckDelete(IEnumerable<EventInstance> updatedEvents, List<EventInstance> events)
    {
        var dinnerTimeEvent = events.MaxBy(e => e.DinnerTime);
        return dinnerTimeEvent is null || updatedEvents.Any(e => dinnerTimeEvent.DinnerTime < e.DinnerTime);
    }

    private static bool CheckUpdate(IEnumerable<EventInstance> updatedEvents, List<EventInstance> events)
    {
        var dinnerTimeEvent = events.MaxBy(e => e.DinnerTime);
        return updatedEvents.Any(e => e.Id == dinnerTimeEvent!.Id);
    }

    private async Task<List<EventInstance>> GetUserEventsFromTodayAsync(int userId, CancellationToken cancellationToken)
    {
        return await context.EventInstances
            .Where(e => e.DeleteDate == null)
            .Where(e => e.UserId == userId)
            .Where(e => e.PresenceType != PresenceType.Unknown)
            .Where(e => e.Date == dateTimeProvider.CurrentDate)
            .ToListAsync(cancellationToken);
    }
    
    public enum UpdateAction
    {
        Create,
        Update,
        Delete
    }
}