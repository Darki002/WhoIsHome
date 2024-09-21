using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Request;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class RepeatedEventController(RepeatedEventService repeatedEventService) : AggregateControllerBase<RepeatedEvent, RepeatedEventModelResponse>(repeatedEventService)
{
    [HttpPost]
    public async Task<ActionResult<RepeatedEventModelResponse>> CreateEvent([FromBody] NewRepeatedEventModel eventModel, CancellationToken cancellationToken)
    {
        // TODO Authentication
        var userId = 1;
        
        var result = await repeatedEventService.CreateAsync(
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            userId: userId,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    public async Task<ActionResult<RepeatedEventModelResponse>> UpdateEvent([FromBody] RepeatedEventModel eventModel, CancellationToken cancellationToken) 
    {
        // TODO Authentication
        var userId = 1;
        
        var result = await repeatedEventService.UpdateAsync(
            id: eventModel.Id,
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            userId: userId,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    protected override RepeatedEventModelResponse ConvertToModel(RepeatedEvent data) => RepeatedEventModelResponse.From(data);
}