using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Response;
using OneTimeEventModel = WhoIsHome.WebApi.Models.Request.OneTimeEventModel;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class EventController(OneTimeEventService oneTimeEventService) : AggregateControllerBase<OneTimeEvent, OneTimeEventModelResponse>(oneTimeEventService)
{
    [HttpPost]
    public async Task<ActionResult<OneTimeEventModelResponse>> CreateEvent([FromBody] NewOneTimeEventModel eventModel, CancellationToken cancellationToken)
    {
        // TODO Authentication
        var userId = 1;
        
        var result = await oneTimeEventService.CreateAsync(
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            userId: userId,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    public async Task<ActionResult<OneTimeEventModelResponse>> UpdateEvent([FromBody] OneTimeEventModel eventModel, CancellationToken cancellationToken)
    {
        // TODO Authentication
        
        var result = await oneTimeEventService.UpdateAsync(
            id: eventModel.Id,
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }
    
    protected override OneTimeEventModelResponse ConvertToModel(OneTimeEvent data, User user) => OneTimeEventModelResponse.From(data, user);
}