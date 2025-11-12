using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Types;
using WhoIsHome.Validations;

namespace WhoIsHome.Services;

internal class EventGroupService(
    WhoIsHomeContext context,
    IEventService eventService,
    IUserContext userContext) 
    : IEventGroupService
{
    public async Task<EventGroup> CreateAsync(
        string title, 
        DateOnly startDate, 
        DateOnly? endDate, 
        WeekDay weekDays, 
        TimeOnly startTime, 
        TimeOnly? endTime,
        PresenceType presenceType, 
        TimeOnly? dinnerTime, 
        CancellationToken cancellationToken)
    {
        var eventGroup = new EventGroup(
            title: title, 
            startDate: startDate, 
            endDate: endDate, 
            weekDays: weekDays, 
            startTime: startTime, 
            endTime: endTime, 
            presenceType: presenceType,
            dinnerTime: dinnerTime, 
            userId: userContext.UserId);

        var result = await context.EventGroups.AddAsync(eventGroup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        // When Template is created, we create the Events, no matter what
        await eventService.GenerateNewAsync(eventGroup, CancellationToken.None); 
        
        return result.Entity;
    }

    public async Task<ValidationResult<EventGroup>> UpdateAsync(
        int id, 
        string title, 
        DateOnly startDate, 
        DateOnly? endDate, 
        WeekDay weekDays, 
        TimeOnly startTime, 
        TimeOnly? endTime,
        PresenceType presenceType, 
        TimeOnly? dinnerTime, 
        CancellationToken cancellationToken)
    {
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
        eventGroup.DinnerTime = dinnerTime;
        
        var result = context.EventGroups.Update(eventGroup);
        await context.SaveChangesAsync(cancellationToken);

        // When Template is updated, we update the Events, no matter what
        await eventService.GenerateUpdateAsync(eventGroup, CancellationToken.None);

        return ValidationResult<EventGroup>.Success(result.Entity);
    }

    public async Task<ValidationError?> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null) return null;

        if (!userContext.IsUserPermitted(result.UserId))
        {
            return new ValidationError($"User with ID {result.UserId} is not allowed to delete or modify the content of {id}");
        }
        
        await eventService.DeleteAsync(id);
        context.EventGroups.Remove(result);
        await context.SaveChangesAsync(cancellationToken);

        return null;
    }
}