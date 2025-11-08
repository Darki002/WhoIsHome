using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;

namespace WhoIsHome.Services;

public class EventService(    
    WhoIsHomeContext context,
    IEventUpdateHandler eventUpdateHandler, 
    IDateTimeProvider dateTimeProvider) : IEventService
{
    private const int DaysToGenerateInAdvance = 14;
    
    public async Task GenerateNewAsync(EventGroup eventGroup, CancellationToken cancellationToken)
    {
        await context.EventInstances.AddRangeAsync(GenerateFor(eventGroup, []), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task GenerateUpdateAsync(EventGroup eventGroup, CancellationToken cancellationToken)
    {
        var existingEvents = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroup.Id)
            .ToListAsync(cancellationToken);

        var forDeletion = existingEvents.Where(e => e.IsOriginal);
        context.RemoveRange(forDeletion);

        var editedEvents = existingEvents
            .Where(e => !e.IsOriginal)
            .Select(e => e.Date)
            .ToHashSet();
        
        await context.EventInstances.AddRangeAsync(GenerateFor(eventGroup, editedEvents), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private IEnumerable<EventInstance> GenerateFor(EventGroup eventGroup, HashSet<DateOnly> exceptions)
    {
        var endDate = dateTimeProvider.CurrentDate.StartOfWeek().AddDays(DaysToGenerateInAdvance);

        if (eventGroup.EndDate.HasValue && endDate > eventGroup.EndDate)
        {
            endDate = eventGroup.EndDate.Value;
        }
        
        foreach (var currentDate in GetDatesUntil(eventGroup.StartDate, endDate, eventGroup.WeekDays))
        {
            if (exceptions.Contains(currentDate))
            {
                continue;
            }
            
            var instance = new EventInstance
            {
                Title = eventGroup.Title,
                Date = currentDate,
                StartTime = eventGroup.StartTime,
                EndTime = eventGroup.EndTime,
                PresenceType = eventGroup.PresenceType,
                DinnerTime = eventGroup.DinnerTime,
                IsOriginal = true,
                OriginalDate = currentDate,
                EventGroupId = eventGroup.Id,
                UserId = eventGroup.UserId
            };

            yield return instance;
        }
    }
    
    private static IEnumerable<DateOnly> GetDatesUntil(DateOnly startDate, DateOnly endDate, WeekDay weekDays)
    {
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            if (weekDays.HasFlag(currentDate.DayOfWeek.ToWeekDay()))
            {
                yield return currentDate;
            }
            
            currentDate = currentDate.AddDays(1);
        }
    }

    public async Task DeleteAsync(int eventGroupId)
    {
        var events = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .Where(e => e.IsOriginal)
            .ToListAsync();

        var eventFromToday = events.SingleOrDefault(e => e.Date == dateTimeProvider.CurrentDate);
        
        context.EventInstances.RemoveRange(events);
        await context.SaveChangesAsync();

        if (eventFromToday is not null)
        {
            await eventUpdateHandler.HandleAsync(eventFromToday, EventUpdateHandler.UpdateAction.Delete);
        }
    }
}