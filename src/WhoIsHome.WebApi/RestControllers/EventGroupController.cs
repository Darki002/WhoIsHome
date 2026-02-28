using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhoIsHome.Entities;
using WhoIsHome.External.Database;
using WhoIsHome.Handlers;
using WhoIsHome.Services;
using WhoIsHome.Shared.Authentication;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi.Models;
using WhoIsHome.WebApi.Models.Dto;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.RestControllers;

[Authorize]
[Route("api/v1/event-group")]
public class EventGroupController(
    IEventService eventService,
    IEventUpdateHandler eventUpdateHandler,
    WhoIsHomeContext context, 
    IUserContextProvider userContextProvider, 
    IDateTimeProvider dateTimeProvider) : Controller
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<EventGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {id} not found."] });
        }

        return Ok(ToModel(result));
    }

    [HttpGet("{eventGroupId:int}/instance")]
    [ProducesResponseType<IReadOnlyList<EventInstanceModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInstancesAsync(
        int eventGroupId, 
        CancellationToken cancellationToken,
        [FromQuery] DateOnly? start = null,
        [FromQuery] int weeks = 2)
    {
        start ??= dateTimeProvider.CurrentDate;
        
        if (weeks > 8)
        {
            return BadRequest(new ErrorResponse
            {
                Errors = ["Weeks query parameter exited limit of 8 weeks."]
            });
        }

        var eventGroup = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == eventGroupId, cancellationToken);
        
        if (eventGroup is null)
        {
            return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {eventGroupId} not found."] });
        }
        
        var result = await eventService.PredictNextAsync(eventGroup, start.Value, weeks, cancellationToken);
        var model = result.Select(ToModel).ToList();
        return Ok(model);
    }
    
    [HttpGet("{eventGroupId:int}/instance/{date:datetime}")]
    [ProducesResponseType<EventInstanceModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInstanceByDateAsync(int eventGroupId, DateTime date, CancellationToken cancellationToken)
    {
        var originalDate = DateOnly.FromDateTime(date);
        var result = await context.EventInstances
            .Where(e => e.EventGroupId == eventGroupId)
            .SingleOrDefaultAsync(e => e.OriginalDate == originalDate, cancellationToken);

        if (result is null)
        {
            var eventGroup = await context.EventGroups.SingleOrDefaultAsync(e => e.Id == eventGroupId, cancellationToken);

            if (eventGroup is null)
            {
                return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {eventGroupId} not found."] });
            }
            
            result = new EventInstance
            {
                Id = 0,
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

        return Ok(ToModel(result));
    }
    
    [HttpPost]
    [ProducesResponseType<EventGroupModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEventAsync(
        [FromBody] EventGroupModelDto eventModelDto,
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
            userId: userContextProvider.UserId);

        var validationResult = eventGroup.Validate();
        if (validationResult.Count > 0)
        {
            return BadRequest(new ErrorResponse { Errors = validationResult.Select(e => e.Message) });
        }

        var result = await context.EventGroups.AddAsync(eventGroup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        await eventService.GenerateNewAsync(eventGroup); 
        return Ok(new { result.Entity.Id });
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEventAsync(
        int id,
        [FromBody] EventGroupModelDto dto,
        CancellationToken cancellationToken)
    {
        var eventGroup = await context.EventGroups.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        if (eventGroup is null)
        {
            return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {id} not found."] });
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
        
        var validationResult = eventGroup.Validate();
        if (validationResult.Count > 0)
        {
            return BadRequest(new { Error = validationResult.Select(e => e.Message) });
        }
        
        context.EventGroups.Update(eventGroup);
        await context.SaveChangesAsync(cancellationToken);

        if (regenerateEventInstances)
        {
            await eventService.GenerateUpdateAsync(eventGroup);
        }
        
        return Ok();
    }

    [HttpPatch("{eventGroupId:int}/instance/{date:datetime}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
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

            var eventGroup = await context.EventGroups
                .SingleOrDefaultAsync(g => g.Id == eventGroupId, cancellationToken);

            if (eventGroup is null)
            {
                return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {eventGroupId} not found."] });
            }

            eventInstance = new EventInstance
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

        var sendPushUp = eventInstance.Date == dateTimeProvider.CurrentDate;
        
        if (ModelState.ContainsKey(nameof(dto.Date)))
        {
            if (dto.Date < dateTimeProvider.CurrentDate)
            {
                return BadRequest(new  ErrorResponse { Errors = ["Date can not set into the past."] });
            }
            
            eventInstance.Date = dto.Date;
            eventInstance.IsOriginal = false;
        }
        
        if (ModelState.ContainsKey(nameof(dto.StartTime)))
        {
            eventInstance.StartTime = dto.StartTime;
            eventInstance.IsOriginal = false;
        }
        
        if (ModelState.ContainsKey(nameof(dto.EndTime)))
        {
            eventInstance.EndTime = dto.EndTime;
            eventInstance.IsOriginal = false;
        }
        
        if (ModelState.ContainsKey(nameof(dto.PresenceType)))
        {
            if (!PresenceTypeHelper.IsDefined(dto.PresenceType))
            {
                return BadRequest(new ErrorResponse { Errors = [$"Invalid presence type {dto.PresenceType}!"] });
            }
            
            eventInstance.PresenceType = PresenceTypeHelper.FromString(dto.PresenceType);
            eventInstance.IsOriginal = false;
        }
        
        if (ModelState.ContainsKey(nameof(dto.DinnerTime)))
        {
            eventInstance.DinnerTime = dto.DinnerTime;
            eventInstance.IsOriginal = false;
        }
        
        var validationResult = eventInstance.Validate();

        if (validationResult.Count > 0)
        {
            return BadRequest(new { Error = validationResult.Select(e => e.Message) });
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEventAsync(int id, CancellationToken cancellationToken)
    {
        var result = await context.EventGroups
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (result is null)
        {
            return BadRequest(new ErrorResponse { Errors = [$"EventGroup with id {id} not found."] });
        }

        if (!userContextProvider.IsUserPermitted(result.UserId))
        {
            return BadRequest( new ErrorResponse { Errors = [$"User with ID {result.UserId} is not allowed to delete or modify the content of {id}"] });
        }

        context.EventGroups.Remove(result);
        await context.SaveChangesAsync(cancellationToken);
        await eventService.DeleteAsync(id);
        return Ok();
    }
    
    [HttpDelete("{eventGroupId:int}/instance/{date:datetime}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
    
    private static EventGroupModel ToModel(EventGroup result)
    {
        return EventGroupModel.From(result);
    }
    
    private static EventInstanceModel ToModel(EventInstance result)
    {
        return EventInstanceModel.From(result);
    }
}