using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

public class EventService(    
    WhoIsHomeContext context,
    IEventUpdateHandler eventUpdateHandler, 
    IDateTimeProvider dateTimeProvider) : IEventService
{
    private const int DaysToGenerateInAdvance = 14;
    
    public async Task GenerateNewAsync(EventGroup eventGroup, CancellationToken cancellationToken)
    {
        var newEvents = GenerateFor(eventGroup, []);
        await context.EventInstances.AddRangeAsync(newEvents, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var eventToday = newEvents.SingleOrDefault(e => e.Date == dateTimeProvider.CurrentDate);
        if (eventToday is not null)
        {
            await eventUpdateHandler.HandleAsync(eventToday, EventUpdateHandler.UpdateAction.Create);
        }
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
            .Select(e => e.OriginalDate)
            .ToHashSet();

        var updatedEvents = GenerateFor(eventGroup, editedEvents);
        await context.EventInstances.AddRangeAsync(updatedEvents, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        var eventToday = updatedEvents.SingleOrDefault(e => e.Date == dateTimeProvider.CurrentDate);
        if (eventToday is not null)
        {
            await eventUpdateHandler.HandleAsync(eventToday, EventUpdateHandler.UpdateAction.Update);
        }
    }

    private List<EventInstance> GenerateFor(EventGroup eventGroup, HashSet<DateOnly> exceptions)
    {
        var endDate = dateTimeProvider.CurrentDate.StartOfWeek().AddDays(DaysToGenerateInAdvance);

        if (eventGroup.EndDate.HasValue && endDate > eventGroup.EndDate)
        {
            endDate = eventGroup.EndDate.Value;
        }

        var result = new List<EventInstance>();
        
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

            result.Add(instance);
        }

        return result;
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

    public async Task<ValidationError?> EditSingleInstanceAsync(
        int eventGroupId, 
        DateOnly originalDate,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        PresenceType presenceType,
        TimeOnly dinnerTime,
        CancellationToken cancellationToken)
    {
        var eventInstance = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .SingleOrDefaultAsync(e => e.Date == originalDate, cancellationToken);

        if (eventInstance is null)
        {
            return new ValidationError($"No Event found with date {originalDate}.");
        }
        
        eventInstance.Date = date;
        eventInstance.StartTime = startTime;
        eventInstance.EndTime = endTime;
        eventInstance.PresenceType = presenceType;
        eventInstance.DinnerTime = dinnerTime;
        eventInstance.IsOriginal = false;

        context.EventInstances.Update(eventInstance);
        await context.SaveChangesAsync(cancellationToken);
        return null;
    }

    public async Task DeleteSingleInstanceAsync(int eventGroupId, DateOnly date, CancellationToken cancellationToken)
    {
        var eventInstance = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .SingleOrDefaultAsync(e => e.Date == date, cancellationToken);

        if (eventInstance is not null)
        {
            context.EventInstances.Remove(eventInstance);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}