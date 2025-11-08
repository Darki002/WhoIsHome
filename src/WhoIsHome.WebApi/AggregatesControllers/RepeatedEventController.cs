using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Entities;
using WhoIsHome.Services;
using WhoIsHome.Shared.Helper;
using WhoIsHome.Shared.Types;
using WhoIsHome.WebApi.Models.Dto;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

[Route("event-groups")]
[Authorize]
public class EventGroupController(IEventGroupService eventGroupService) : Controller
{
    [HttpPost]
    public async Task<ActionResult<EventGroupModel>> CreateEventAsync([FromBody] EventGroupModelDto eventModelDto,
        CancellationToken cancellationToken)
    {
        var result = await eventGroupService.CreateAsync(
            title: eventModelDto.Title,
            firstOccurrence: eventModelDto.FirstOccurrence,
            lastOccurrence: eventModelDto.LastOccurrence,
            startTime: eventModelDto.StartTime,
            endTime: eventModelDto.EndTime,
            weekDays: eventModelDto.WeekDays.ToWeekDays(),
            presenceType: PresenceTypeHelper.FromString(eventModelDto.PresenceType),
            time: eventModelDto.DinnerTime,
            cancellationToken: cancellationToken);

        return Ok(ToModel(result));
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<EventGroupModel>> UpdateEventAsync(
        int id,
        [FromBody] EventGroupModelDto eventModelDto,
        CancellationToken cancellationToken)
    {
        var result = await eventGroupService.UpdateAsync(
            id: id,
            title: eventModelDto.Title,
            startDate: eventModelDto.FirstOccurrence,
            endDate: eventModelDto.LastOccurrence,
            startTime: eventModelDto.StartTime,
            endTime: eventModelDto.EndTime,
            weekDays: eventModelDto.WeekDays.ToWeekDays(),
            presenceType: PresenceTypeHelper.FromString(eventModelDto.PresenceType),
            time: eventModelDto.DinnerTime,
            cancellationToken: cancellationToken);

        if (result.HasErrors)
        {
            return BadRequest(result.ValidationErrors.Select(e => e.Message));
        }

        return Ok(ToModel(result.Result));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<EventGroupModel>> DeleteEventAsync(int id, CancellationToken cancellationToken)
    {
        var result = await eventGroupService.DeleteAsync(id: id, cancellationToken: cancellationToken);

        if (result is not null)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
    
    private static ActionResult<EventGroupModel> ToModel(EventGroup result)
    {
        return EventGroupModel.From(result);
    }
}