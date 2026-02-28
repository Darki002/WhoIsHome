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
    
    public async Task GenerateNewAsync(EventGroup eventGroup)
    {
        var newEvents = GenerateFor(eventGroup, []);
        await context.EventInstances.AddRangeAsync(newEvents);
        await context.SaveChangesAsync();

        var eventToday = newEvents.SingleOrDefault(e => e.Date == dateTimeProvider.CurrentDate);
        if (eventToday is not null)
        {
            await eventUpdateHandler.HandleAsync(eventToday, EventUpdateHandler.UpdateAction.Create);
        }
    }

    public async Task GenerateUpdateAsync(EventGroup eventGroup)
    {
        var existingEvents = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroup.Id)
            .ToListAsync();

        var forDeletion = existingEvents.Where(e => e.IsOriginal);
        context.EventInstances.RemoveRange(forDeletion);

        var editedEvents = existingEvents
            .Where(e => !e.IsOriginal)
            .ToHashSet();
        context.EventInstances.UpdateRange(editedEvents);
        
        foreach (var eventInstance in editedEvents)
        {
            eventInstance.Title = eventGroup.Title;
        }

        var updatedEvents = GenerateFor(eventGroup, editedEvents.Select(e => e.OriginalDate).ToHashSet());
        await context.EventInstances.AddRangeAsync(updatedEvents);
        await context.SaveChangesAsync();
        
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
        
        return GenerateForDuration(
            eventGroup: eventGroup, 
            start: eventGroup.StartDate, 
            end: endDate, 
            exceptions: exceptions)
            .ToList();
    }
    
    public async Task GenerateNextAsync(EventGroup eventGroup, CancellationToken cancellationToken)
    {
        var existingEventDates = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroup.Id)
            .Where(e => e.Date >= dateTimeProvider.CurrentDate)
            .Select(e => e.OriginalDate)
            .ToListAsync(cancellationToken);
        
        var today = dateTimeProvider.CurrentDate;
        var result = GenerateForDuration(
            eventGroup: eventGroup, 
            start: today, 
            end: today.AddDays(14), 
            exceptions: existingEventDates.ToHashSet());
        
        await context.EventInstances.AddRangeAsync(result, cancellationToken);
        await context.SaveChangesAsync(cancellationToken); 
    }
    
    private static IEnumerable<EventInstance> GenerateForDuration(
        EventGroup eventGroup, 
        DateOnly start, 
        DateOnly end,
        HashSet<DateOnly> exceptions)
    {
        return GetDatesUntil(start, end, eventGroup.WeekDays)
            .Except(exceptions)
            .Select(CreateEvent);

        EventInstance CreateEvent(DateOnly currentDate)
        {
            return new EventInstance
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

    public async Task<EventInstance?> FindEventInstance(int eventGroupId, DateOnly originalDate)
    {
        var eventGroup = await context.EventGroups
            .Include(e => e.Events)
            .SingleOrDefaultAsync(e => e.Id == eventGroupId);

        if (eventGroup is null)
        {
            return null;
        }

        var result = eventGroup.Events.SingleOrDefault(e => e.OriginalDate == originalDate);

        if (result is null)
        {
            var isWeekDayOfGroup = eventGroup.WeekDays.ToDayOfWeekList().Contains(originalDate.DayOfWeek);
            if (isWeekDayOfGroup)
            {
                result = new EventInstance
                {
                    Title = eventGroup.Title,
                    Date = originalDate,
                    StartTime = eventGroup.StartTime,
                    EndTime = eventGroup.EndTime,
                    PresenceType = eventGroup.PresenceType,
                    DinnerTime = eventGroup.DinnerTime,
                    IsOriginal = true,
                    OriginalDate = originalDate,
                    UserId = eventGroup.UserId,
                    EventGroupId = eventGroupId,
                };
            }
        }
        
        return result;
    }

    public async Task<IReadOnlyList<EventInstance>> PredictNextAsync(
        EventGroup eventGroup, 
        DateOnly start, 
        int weeks,
        CancellationToken cancellationToken)
    {
        var endDate = start.AddDays(7 * weeks);
        
        var result = await context.EventInstances
            .Where(e => e.Date >= start)
            .Where(e => e.Date < endDate)
            .Where(e => e.EventGroupId == eventGroup.Id)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken: cancellationToken);

        var dbDates = result.Select(e => e.OriginalDate).ToHashSet();
        var predictedEvents = GenerateForDuration(
            eventGroup: eventGroup, 
            start: start, 
            end: endDate.AddDays(-1),
            exceptions: dbDates);
        
        return result
            .Concat(predictedEvents)
            .OrderBy(e => e.Date)
            .ToList();
    }
}