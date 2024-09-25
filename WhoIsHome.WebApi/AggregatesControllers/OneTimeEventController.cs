using Microsoft.AspNetCore.Mvc;
using WhoIsHome.Aggregates;
using WhoIsHome.Services;
using WhoIsHome.WebApi.Models.New;
using WhoIsHome.WebApi.Models.Response;
using OneTimeEventModel = WhoIsHome.WebApi.Models.Request.OneTimeEventModel;

namespace WhoIsHome.WebApi.AggregatesControllers;

public class OneTimeEventController(OneTimeEventAggregateAggregateService oneTimeEventAggregateAggregateService) : AggregateControllerBase<OneTimeEvent, OneTimeEventModelResponse>(oneTimeEventAggregateAggregateService)
{
    [HttpPost]
    public async Task<ActionResult<OneTimeEventModelResponse>> CreateEvent([FromBody] NewOneTimeEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateAggregateService.CreateAsync(
            title: eventModel.Title,
            date: eventModel.Date,
            startTime: eventModel.StartTime,
            endTime: eventModel.EndTime,
            dinnerTime: eventModel.DinnerTime,
            cancellationToken: cancellationToken);
        
        return BuildResponse(result);
    }

    [HttpPut]
    public async Task<ActionResult<OneTimeEventModelResponse>> UpdateEvent([FromBody] OneTimeEventModel eventModel, CancellationToken cancellationToken)
    {
        var result = await oneTimeEventAggregateAggregateService.UpdateAsync(
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