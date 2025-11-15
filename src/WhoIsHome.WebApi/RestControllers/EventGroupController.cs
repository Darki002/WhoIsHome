using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi.Models.Dto;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.RestControllers;

[Authorize]
[Route("event-group")]
public class EventGroupController(
    IEventService eventService,
    IEventUpdateHandler eventUpdateHandler,
    WhoIsHomeContext context, 
    IUserContext userContext, 
    IDateTimeProvider dateTimeProvider) : Controller
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            return BadRequest($"EventGroup with id {id} not found.");
        }

        return Ok(ToModel(result));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventGroupModelDto eventModelDto,
        CancellationToken cancellationToken)
    {
        var eventGroup = new EventGroup(
            title: eventModelDto.Title, 
            startDate: eventModelDto.StartDate, 
            endDate: eventModelDto.EndDate, 
            weekDays: eventModelDto.WeekDays.ToWeekDays(), 
            startTime: eventModelDto.StartTime, 
            endTime: eventModelDto.EndTime, 
            presenceType: PresenceTypeHelper.FromString(eventModelDto.PresenceType),
            dinnerTime: eventModelDto.DinnerTime,
            userId: userContext.UserId);

        var validationResult = eventGroup.Validate();
        if (validationResult.Count > 0)
        {
            return BadRequest(new { Error = validationResult.Select(e => e.Message) });
        }

        var result = await context.EventGroups.AddAsync(eventGroup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        await eventService.GenerateNewAsync(eventGroup); 
        return Ok(ToModel(result.Entity));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateEventAsync(
        int id,
        [FromBody] EventGroupModelDto dto,
        CancellationToken cancellationToken)
    {
        var eventGroup = await context.EventGroups.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        if (eventGroup is null)
        {
            return BadRequest(new { Error = $"EventGroup with id {id} not found." });
        }

        var regenerateEventInstances = false;

        if (ModelState.ContainsKey(nameof(dto.Title)))
        {
            eventGroup.Title = dto.Title;
        }
        
        if (ModelState.ContainsKey(nameof(dto.StartDate)))
        {
            eventGroup.StartDate = dto.StartDate;
            regenerateEventInstances = true;
        }
        
        if (ModelState.ContainsKey(nameof(dto.EndDate)))
        {
            eventGroup.EndDate = dto.EndDate;
            regenerateEventInstances = true;
        }
        
        if (ModelState.ContainsKey(nameof(dto.StartTime)))
        {
            eventGroup.StartTime = dto.StartTime;
            regenerateEventInstances = true;
        }
        
        if (ModelState.ContainsKey(nameof(dto.EndTime)))
        {
            eventGroup.EndTime = dto.EndTime;
            regenerateEventInstances = true;
        }
        
        if (ModelState.ContainsKey(nameof(dto.WeekDays)))
        {
            eventGroup.WeekDays = dto.WeekDays.ToWeekDays();
            regenerateEventInstances = true;
        }
        
        if (ModelState.ContainsKey(nameof(dto.PresenceType)))
        {
            eventGroup.PresenceType = PresenceTypeHelper.FromString(dto.PresenceType);
        }
        
        if (ModelState.ContainsKey(nameof(dto.DinnerTime)))
        {
            eventGroup.DinnerTime = dto.DinnerTime;
        }
        
        context.EventGroups.Update(eventGroup);
        await context.SaveChangesAsync(cancellationToken);

        var validationResult = eventGroup.Validate();
        if (validationResult.Count > 0)
        {
            return BadRequest(new { Error = validationResult.Select(e => e.Message) });
        }

        if (regenerateEventInstances)
        {
            await eventService.GenerateUpdateAsync(eventGroup);
        }
        
        return Ok();
    }

    [HttpPatch("{eventGroupId:int}/instance/{date:datetime}")]
    public async Task<IActionResult> EditEventInstance(
        int eventGroupId, 
        DateTime date, 
        [FromBody] EventInstanceDto dto, 
        CancellationToken cancellationToken)
    {
        var originalDate = DateOnly.FromDateTime(date);
        
        var eventInstance = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .SingleOrDefaultAsync(e => e.OriginalDate == originalDate, cancellationToken);
        
        if (eventInstance is null)
        {
            return BadRequest(new { Error = $"No Event found with date {eventGroupId}." });
        }

        var sendPushUp = eventInstance.Date == dateTimeProvider.CurrentDate;
        
        if (ModelState.ContainsKey(nameof(dto.Title)))
        {
            eventInstance.Title = dto.Title;
        }
        
        if (ModelState.ContainsKey(nameof(dto.Date)))
        {
            eventInstance.Date = dto.Date;
        }
        
        if (ModelState.ContainsKey(nameof(dto.StartTime)))
        {
            eventInstance.StartTime = dto.StartTime;
        }
        
        if (ModelState.ContainsKey(nameof(dto.EndTime)))
        {
            eventInstance.EndTime = dto.EndTime;
        }
        
        if (ModelState.ContainsKey(nameof(dto.PresenceType)))
        {
            eventInstance.PresenceType = PresenceTypeHelper.FromString(dto.PresenceType);
        }
        
        if (ModelState.ContainsKey(nameof(dto.DinnerTime)))
        {
            eventInstance.DinnerTime = dto.DinnerTime;
        }
        
        context.EventInstances.Update(eventInstance);
        await context.SaveChangesAsync(cancellationToken);

        if (sendPushUp)
        {
            await eventUpdateHandler.HandleAsync(eventInstance, EventUpdateHandler.UpdateAction.Update);
        }
        
        return Ok();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEventAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            return BadRequest(new { Error = $"EventGroup with id {id} not found." });
        }

        if (!userContext.IsUserPermitted(result.UserId))
        {
            return BadRequest( new { Error = $"User with ID {result.UserId} is not allowed to delete or modify the content of {id}" });
        }

        context.EventGroups.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
        await eventService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpDelete("{eventGroupId:int}/instance/{date:datetime}")]
    public async Task<IActionResult> DeleteEventInstance(
        int eventGroupId, 
        DateTime date,
        CancellationToken cancellationToken)
    {
        var originalDate = DateOnly.FromDateTime(date);
        var eventInstance = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .SingleOrDefaultAsync(e => e.OriginalDate == originalDate, cancellationToken);

        if (eventInstance is null) return Ok();
        context.EventInstances.Remove(eventInstance);
        await context.SaveChangesAsync(cancellationToken);
            
        if (eventInstance.Date == dateTimeProvider.CurrentDate)
        {
            await eventUpdateHandler.HandleAsync(eventInstance, EventUpdateHandler.UpdateAction.Update);
        }

        return Ok();
    }
    
    private static ActionResult<EventGroupModel> ToModel(EventGroup result)
    {
        return EventGroupModel.From(result);
    }
}