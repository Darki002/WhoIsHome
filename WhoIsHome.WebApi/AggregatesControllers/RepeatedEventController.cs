using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Request;
using WhoIsHome.WebApi.Models.Response;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class RepeatedEventController(RepeatedEventAggregateAggregateService repeatedEventAggregateAggregateService) : AggregateControllerBase<RepeatedEvent, RepeatedEventModelResponse>(repeatedEventAggregateAggregateService)
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RepeatedEventModelResponse>> CreateEvent([FromBody] NewRepeatedEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await repeatedEventAggregateAggregateService.CreateAsync(
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<RepeatedEventModelResponse>> UpdateEvent([FromBody] RepeatedEventModel eventModel, CancellationToken cancellationToken) 
    {
        var result = await repeatedEventAggregateAggregateService.UpdateAsync(
            id: eventModel.Id,
            title: eventModel.Title,
            firstOccurrence: eventModel.FirstOccurrence,
            lastOccurrence: eventModel.LastOccurrence,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }
    
    protected override RepeatedEventModelResponse ConvertToModel(RepeatedEvent data, User user) => RepeatedEventModelResponse.From(data, user);
}