using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Services.RepeatedEvents;
using WhoIsHome.WebApi.Models;

namespace WhoIsHome.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RepeatedEventController(IRepeatedEventService repeatedEventService) : WhoIsHomeControllerBase<RepeatedEvent, RepeatedEventModel>(repeatedEventService)
{
    [HttpPost]
    public async Task<ActionResult<RepeatedEventModel>> CreateEvent([FromBody] NewRepeatedEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await repeatedEventService.CreateAsync(
            eventName: eventModel.EventName,
            personId: eventModel.PersonId,
            startDate: eventModel.StartDate,
            endDate: eventModel.EndDate,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            relevantForDinner: eventModel.RelevantForDinner,
            dinnerAt: eventModel.DinnerAt,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    public async Task<ActionResult<RepeatedEventModel>> UpdateEvent([FromBody] RepeatedEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await repeatedEventService.UpdateAsync(
            id: eventModel.Id,
            eventName: eventModel.EventName,
            startDate: eventModel.StartDate,
            endDate: eventModel.EndDate,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            relevantForDinner: eventModel.RelevantForDinner,
            dinnerAt: eventModel.DinnerAt,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    protected override RepeatedEventModel ConvertToModel(RepeatedEvent data) => RepeatedEventModel.From(data);
}