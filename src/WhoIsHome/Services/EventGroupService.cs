using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

internal class EventGroupService(
    IDbContextFactory<WhoIsHomeContext> contextFactory,
    IEventUpdateHandler eventUpdateHandler, 
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext) 
    : IEventGroupService
{
    public async Task<EventGroup> CreateAsync(string title, DateOnly startDate, DateOnly? endDate,
        TimeOnly startTime, TimeOnly? endTime, WeekDay weekDays, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var eventGroup = new EventGroup(
            title: title, 
            startDate: startDate, 
            endDate: endDate, 
            weekDays: weekDays, 
            startTime: startTime, 
            endTime: endTime, 
            presenceType: presenceType,
            dinnerTime: time, 
            userId: userContext.UserId);

        var result = await context.EventGroups.AddAsync(eventGroup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        // TODO: generate events for next 2 Weeks

        await eventUpdateHandler.HandleAsync(result.Entity, EventUpdateHandler.UpdateAction.Create);
        
        return result.Entity;
    }

    public async Task<ValidationResult<EventGroup>> UpdateAsync(int id, string title, DateOnly startDate,
        DateOnly? endDate, TimeOnly startTime, TimeOnly? endTime, WeekDay weekDays, PresenceType presenceType, TimeOnly? time,
        CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var eventGroup = context.EventGroups.SingleOrDefault(e => e.Id == id);

        if (eventGroup is null)
        {
            return ValidationResult<EventGroup>.Error($"EventGroup with id {id} not found.");
        }
        
        if (!userContext.IsUserPermitted(eventGroup.UserId))
        {
            return ValidationResult<EventGroup>.Error(
                $"User with ID {eventGroup.UserId} is not allowed to delete or modify the content of {id}");
        }

        eventGroup.Title = title;
        eventGroup.StartDate = startDate;
        eventGroup.EndDate = endDate;
        eventGroup.StartTime = startTime;
        eventGroup.EndTime = endTime;
        eventGroup.WeekDays = weekDays;
        eventGroup.PresenceType = presenceType;
        eventGroup.DinnerTime = time;
        
        var result = context.EventGroups.Update(eventGroup);
        await context.SaveChangesAsync(cancellationToken);
        
        // TODO: update events
        
        await eventUpdateHandler.HandleAsync(result.Entity, EventUpdateHandler.UpdateAction.Update);

        return ValidationResult<EventGroup>.Success(result.Entity);
    }

    public async Task<ValidationError?> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var result = await context.EventGroups
            .Include(e => e.Events)
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) return null;

        if (!userContext.IsUserPermitted(result.UserId))
        {
            return new ValidationError($"User with ID {result.UserId} is not allowed to delete or modify the content of {id}");
        }

        var eventInstanceToday = result.Events.SingleOrDefault(e => e.Date == dateTimeProvider.CurrentDate);
        
        context.EventInstances.RemoveRange(result.Events);
        context.EventGroups.Remove(result);
        await context.SaveChangesAsync(cancellationToken);

        if (eventInstanceToday is not null)
        {
            await eventUpdateHandler.HandleAsync(eventInstanceToday, EventUpdateHandler.UpdateAction.Delete);
        }

        return null;
    }
}